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

namespace FFC_PDM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            GridDataView();
            Col1View();
            Col2View();
            Col3View();

            //김정관 추가
            UpdateVoltageGraph();
            //김정관 끝
        }

        public void GridDataView()
        {
            List<GridData> list = new List<GridData>();
            list.Add(new GridData { model_name = "1" });
            list.Add(new GridData { model_name = "2" });
            list.Add(new GridData { model_name = "3" });
            list.Add(new GridData { model_name = "4" });
            list.Add(new GridData { model_name = "5" });

            DG_Name.ItemsSource = list;
        }

        public void Col1View()
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
        private void UpdateVoltageGraph()
        {
            ViewDetailsTabChartData viewDetailsTabChartData = new ViewDetailsTabChartData();
            var voltageData = viewDetailsTabChartData.GetVoltageData();

            // 이제 voltageData를 사용하여 그래프 업데이트 등의 작업을 수행

            // 여기서는 간단히 예시로 Scatter 플롯을 그리도록 하겠습니다.
            col1.Plot.Clear();

            // 가정: voltageData의 각 요소는 DateTime, string, double 등의 속성을 가지는 클래스 또는 튜플
            var machineIDsWithVolts = voltageData.Item1.Select(d => new { MachineID = d.machineID, Volt = d.volt }).ToList();
            var machineIDs = machineIDsWithVolts.Select(d => d.MachineID).ToArray();

            // 수정된 부분: 데이터 형식을 올바르게 가져오는지 확인
            var volts = machineIDsWithVolts.Select(d => double.Parse(d.Volt)).ToArray();

            // 그래프에 데이터 추가
            var scatter = col1.Plot.AddScatter(machineIDs, volts);
            scatter.MarkerSize = 5;

            // 그래프 업데이트
            col1.Refresh();
        }
        // 김정관 끝
    }

    public class GridData
    {
        public string model_name { get; set; }
    }
}
