using ScottPlot;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
        public MainWindow()
        {
            InitializeComponent();

            // 윤석희 추가
            WP_RecentFacilityView();
            WP_ErrorRateView();
            WP_OperatingRatioView();
            DG_FailuressListView();
            // 윤석희끝

            GenerateCBModelName();
            

            //김정관 추가
            UpdateWarningGraph();
            UpdateMaintGraph();
            //김정관 끝

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
            WP_RecentFacility = facilityDataChartControl.CreateBarChart(WP_RecentFacility, data, "최근 10건 고장 모델", true);
            WP_RecentFacility.Refresh();
        }

        public void WP_ErrorRateView()
        {
            FacilityDataChartControl facilityDataChartControl = new FacilityDataChartControl();
            Dictionary<string, int> data = new StatisticsTabChartData().ErrorRateData();
            WP_ErrorRate = facilityDataChartControl.CreateBarChart(WP_ErrorRate, data, "오류 횟수", true);
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

                CB_Model_ID.Items.Add((i).ToString());
            }



            // machineIDList를 콤보박스에 추가하기
        }

        // 김정관 추가
        private void UpdateGraph(int selectedModelID, DateTime startDate, DateTime endDate) // 회전 데이터 표시 차트 업데이트 메서드
        {
            FacilityDataChartControl facilityDataChartControl = new FacilityDataChartControl();
            List<ParseTelemetry_1> date = new FacilityDataControl().GetParseTelemetryData(); // 김정관 수정

            WP_Volt.Plot.Clear();
            WP_Rotate.Plot.Clear();
            WP_Pressure.Plot.Clear();
            WP_Vibration.Plot.Clear();

            WP_Volt = facilityDataChartControl.CreateVoltageChart(WP_Volt, date, "VoltageGraph", selectedModelID, startDate, endDate);
            WP_Volt.Plot.AddVerticalSpan(200, 10000);
            WP_Volt.Refresh();

            WP_Rotate = facilityDataChartControl.CreateRotateChart(WP_Rotate, date, "RotateGraph", selectedModelID, startDate, endDate);
            WP_Rotate.Plot.AddVerticalSpan(360, -10000);
            WP_Rotate.Refresh();

            WP_Pressure = facilityDataChartControl.CreatePressureChart(WP_Pressure, date, "PressureGraph", selectedModelID, startDate, endDate);
            WP_Pressure.Plot.AddVerticalSpan(120, 10000);
            WP_Pressure.Refresh();

            WP_Vibration = facilityDataChartControl.CreateVibrationChart(WP_Vibration, date, "VibrationGraph", selectedModelID, startDate, endDate);
            WP_Vibration.Plot.AddVerticalSpan(50, 10000);
            WP_Vibration.Refresh();
        }

        public void UpdateWarningGraph()
        {
            // FacilityDataChartControl 및 StatisticsTabChartData 인스턴스 생성
            FacilityDataChartControl facilityDataChartControl = new FacilityDataChartControl();
            StatisticsTabChartData statisticsTabChartData = new StatisticsTabChartData();

            // 통계 데이터 가져오기
            Dictionary<string, int> data = statisticsTabChartData.GetParseErrorsData();

            // 파이 차트 업데이트
            WP_Warning = facilityDataChartControl.CreateWarningPieChart(WP_Warning, data, "고장 모델 비율");
            WP_Warning.Refresh();
        }

        public void UpdateMaintGraph()
        {
            // FacilityDataChartControl 및 StatisticsTabChartData 인스턴스 생성
            FacilityDataChartControl facilityDataChartControl = new FacilityDataChartControl();
            StatisticsTabChartData statisticsTabChartData = new StatisticsTabChartData();

            // 통계 데이터 가져오기
            Dictionary<string, int> data = statisticsTabChartData.GetParseMaintData();

            // 파이 차트 업데이트
            WP_Maint = facilityDataChartControl.CreateMaintPieChart(WP_Maint, data, "유지 보수");
            WP_Maint.Refresh();
        }

        private void Search_Click(object sender, RoutedEventArgs e) //값이 비어져있으면 메시지박스에 입력안한 항목있다고 띄어주기, 
        {
            // CB_Model_ID에서 선택된 값 가져오기
            int? selectedModelIDNullable = CB_Model_ID.SelectedValue as int?;
            // 만약 선택된 값이 null이면 기본값인 0으로 대체
            int selectedModelID = selectedModelIDNullable ?? 0; // null이면 0으로 처리
            // DatePicker에서 선택된 시작 날짜 가져오기
            DateTime startDateValue = startDate.SelectedDate ?? DateTime.MinValue;
            // DatePicker에서 선택된 종료 날짜 가져오기
            DateTime endDateValue = endDate.SelectedDate ?? DateTime.MaxValue;


            UpdateGraph(selectedModelID, startDateValue, endDateValue);
        }
        // 김정관 끝

        ObservableCollection<StatisticsTabGridData> gridDatas = new ObservableCollection<StatisticsTabGridData>();
        
        private void AddRowButton_Click(object sender, RoutedEventArgs e)
        {
            // + 버튼 클릭 시 새로운 행 추가
            gridDatas.Add(new StatisticsTabGridData { age = null, modelId = null, pressure = null, rotate = null, vibration = null, volt = null, failure = null});
            DG_checkData.ItemsSource = gridDatas;
        }

        private void RemoveRowButton_Click(object sender, RoutedEventArgs e)
        {
            if (DG_checkData.Items.Count > 0)
            {
                int lastIndex = DG_checkData.Items.Count - 1;
                gridDatas.RemoveAt(lastIndex);
            }
        }

        private void Btn_check_Click(object sender, RoutedEventArgs e)
        {
            List<string> inputDataList = new List<string>();
            foreach(StatisticsTabGridData data in gridDatas)
            {
                if(data.volt == null ||  data.rotate == null || data.pressure == null || data.vibration == null || data.modelId == null || data.age == null)
                {
                    MessageBox.Show("모든 값을 입력해야 합니다");
                    break;
                }

                inputDataList.Add($"[[{data.modelId.ToString()},{data.age.ToString()},{data.volt.ToString()},{data.rotate.ToString()},{data.pressure.ToString()},{data.vibration.ToString()}]]");
            }
            GetPythonModel getPythonModel = new GetPythonModel();
            List<string> outputDataList = getPythonModel.FailureCheck(inputDataList);

            for(int i = 0; i < outputDataList.Count; i++)
            {
                if (outputDataList[i] == "[1]\r\n")
                {
                    gridDatas[i].failure = "고장 위험";
                }
                else
                {
                    gridDatas[i].failure = "안전";
                }
            }

            DG_checkData.Items.Refresh();
        }
    }

}
