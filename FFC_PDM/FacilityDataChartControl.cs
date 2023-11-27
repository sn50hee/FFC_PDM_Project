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
        public WpfPlot CreateCustomChart(WpfPlot chart, (List<(string machineID, double voltage)> voltageData, Dictionary<string, double[]> chartData) data, string title)
        {
            Plot plt = chart.Plot;

            // 예: Voltage에 대한 라인 플롯
            var voltageLine = plt.AddSignal(data.chartData["Values"]);
            voltageLine.Color = System.Drawing.Color.Blue;

            // 추가: 다른 차트 데이터에 대한 플롯 생성
            // (이 예제에서는 생략)

            plt.Title(title);
            plt.XLabel("시간");
            plt.YLabel("전압");

            return chart;
        }
        // 김정관 끝
    }

    // 김정관 추가 시작
    internal class ViewDetailsTabChartData : FacilityDataControl
    {
        public (List<(DateTime datetime, string machineID, double volt)>, Dictionary<string, double[]>) GetVoltageData()
        {
            // CSV 파일에서 데이터 읽기
            var csvData = File.ReadAllLines("Resource/PdM_telemetry.csv")
                .Skip(1) // 헤더를 건너뜁니다.
                .Select(line => line.Split(','))
                .Select(parts => (
                    datetime: DateTime.Parse(parts[0].Trim('"')),
                    machineID: parts[1].Trim('"'),
                    volt: double.Parse(parts[2])
                ))
                .ToList();

            // 추가: Voltage를 기반으로 플롯 데이터 생성
            Dictionary<string, double[]> chartData = new Dictionary<string, double[]>
            {
                { "Values", csvData.Select(data => data.volt).ToArray() },
                // 추가: 기타 차트 데이터도 필요에 따라 추가
            };

            return (csvData, chartData);
        }

    }

}
    //김정관 끝


