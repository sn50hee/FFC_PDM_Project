using ScottPlot;
using ScottPlot.Drawing.Colormaps;
using ScottPlot.Plottable;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using static ScottPlot.Plottable.PopulationPlot;

namespace FFC_PDM
{
    internal class FacilityDataChartControl
    {
        StatisticsTabChartData statisticsTabChartData;

        public WpfPlot CreateBarChart(WpfPlot chart, Dictionary<string, int> chartData, string title, bool showValuesAboveBars)
        {
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

        // 김정관 추가 시작
        public WpfPlot CreateCustomChart(WpfPlot chart, List<Telemetry> telemetryData, string title)
        {
            Plot plt = chart.Plot;

            var voltageLine = plt.AddSignal(telemetryData.Select(data => data.volt).ToArray());
            voltageLine.Color = System.Drawing.Color.Blue;

            // Additional plots for other telemetry data (rotate, pressure, vibration) can be added here.

            plt.Title(title);
            plt.XLabel("Datetime");
            plt.YLabel("Voltage");

            return chart;
        }
        // 김정관 끝
    }

    // 김정관 추가 시작
    internal class ViewDetailsTabChartData : FacilityDataControl
    {
        public List<Telemetry> GetTelemetryData()
        {
            // Use the GetTelemetryData method from FacilityDataControl
            return base.GetTelemetryData();
        }

        internal List<Telemetry> GetTelemetryChartData()
        {
            throw new NotImplementedException();
        }
    }

}
    //김정관 끝


