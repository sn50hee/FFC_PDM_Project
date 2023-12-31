﻿using ScottPlot;
using ScottPlot.Plottable;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using XGCommLib;
using static System.Runtime.InteropServices.JavaScript.JSType;

// main에서는 차트만 보여주기

namespace FFC_PDM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<ParseTelemetry> rawTelemetryData;
        //public PlcDataAccessHandler handler = new PlcDataAccessHandler(CheckDataList);
        public MainWindow()
        {
            InitializeComponent(); 

            // 윤석희 추가
            WP_RecentFacilityView();
            WP_ErrorRateView();
            WP_OperatingRatioView();
            DG_FailuressListView();
            WP_BackgroundAlpha();
            // 윤석희끝
            GenerateCBModelName();
            DisableClick();

            InitializeTextBoxHandlers();

            // 통신 시작
        }

        public void WP_BackgroundAlpha()
        {
            WpfPlot[] wpfPlots = { WP_Volt, WP_Rotate, WP_Pressure, WP_Vibration, WP_Warning, WP_Maint };
            foreach(WpfPlot chart in wpfPlots)
            {
                Plot plt = chart.Plot;
                plt.Style(
                    figureBackground: System.Drawing.Color.FromArgb(0, 0, 0, 0),
                    dataBackground: System.Drawing.Color.FromArgb(0, 0, 0, 0));
            }

        }


        public void WP_OperatingRatioView()
        {
            StatisticsTabChartData statisticsTabChartData = new StatisticsTabChartData();
            List<(System.DateTime, double)> data = statisticsTabChartData.TelemetryDataListToDict();

            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Tick += (sender, e) => {
                WP_OperatingRatio.Plot.Clear();
                WP_OperatingRatio = statisticsTabChartData.GetPointsToPlot(WP_OperatingRatio, data, 10);
                WP_OperatingRatio.Refresh();
            };
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Start();
        }

        ObservableCollection<StatisticsTabGridData> RiskOfFailuressData = new ObservableCollection<StatisticsTabGridData>();
        public void DG_FailuressListView()
        {
            StatisticsTabChartData statisticsTabChartData = new StatisticsTabChartData();
            List<StatisticsTabGridData> data = statisticsTabChartData.GetFailuressListViewData(200,360,120,50);

            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Tick += (sender, e) => {
                RiskOfFailuressData.Add(statisticsTabChartData.RiskOfFailuressDataPlot(DG_FailuressList, data, 1));
                DG_FailuressList.ItemsSource = RiskOfFailuressData;
            };
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Start();

        }

        public void WP_RecentFacilityView()
        {
            FacilityDataChartControl facilityDataChartControl = new FacilityDataChartControl();
            Dictionary<string, int> data = new StatisticsTabChartData().RecentFacilityData();
            WP_RecentFacility = facilityDataChartControl.CreateBarChart(WP_RecentFacility, data, "최근 10건 고장 장비", true, false);
            WP_RecentFacility.Refresh();
        }

        public void WP_ErrorRateView()
        {
            FacilityDataChartControl facilityDataChartControl = new FacilityDataChartControl();
            Dictionary<string, int> data = new StatisticsTabChartData().ErrorRateData();
            WP_ErrorRate = facilityDataChartControl.CreateBarChart(WP_ErrorRate, data, "오류 횟수", true, true);
            WP_ErrorRate.Refresh();
        }

        public void GenerateCBModelName()
        {
            SearchDataFilterl searchDataFilterl = new SearchDataFilterl();
            CB_ModelName = searchDataFilterl.MadeComboBox(CB_ModelName);

        }


        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CB_Model_ID.Items.Clear();
            //MessageBox.Show("작동함?");
            // 두번째 콤보박스에 1 2 3 4 아이템 추가하기
            // 1. 함수에서 리스트를 리턴하고, 리턴된 리스트를 받아서 메시지박스로 출력하는 테스트
            SearchDataFilterl sdf = new SearchDataFilterl();
            Dictionary<string, List<double>> machines;
            List<double> Id_list;
            /* 새로운 리스트 machineIDList를 생성해서 GetValues()에서 리턴으로 넘어오는 리스트를 받기 */
            machines = sdf.MadeIDComboBox();
            machines[CB_ModelName.SelectedItem.ToString()].Sort();
            Id_list = machines[CB_ModelName.SelectedItem.ToString()];
            foreach (double i in machines[CB_ModelName.SelectedItem.ToString()])
            {

                CB_Model_ID.Items.Add((i).ToString()+"번 설비");
            }



            // machineIDList를 콤보박스에 추가하기
        }

        // 김정관 추가
        private void UpdateGraph(int selectedModelID, DateTime startDate, DateTime endDate) // 회전 데이터 표시 차트 업데이트 메서드
        {
            viewDetailsTabChartDataControl = new ViewDetailsTabChartDataControl();
            List<ParseTelemetry_1> date = new FacilityDataControl().GetParseTelemetryData(); // 김정관 수정

            WP_Volt.Plot.Clear();
            WP_Rotate.Plot.Clear();
            WP_Pressure.Plot.Clear();
            WP_Vibration.Plot.Clear();

            WP_Volt = viewDetailsTabChartDataControl.CreateVoltageChart(WP_Volt, date, "VoltageGraph", selectedModelID, startDate, endDate);
            var voltSpan = WP_Volt.Plot.AddVerticalSpan(200, 10000);
            voltSpan.Color = System.Drawing.Color.FromArgb(100, System.Drawing.Color.Red);
            WP_Volt.Refresh();

            WP_Rotate = viewDetailsTabChartDataControl.CreateRotateChart(WP_Rotate, date, "RotateGraph", selectedModelID, startDate, endDate);
            var rotateSpan = WP_Rotate.Plot.AddVerticalSpan(360, -10000);
            rotateSpan.Color = System.Drawing.Color.FromArgb(100, System.Drawing.Color.Red);
            WP_Rotate.Refresh();

            WP_Pressure = viewDetailsTabChartDataControl.CreatePressureChart(WP_Pressure, date, "PressureGraph", selectedModelID, startDate, endDate);
            var pressureSpan = WP_Pressure.Plot.AddVerticalSpan(120, 10000);
            pressureSpan.Color = System.Drawing.Color.FromArgb(100, System.Drawing.Color.Red);
            WP_Pressure.Refresh();

            WP_Vibration = viewDetailsTabChartDataControl.CreateVibrationChart(WP_Vibration, date, "VibrationGraph", selectedModelID, startDate, endDate);
            var vibrationSpan = WP_Vibration.Plot.AddVerticalSpan(50, 10000);
            vibrationSpan.Color = System.Drawing.Color.FromArgb(100, System.Drawing.Color.Red);
            WP_Vibration.Refresh();
        }

        public void UpdateWarningGraph(double selectedModelID, DateTime startDate, DateTime endDate)
        {
            // FacilityDataChartControl 및 StatisticsTabChartData 인스턴스 생성
            StatisticsTabChartData statisticsTabChartData = new StatisticsTabChartData();

            // 선택한 machineID에 대한 에러 데이터 가져오기
            Dictionary<string, int> data = statisticsTabChartData.GetParseErrorsData(selectedModelID, startDate, endDate);

            WP_Warning.Plot.Clear();
            if (data.Any())
            {
                // 파이 차트 업데이트
                WP_Warning = viewDetailsTabChartDataControl.CreateWarningPieChart(WP_Warning, data);
                WP_Warning.Refresh();
            }

        }

        public void UpdateMaintGraph(double selectedModelID, DateTime startDate, DateTime endDate)
        {
            // FacilityDataChartControl 및 StatisticsTabChartData 인스턴스 생성
            StatisticsTabChartData statisticsTabChartData = new StatisticsTabChartData();

            // 통계 데이터 가져오기
            Dictionary<string, int> data = statisticsTabChartData.GetParseMaintData(selectedModelID, startDate, endDate);

            WP_Maint.Plot.Clear();
            if (data.Any())
            {
                WP_Maint = viewDetailsTabChartDataControl.CreateMaintPieChart(WP_Maint, data);
                WP_Maint.Refresh();
            }
            
        }

        private void Search_Click(object sender, RoutedEventArgs e) //값이 비어져있으면 메시지박스에 입력안한 항목있다고 띄어주기, 
        {
            if(CB_Model_ID.SelectedValue==null || startDate.SelectedDate==null || endDate.SelectedDate == null)
            {
                MessageBox.Show("선택하지 않은 항목이 있습니다.");
            }
            else
            {
                string selectedValue = CB_Model_ID.SelectedValue.ToString();
                // CB_Model_ID에서 선택된 값 가져오기
                int? selectedModelIDNullable = int.Parse(selectedValue.Substring(0, selectedValue.Length - 4));
                // 만약 선택된 값이 null이면 기본값인 0으로 대체
                int selectedModelID = selectedModelIDNullable ?? 0; // null이면 0으로 처리
                                                                    // DatePicker에서 선택된 시작 날짜 가져오기 --> 00시부터하고
                DateTime startDateValue = startDate.SelectedDate?.Date ?? DateTime.MinValue;
                // DatePicker에서 선택된 종료 날짜 가져오기 --> 23시 59분까지 받아올 수 있게
                DateTime endDateValue = endDate.SelectedDate?.Date.AddDays(1).AddSeconds(-1) ?? DateTime.MaxValue;

                if(startDateValue >= endDateValue)
                {
                    MessageBox.Show("종료일은 시작일보다 작을 수 없습니다.");
                }
                else
                {
                    //김정관 추가
                    UpdateGraph(selectedModelID, startDateValue, endDateValue);
                    UpdateWarningGraph(selectedModelID, startDateValue, endDateValue);
                    UpdateMaintGraph(selectedModelID, startDateValue, endDateValue);
                    //김정관 끝
                }

            }

        }

        private void Btn_check_Click(object sender, RoutedEventArgs e)
        {
            List<string> inputDataList1 = new List<string>();
            List<string> inputDataList2 = new List<string>();

            // 리스트를 생성하고 DG_checkData의 데이터를 복사
            List<CheckData> gridDatas = new List<CheckData>(DG_checkData.ItemsSource.OfType<CheckData>());

            foreach (CheckData data in gridDatas)
            {
                inputDataList1.Add($"[[{data.ModelId.ToString()},{data.Age.ToString()}, {data.VoltMin.ToString()}, {data.RotateMin.ToString()}, {data.PressureMin.ToString()}, {data.VibrationMin.ToString()}]]");
                inputDataList2.Add($"[[{data.ModelId.ToString()},{data.Age.ToString()}, {data.VoltMax.ToString()}, {data.RotateMax.ToString()}, {data.PressureMax.ToString()}, {data.VibrationMax.ToString()}]]");
            }

            GetPythonModel getPythonModel = new GetPythonModel();
            List<string> outputDataList1 = getPythonModel.FailureCheck(inputDataList1);
            List<string> outputDataList2 = getPythonModel.FailureCheck(inputDataList2);

            for (int i = 0; i < outputDataList1.Count; i++)
            {
                if (outputDataList1[i] == "[1]\r\n" || outputDataList2[i] == "[1]\r\n")
                {
                    gridDatas[i].Failure = "고장 위험";
                }
                else
                {
                    gridDatas[i].Failure = "안전";
                }
            }

            // 수정된 데이터로 다시 바인딩
            DG_checkData.ItemsSource = gridDatas;
            DG_checkData.Items.Refresh();
        }
        // 김정관 수정끝

        ViewDetailsTabChartDataControl viewDetailsTabChartDataControl;
        private void WP_Volt_MouseMove(object sender, MouseEventArgs e)
        {
            if (viewDetailsTabChartDataControl != null)
            {
                WP_Volt = viewDetailsTabChartDataControl.Volt_MouseMove(WP_Volt, e);
                WP_Volt.Refresh();
            }
        }
        private void WP_Volt_MouseLeave(object sender, MouseEventArgs e)
        {
            if (viewDetailsTabChartDataControl != null)
            {
                WP_Volt = viewDetailsTabChartDataControl.Volt_MouseLeave(WP_Volt, e);
                WP_Volt.Refresh();
            }
        }

        private void WP_Rotate_MouseMove(object sender, MouseEventArgs e)
        {
            if (viewDetailsTabChartDataControl != null)
            {
                WP_Rotate = viewDetailsTabChartDataControl.Rotate_MouseMove(WP_Rotate, e);
                WP_Rotate.Refresh();
            }
        }
        private void WP_Rotate_MouseLeave(object sender, MouseEventArgs e)
        {
            if (viewDetailsTabChartDataControl != null)
            {
                WP_Rotate = viewDetailsTabChartDataControl.Rotate_MouseLeave(WP_Rotate, e);
                WP_Rotate.Refresh();
            }
        }

        private void WP_Pressure_MouseMove(object sender, MouseEventArgs e)
        {
            if (viewDetailsTabChartDataControl != null)
            {
                WP_Pressure = viewDetailsTabChartDataControl.Pressure_MouseMove(WP_Pressure, e);
                WP_Pressure.Refresh();
            }
        }
        private void WP_Pressure_MouseLeave(object sender, MouseEventArgs e)
        {
            if (viewDetailsTabChartDataControl != null)
            {
                WP_Pressure = viewDetailsTabChartDataControl.Pressure_MouseLeave(WP_Pressure, e);
                WP_Pressure.Refresh();
            }
        }

        private void WP_Vibration_MouseMove(object sender, MouseEventArgs e)
        {
            if (viewDetailsTabChartDataControl != null)
            {
                WP_Vibration = viewDetailsTabChartDataControl.Vibration_MouseMove(WP_Vibration, e);
                WP_Vibration.Refresh();
            }
        }
        private void WP_Vibration_MouseLeave(object sender, MouseEventArgs e)
        {
            if (viewDetailsTabChartDataControl != null)
            {
                WP_Vibration = viewDetailsTabChartDataControl.Vibration_MouseLeave(WP_Vibration, e);
                WP_Vibration.Refresh();
            }
        }

        List<CheckData> CheckDataList = new List<CheckData>();
        private ModelAgeManager modelAgeManager = new ModelAgeManager();
        private void Btn_apply_Click(object sender, RoutedEventArgs e)
        {
            CheckDataList.Clear(); // modelID 중복금지 코드

            if (string.IsNullOrWhiteSpace(volt_min.Text) || string.IsNullOrWhiteSpace(volt_max.Text) || string.IsNullOrWhiteSpace(rot_max.Text) || string.IsNullOrWhiteSpace(rot_min.Text) || string.IsNullOrWhiteSpace(press_min.Text) || string.IsNullOrWhiteSpace(press_max.Text) || string.IsNullOrWhiteSpace(vibe_min.Text) || string.IsNullOrWhiteSpace(vibe_max.Text))
            {
                MessageBox.Show("입력하지 않은 값이 있습니다. 값을 입력하세요.");
                return;
            }

            for (int modelId = 1; modelId <= 8; modelId++)
            {
                int ageForModel = modelAgeManager.GetAgeForModel(modelId);
                CheckData newData = new CheckData
                {
                    ModelId = modelId.ToString(),
                    Age = ageForModel,
                    VoltMin = double.Parse(volt_min.Text),
                    VoltMax = double.Parse(volt_max.Text),
                    RotateMin = double.Parse(rot_min.Text),
                    RotateMax = double.Parse(rot_max.Text),
                    PressureMin = double.Parse(press_min.Text),
                    PressureMax = double.Parse(press_max.Text),
                    VibrationMin = double.Parse(vibe_min.Text),
                    VibrationMax = double.Parse(vibe_max.Text),
                    Failure = ""
                };

                // List에 데이터 추가
                CheckDataList.Add(newData);
            }

            // 데이터 그리드 업데이트
            DG_checkData.ItemsSource = CheckDataList;
            DG_checkData.Items.Refresh();

            Btn_check.IsEnabled = true;
            Btn_plc.IsEnabled = true;
        }

        private void Btn_plc_Click(object sender, RoutedEventArgs e)
        {
            List<CheckData> gridDatas = new List<CheckData>(DG_checkData.ItemsSource.OfType<CheckData>());

            PlcDataAccessHandler handler = new PlcDataAccessHandler(gridDatas);
            
            handler.Connect_Start();
        }

        //김정관 시작
        // 숫자만 입력 가능한 메서드
        private void NumericInputOnly(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsNumericInput(e.Text);
        }

        private bool IsNumericInput(string text)
        {
            return text.All(char.IsDigit);
        }

        // 띄어쓰기 제한
        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }


        private void InitializeTextBoxHandlers()
        {
            AddPastingHandlerToTextBox(volt_min);
            AddPastingHandlerToTextBox(rot_min);
            AddPastingHandlerToTextBox(vibe_min);
            AddPastingHandlerToTextBox(press_min);
            AddPastingHandlerToTextBox(volt_max);
            AddPastingHandlerToTextBox(rot_max);
            AddPastingHandlerToTextBox(vibe_max);
            AddPastingHandlerToTextBox(press_max);
        }

        // 텍스트 박스 붙여넣기 only 숫자만 가능
        private void AddPastingHandlerToTextBox(TextBox textBox)
        {
            DataObject.AddPastingHandler(textBox, TextBox_Pasting);
        }

        private void TextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string pastedText = (string)e.DataObject.GetData(typeof(string));

                if (!IsNumericInput(pastedText))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }


        private void DisableClick()
        {
            if (DG_checkData.Items.Count == 0)
            {
                Btn_check.IsEnabled = false;
                Btn_plc.IsEnabled = false;
            }
        }

        //김정관 끝
    }

}
