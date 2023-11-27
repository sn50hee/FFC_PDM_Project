using ScottPlot;
using ScottPlot.Drawing.Colormaps;
using ScottPlot.Plottable;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
//using static ScottPlot.Plottable.PopulationPlot;

namespace FFC_PDM
{
    internal class FacilityDataChartControl
    {
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
        public WpfPlot CreateCustomChart(WpfPlot chart, List<ParseTelemetry> chartData, string title)
        {
            Plot plt = chart.Plot;

            var machineIDsWithVolts = chartData.Select(d => new { MachineID = d.machineID, Volt = d.volt }).ToList();
            var machineIDs = machineIDsWithVolts.Select(d =>
            {
                if (double.TryParse(d.MachineID, out double result))
                {
                    return result;
                }
                else
                {
                    return 0.0;
                }
            })
            .Where(d => !double.IsNaN(d))
            .ToArray();

            var volts = machineIDsWithVolts.Select(d =>
            {
                if (double.TryParse(d.Volt.ToString(), out double result))
                {
                    return result;
                }
                else
                {
                    return 0.0;
                }
            }).ToArray();

            var machineIDsSubset = machineIDs.Take(101).ToArray();
            var voltsSubset = volts.Take(101).ToArray();

            var scatter = plt.AddScatter(machineIDsSubset, voltsSubset);
            scatter.MarkerSize = 5;

            plt.Title(title);
            //plt.XLabel("시간");
            //plt.YLabel("전압");

            return chart;
        }
        // 김정관 끝
    }


}


