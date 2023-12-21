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
            var bar = plt.AddBar(valuesAsDouble, positions, color: System.Drawing.Color.FromArgb(150, 15, 163, 177));
            // var bar = plt.AddBar(valuesAsDouble, positions, color: ColorTranslator.FromHtml("#0fa3b1"));

            bar.BorderLineWidth = 1;
            bar.BorderColor = System.Drawing.Color.FromArgb(200, 15, 163, 177);

            bar.ShowValuesAboveBars = showValuesAboveBars; 
            plt.XTicks(positions, keys);
            plt.SetAxisLimits(yMin: 0);
            plt.Grid(false);
            plt.YAxis.Ticks(false);

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
                color: ColorTranslator.FromHtml("#0fa3b1")
            );
            wpfPlot.Plot.AddFill(chartData.Select(item => item.Item1.ToOADate()).ToArray(),
                chartData.Select(item => item.Item2).ToArray(), color: System.Drawing.Color.FromArgb(100,15,163,177));
            wpfPlot.Plot.Title("장비 가동률");
            wpfPlot.Plot.XAxis.DateTimeFormat(true);
            wpfPlot.Plot.YAxis.SetBoundary(95, 100);
            wpfPlot.Plot.Grid(enable: true);

            return wpfPlot;
        }
        
    }
}