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
        FacilityDataChartControl facilityDataChartControl;
        public MainWindow()
        {
            InitializeComponent();
            GridDataView();
            Col1View();
            Col2View();
            Col3View();

            InitDetailsTab();
            facilityDataChartControl = new FacilityDataChartControl();
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

        public void LoadTelemetryChart()
        {
            FacilityDataChartControl facilityDataChartControl = new FacilityDataChartControl();
            var telemetryChartData = new ViewDetailsTabChartData().GetTelemetryChartData();
            col1 = facilityDataChartControl.CreateCustomChart(col1, telemetryChartData, "Voltage Chart");
            col1.Refresh();
        }

        private void InitDetailsTab()
        {
            // 백그라운드 스레드에서 데이터 가져오기
            Task.Run(() =>
            {
                var modelNames = GetModelNames();
                var modelIDs = GetModelIDs();

                // UI 스레드에서 업데이트
                Dispatcher.Invoke(() =>
                {
                    // 모델명 ComboBox 초기화
                    ModelNameComboBox.ItemsSource = modelNames;
                    // 모델 ID ComboBox 초기화
                    ModelIDComboBox.ItemsSource = modelIDs;
                });
            });
        }

        private List<string> GetModelNames()
        {
            var telemetryData = new FacilityDataControl().GetTelemetryData();
            return telemetryData.Select(t => t.machineID).Distinct().ToList();
        }

        private List<string> GetModelIDs()
        {
            var telemetryData = new FacilityDataControl().GetTelemetryData();
            return telemetryData.Select(t => t.machineID).Distinct().ToList();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            // 선택된 모델 ID 가져오기
            string selectedModelID = ModelIDComboBox.SelectedItem as string;

            // 선택된 시작일과 종료일 가져오기
            DateTime startDate = DatePickerStart.SelectedDate ?? DateTime.MinValue;
            DateTime endDate = DatePickerEnd.SelectedDate ?? DateTime.MaxValue;

            // Telemetry 데이터 가져오기
            List<Telemetry> telemetryData = GetTelemetryData(selectedModelID, startDate, endDate);

            // 차트 업데이트
            UpdateCharts(telemetryData);
        }

        private List<Telemetry> GetTelemetryData(string modelID, DateTime startDate, DateTime endDate)
        {
            // 모든 Telemetry 데이터 가져오기
            List<Telemetry> allTelemetryData = new FacilityDataControl().GetTelemetryData();

            // 선택된 모델 ID로 필터링
            List<Telemetry> filteredData = allTelemetryData
                .Where(data => data.machineID == modelID && data.datetime >= startDate && data.datetime <= endDate)
                .ToList();

            return filteredData;
        }

        private void UpdateCharts(List<Telemetry> telemetryData)
        {
            // VoltagePlot 차트 업데이트
            facilityDataChartControl.CreateCustomChart(VoltagePlot, telemetryData, "Voltage Chart");

            // 추가적으로 필요한 차트 업데이트를 여기에 추가할 수 있습니다.
        }

    }

    public class GridData
    {
        public string model_name { get; set; }
    }
}
