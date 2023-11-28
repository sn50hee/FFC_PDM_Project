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
            GridDataView();
            Col1View();
            Col2View();
            Col3View();

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
            list.Add(new GridData { model_name = "1" });
            list.Add(new GridData { model_name = "2" });
            list.Add(new GridData { model_name = "3" });
            list.Add(new GridData { model_name = "4" });
            list.Add(new GridData { model_name = "5" });

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
            Dictionary<string, int>  date = new StatisticsTabChartData().ErrorRateData();
            col3 = facilityDataChartControl.CreateBarChart(col3, date, "오류 횟수", true);
            col3.Refresh();
        }


        // 김정관 추가
        private void UpdateVoltageGraph() // 전압 데이터 표시 차트 업데이트 메서드
        {
            FacilityDataChartControl facilityDataChartControl = new FacilityDataChartControl();
            List<ParseTelemetry_1> date = new FacilityDataControl().GetParseTelemetryData(); // 김정관 수정
            WP_Volt = facilityDataChartControl.CreateVoltageChart(WP_Volt, date, "VoltageGraph");
            WP_Volt.Refresh();

            // WP_Volt

            // UI 업데이트를 Dispatcher에서 실행
            //Dispatcher.Invoke(() =>
            //{
            //    WP_Volt.Plot.Clear();

            //    var machineIDsWithVolts = voltageData.Item1.Select(d => new { MachineID = d.machineID, Volt = d.volt }).ToList();
            //    var machineIDs = machineIDsWithVolts.Select(d =>
            //    {
            //        if (double.TryParse(d.MachineID, out double result))
            //        {
            //            return result;
            //        }
            //        else
            //        {
            //            return 0.0;
            //        }
            //    })
            //    .Where(d => !double.IsNaN(d))
            //    .ToArray();

            //    var volts = machineIDsWithVolts.Select(d =>
            //    {
            //        if (double.TryParse(d.Volt.ToString(), out double result))
            //        {
            //            return result;
            //        }
            //        else
            //        {
            //            return 0.0;
            //        }
            //    }).ToArray();

            //    var scatter = WP_Volt.Plot.AddScatter(machineIDs, volts);
            //    scatter.MarkerSize = 5;

            //     그래프 업데이트
            //    WP_Volt.Refresh();
            //});
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
    }


    public class GridData
    {
        public string model_name { get; set; }
    }
}
