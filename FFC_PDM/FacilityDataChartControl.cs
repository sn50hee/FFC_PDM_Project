using ScottPlot;
using ScottPlot.Drawing.Colormaps;
using ScottPlot.Plottable;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
//using static ScottPlot.Plottable.PopulationPlot;

namespace FFC_PDM
{
    internal class FacilityDataChartControl
    // 김정관
    // 클래스 내에서 세 가지 메서드에서 각각 막대차트, 원형차트, 사용자 정의차트 생성 및 옵션 설정해 데이터 시각화
    {
        public WpfPlot CreateBarChart(WpfPlot chart, Dictionary<string, int> chartData, string title, bool showValuesAboveBars)
        {
            //OrderBy -> 정렬(Sort)같은애
            string[] keys = chartData.Keys.OrderBy(key => key).ToArray();
            double[] valuesAsDouble = keys.OrderBy(key => key).Select(key => (double)chartData[key]).ToArray();
            double[] positions = Enumerable.Range(0, keys.Length).Select(index => (double)index).ToArray();

            Plot plt = chart.Plot;
            var bar = plt.AddBar(valuesAsDouble, positions, color: ColorTranslator.FromHtml("#0fa3b1"));

            bar.ShowValuesAboveBars = showValuesAboveBars;
            plt.XTicks(positions, keys);
            plt.SetAxisLimits(yMin: 0);

            plt.Legend();
            plt.Title(title);

            return chart;
        }

        public WpfPlot CreatePieChart(WpfPlot chart, Dictionary<string, int> chartData, string title, bool showPercentages, bool showValues, bool showLabels)
        {
            string[] keys = chartData.Keys.OrderBy(key => key).ToArray();
            double[] valuesAsDouble = keys.OrderBy(key => key).Select(key => (double)chartData[key]).ToArray();

            Plot plt = chart.Plot;
            var pie = plt.AddPie(valuesAsDouble);

            pie.SliceLabels = keys;
            pie.ShowPercentages = showPercentages;
            pie.ShowValues = showValues;
            pie.ShowLabels = showLabels;
            plt.Legend();
            plt.Title(title);

            return chart;
        }

        public WpfPlot CreatePlottingDateTimeChart(WpfPlot wpfPlot, List<(System.DateTime, double)> chartData)
        {
            List<DateTime> dateTimeList = chartData.Select(item => item.Item1).ToList();
            List<double> doubleList = chartData.Select(item => item.Item2).ToList();

            wpfPlot.Plot.PlotScatter(
                chartData.Select(item => item.Item1.ToOADate()).ToArray(),
                chartData.Select(item => item.Item2).ToArray(),
                markerSize: 5,
                color: ColorTranslator.FromHtml("#0fa3b1")
            );
            wpfPlot.Plot.Title("장비 가동률");
            wpfPlot.Plot.XAxis.DateTimeFormat(true);
            wpfPlot.Plot.YAxis.SetBoundary(95, 100);
            wpfPlot.Plot.Grid(enable: true);

            return wpfPlot;
        }



        // 김정관 추가 시작
        // 기존 코드에서 double.TryParse로 변환할때 문자열 변환x
        // if문 제거 -> 데이터 필터링 제거함(101개) 대신 Take씀
        // FacilityDataControl클래스의 두 인스턴스 생성을 하나로 통합
        public WpfPlot CreateVoltageChart(WpfPlot chart, List<ParseTelemetry_1> chartData, string title, int selectedModelID, DateTime startDate, DateTime endDate)
        {
            Plot plt = chart.Plot;
            plt.Clear();

            var dataSubset = chartData
               .Where(d => !double.IsNaN(d.machineID) && !double.IsNaN(d.volt) &&
                   (selectedModelID == 0 || d.machineID == selectedModelID) &&
                   (d.datetime >= startDate && d.datetime <= endDate))  // 데이터 양 조절
                .OrderBy(d => d.datetime)
                .ToList();

            var datetimeValues = dataSubset.Select(d => d.datetime.ToOADate()).ToArray();
            var volts = dataSubset.Select(d => d.volt).ToArray();

            var scatter = plt.AddScatter(datetimeValues, volts);
            scatter.MarkerSize = 5;

            plt.XAxis.DateTimeFormat(true);

            plt.AxisAuto(); // 축 범위 자동 조정

            plt.Title(title);
            plt.XLabel("Date");
            plt.YLabel("Voltage");

            return chart;
        }

        public WpfPlot CreateRotateChart(WpfPlot chart, List<ParseTelemetry_1> chartData, string title, int selectedModelID, DateTime startDate, DateTime endDate)
        {
            Plot plt = chart.Plot;

            var dataSubset = chartData
               .Where(d => !double.IsNaN(d.machineID) && !double.IsNaN(d.rotate) &&
                   (selectedModelID == 0 || d.machineID == selectedModelID) &&
                   (d.datetime >= startDate && d.datetime <= endDate))
                .OrderBy(d => d.datetime)
               .ToList();

            var datetimeValues = dataSubset.Select(d => d.datetime.ToOADate()).ToArray();
            var rotates = dataSubset.Select(d => d.rotate).ToArray();

            var scatter = plt.AddScatter(datetimeValues, rotates);
            scatter.MarkerSize = 5;

            plt.XAxis.DateTimeFormat(true);

            plt.AxisAuto(); // 축 범위 자동 조정

            plt.Title(title);
            plt.XLabel("Date");
            plt.YLabel("Rotate");

            return chart;
        }

        public WpfPlot CreatePressureChart(WpfPlot chart, List<ParseTelemetry_1> chartData, string title, int selectedModelID, DateTime startDate, DateTime endDate)
        {
            Plot plt = chart.Plot;

            var dataSubset = chartData
               .Where(d => !double.IsNaN(d.machineID) && !double.IsNaN(d.pressure) &&
                   (selectedModelID == 0 || d.machineID == selectedModelID) &&
                   (d.datetime >= startDate && d.datetime <= endDate))
               .OrderBy(d => d.datetime)
               .ToList();

            var datetimeValues = dataSubset.Select(d => d.datetime.ToOADate()).ToArray();
            var pressures = dataSubset.Select(d => d.pressure).ToArray();

            var scatter = plt.AddScatter(datetimeValues, pressures);
            scatter.MarkerSize = 5;

            plt.XAxis.DateTimeFormat(true);

            plt.AxisAuto(); // 축 범위 자동 조정

            plt.Title(title);
            plt.XLabel("Date");
            plt.YLabel("Pressure");

            return chart;
        }

        public WpfPlot CreateVibrationChart(WpfPlot chart, List<ParseTelemetry_1> chartData, string title, int selectedModelID, DateTime startDate, DateTime endDate)
        {
            Plot plt = chart.Plot;

            var dataSubset = chartData
               .Where(d => !double.IsNaN(d.machineID) && !double.IsNaN(d.vibration) &&
                   (selectedModelID == 0 || d.machineID == selectedModelID) &&
                   (d.datetime >= startDate && d.datetime <= endDate))
               .OrderBy(d => d.datetime)
               .ToList();

            var datetimeValues = dataSubset.Select(d => d.datetime.ToOADate()).ToArray();
            var vibrations = dataSubset.Select(d => d.vibration).ToArray();

            var scatter = plt.AddScatter(datetimeValues, vibrations);
            scatter.MarkerSize = 5;

            plt.XAxis.DateTimeFormat(true);

            plt.AxisAuto(); // 축 범위 자동 조정

            plt.Title(title);
            plt.XLabel("Date");
            plt.YLabel("Vibration");

            return chart;
        }

        public WpfPlot CreateWarningPieChart(WpfPlot chart, Dictionary<string, int> chartData, string title)
        {
            string[] keys = chartData.Keys.OrderBy(key => key).ToArray();
            double[] valuesAsDouble = keys.OrderBy(key => key).Select(key => (double)chartData[key]).ToArray();

            Plot plt = chart.Plot;
            var pie = plt.AddPie(valuesAsDouble);

            pie.SliceLabels = keys;
            pie.ShowPercentages = true;
            pie.ShowValues = true;
            pie.ShowLabels = true;
            plt.Legend();
            plt.Title(title);

            return chart;
        }

        public WpfPlot CreateMaintPieChart(WpfPlot chart, Dictionary<string, int> chartData, string title)
        {
            string[] keys = chartData.Keys.OrderBy(key => key).ToArray();
            double[] valuesAsDouble = keys.OrderBy(key => key).Select(key => (double)chartData[key]).ToArray();

            Plot plt = chart.Plot;
            var pie = plt.AddPie(valuesAsDouble);

            pie.SliceLabels = keys;
            pie.ShowPercentages = true;
            pie.ShowValues = true;
            pie.ShowLabels = true;
            plt.Legend();
            plt.Title(title);

            return chart;
        }

        // 김정관 끝
    }
}