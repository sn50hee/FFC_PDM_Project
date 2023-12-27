using MaterialDesignThemes.Wpf;
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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static ScottPlot.Plottable.PopulationPlot;
//using static ScottPlot.Plottable.PopulationPlot;

namespace FFC_PDM
{
    internal class FacilityDataChartControl

    {
        public WpfPlot CreateBarChart(WpfPlot chart, Dictionary<string, int> chartData, string title, bool showValuesAboveBars, bool sortFlag)
        {
            string[] keys;
            double[] valuesAsDouble;
            double[] positions;

            if (sortFlag)
            {
                keys = chartData.Keys.OrderBy(key => key).ToArray();
                valuesAsDouble = keys.OrderBy(key => key).Select(key => (double)chartData[key]).ToArray();
                positions = Enumerable.Range(0, keys.Length).Select(index => (double)index).ToArray();
            }
            else
            {
                keys = chartData.Keys.ToArray();
                valuesAsDouble = keys.Select(key => (double)chartData[key]).ToArray();
                positions = Enumerable.Range(0, keys.Length).Select(index => (double)index).ToArray();
            }

            Plot plt = chart.Plot;
            List<ScottPlot.Plottable.Bar> bars = new();
            for (int i = 0; i < valuesAsDouble.Length; i++)
            {
                if (valuesAsDouble[i]== valuesAsDouble.Max())
                {
                    ScottPlot.Plottable.Bar bar = new()
                    {
                        Value = valuesAsDouble[i],
                        Position = positions[i],
                        FillColor = System.Drawing.Color.FromArgb(148, 213, 240)
                    };
                    bars.Add(bar);
                    var txt = plt.AddText(valuesAsDouble[i].ToString(), i, valuesAsDouble[i]);
                    txt.Color = System.Drawing.Color.FromArgb(148, 213, 240);
                    txt.Font.Alignment = Alignment.LowerCenter;
                    txt.Font.Size = 16;
                    txt.Font.Bold = true;
                }
                else
                {
                    ScottPlot.Plottable.Bar bar = new()
                    {
                        Value = valuesAsDouble[i],
                        Position = positions[i],
                        FillColor = System.Drawing.Color.FromArgb(204, 204, 204)
                    };
                    bars.Add(bar);

                    var txt = plt.AddText(valuesAsDouble[i].ToString(), i, valuesAsDouble[i]);
                    txt.Color = System.Drawing.Color.FromArgb(204, 204, 204);
                    txt.Font.Alignment = Alignment.LowerCenter;
                    txt.Font.Size = 16;
                    txt.Font.Bold = true;
                }
            }

            plt.AddBarSeries(bars);
            plt.SetAxisLimitsY(valuesAsDouble.Min(), valuesAsDouble.Max()*1.2);

            plt.XTicks(positions, keys);
            plt.SetAxisLimits(yMin: 0);
            plt.Grid(false);
            plt.YAxis.Ticks(false);
            plt.XAxis.TickLabelStyle(fontSize: 15);

            plt.YAxis2.Line(false);
            plt.XAxis2.Line(false);


            plt.Legend();
            // plt.Title(title);

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
                markerSize: 2,
                color: System.Drawing.Color.FromArgb(255, 148, 213, 240)
            );
            wpfPlot.Plot.AddFill(chartData.Select(item => item.Item1.ToOADate()).ToArray(),
                chartData.Select(item => item.Item2).ToArray(), color: System.Drawing.Color.FromArgb(180, 148, 213, 240));
            //wpfPlot.Plot.Title("장비 가동률");
            wpfPlot.Plot.XAxis.DateTimeFormat(true);
            wpfPlot.Plot.YAxis.SetBoundary(95, 100);
            wpfPlot.Plot.YLabel("단위: %");
            wpfPlot.Plot.Grid(enable: true);

            return wpfPlot;
        }
        
    }
}