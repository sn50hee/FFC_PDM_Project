using ScottPlot;
using System;
using System.Collections.Generic;
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
            //GridDataView();
            Col1View();
            Col2View();
            Col3View();
            GenerateCBModelName();

            //김정관 추가
            UpdateVoltageGraph();
            UpdateRotateGraph();
            UpdatePressureGraph();
            UpdateVibrationGraph();
            //김정관 끝

        }

        public void GridDataView() // 샘플데이터 표시(김정관)
        {
            List<GridData> list = new List<GridData>();
            list.Add(new GridData { modelId = "1" });
            list.Add(new GridData { modelId = "2" });
            list.Add(new GridData { modelId = "3" });
            list.Add(new GridData { modelId = "4" });
            list.Add(new GridData { modelId = "5" });

            DG_Name.ItemsSource = list;
        }

        public void Col1View() // 차트 생성 및 초기화, Datagrid에 연결
        {
            FacilityDataChartControl facilityDataChartControl = new FacilityDataChartControl();
            Dictionary<string, int> date = new StatisticsTabChartData().HindranceRateData();
            col1 = facilityDataChartControl.CreatePieChart(col1, date, "부품별 고장", true, true, true);
            col1.Refresh();
        }

        public void Col2View()
        {
            FacilityDataChartControl facilityDataChartControl = new FacilityDataChartControl();
            Dictionary<string, int> date = new StatisticsTabChartData().RecentErrorsData();
            col2 = facilityDataChartControl.CreateBarChart(col2, date, "최근 10일 고장 모델", true);
            col2.Refresh();
        }

        public void Col3View()
        {
            FacilityDataChartControl facilityDataChartControl = new FacilityDataChartControl();
            Dictionary<string, int> date = new StatisticsTabChartData().ErrorRateData();
            col3 = facilityDataChartControl.CreateBarChart(col3, date, "오류 횟수", true);
            col3.Refresh();
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
        private void UpdateVoltageGraph() // 전압 데이터 표시 차트 업데이트 메서드
        {
            FacilityDataChartControl facilityDataChartControl = new FacilityDataChartControl();
            List<ParseTelemetry_1> date = new FacilityDataControl().GetParseTelemetryData(); // 김정관 수정
            WP_Volt = facilityDataChartControl.CreateVoltageChart(WP_Volt, date, "VoltageGraph");
            WP_Volt.Refresh();
        }
        private void UpdateRotateGraph() // 회전 데이터 표시 차트 업데이트 메서드
        {
            FacilityDataChartControl facilityDataChartControl = new FacilityDataChartControl();
            List<ParseTelemetry_1> date = new FacilityDataControl().GetParseTelemetryData(); // 김정관 수정
            WP_Rotate = facilityDataChartControl.CreateRotateChart(WP_Rotate, date, "RotateGraph");
            WP_Rotate.Refresh();

        }
        private void UpdatePressureGraph() // 압력 데이터 표시 차트 업데이트 메서드
        {
            FacilityDataChartControl facilityDataChartControl = new FacilityDataChartControl();
            List<ParseTelemetry_1> date = new FacilityDataControl().GetParseTelemetryData(); // 김정관 수정
            WP_Pressure = facilityDataChartControl.CreatePressureChart(WP_Pressure, date, "PressureGraph");
            WP_Pressure.Refresh();

        }
        private void UpdateVibrationGraph() // 진동 데이터 표시 차트 업데이트 메서드
        {
            FacilityDataChartControl facilityDataChartControl = new FacilityDataChartControl();
            List<ParseTelemetry_1> date = new FacilityDataControl().GetParseTelemetryData(); // 김정관 수정
            WP_Vibration = facilityDataChartControl.CreateVibrationChart(WP_Vibration, date, "VibrationGraph");
            WP_Volt.Refresh();

        }
        // 김정관 끝

        private void AddRowButton_Click(object sender, RoutedEventArgs e)
        {
            // + 버튼 클릭 시 새로운 행 추가
            DG_checkData.Items.Add(new GridData());
        }

        private void RemoveRowButton_Click(object sender, RoutedEventArgs e)
        {
            if (DG_checkData.Items.Count > 0)
            {
                int lastIndex = DG_checkData.Items.Count - 1;
                DG_checkData.Items.RemoveAt(lastIndex);
            }
        }
    }


    public class GridData
    {
        public string modelId { get; set; }
        public string age { get; set; }
        public string errorID { get; set; }
        public string volt { get; set; }
        public string rotate { get; set; }
        public string pressure { get; set; }
        public string vibration { get; set; }
    }
}
