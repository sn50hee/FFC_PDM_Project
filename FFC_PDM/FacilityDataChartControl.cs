using ScottPlot;
using ScottPlot.Drawing.Colormaps;
using ScottPlot.Plottable;
using System;
using System.Collections.Generic;
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
            var bar = plt.AddBar(valuesAsDouble, positions);

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
                markerSize: 5
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
        public WpfPlot CreateVoltageChart(WpfPlot chart, List<ParseTelemetry_1> chartData, string title)
        {
            Plot plt = chart.Plot;
            plt.Clear();

            var dataSubset = chartData
                .Where(d => !double.IsNaN(d.machineID) && !double.IsNaN(d.volt))
                .Take(100)  // 데이터 양 조절
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

        public WpfPlot CreateRotateChart(WpfPlot chart, List<ParseTelemetry_1> chartData, string title)
        {
            Plot plt = chart.Plot;

            var dataSubset = chartData
                .Where(d => !double.IsNaN(d.machineID) && !double.IsNaN(d.rotate))
                .Take(100)  // 데이터 양 조절
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

        public WpfPlot CreatePressureChart(WpfPlot chart, List<ParseTelemetry_1> chartData, string title)
        {
            Plot plt = chart.Plot;

            var dataSubset = chartData
                .Where(d => !double.IsNaN(d.machineID) && !double.IsNaN(d.pressure))
                .Take(100)  // 데이터 양 조절
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

        public WpfPlot CreateVibrationChart(WpfPlot chart, List<ParseTelemetry_1> chartData, string title)
        {
            Plot plt = chart.Plot;

            var dataSubset = chartData
                .Where(d => !double.IsNaN(d.machineID) && !double.IsNaN(d.vibration))
                .Take(20000)  // 데이터 양 조절
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

        public WpfPlot CreateWarningChart(WpfPlot chart, Dictionary<string, int> chartData, string title, bool showPercentages, bool showValues, bool showLabels)
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
        // 김정관 끝
    }



    //김정관 시작
    //internal static class CsvDataProcessor //중복제거 class
    //{
    //    public static List<ParseTelemetry> RemoveDuplicateVolt(List<ParseTelemetry> telemetryList) //중복제거 함수
    //    {
    //        // 중복 제거를 위한 Dictionary
    //        Dictionary<string, ParseTelemetry> uniqueTelemetryDict = new Dictionary<string, ParseTelemetry>();

    //        foreach (var telemetry in telemetryList)
    //        {
    //            // volt를 기준으로 중복 검사
    //            string voltKey = telemetry.volt.ToString();
    //            if (!uniqueTelemetryDict.ContainsKey(voltKey))
    //            {
    //                uniqueTelemetryDict.Add(voltKey, telemetry);
    //            }
    //        }

    //        // 중복이 제거된 Telemetry 리스트 반환
    //        return uniqueTelemetryDict.Values.ToList();
    //    }
    //}
    //김정관 끝


}


