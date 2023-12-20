using ScottPlot.Plottable;
using ScottPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FFC_PDM
{
    internal class ViewDetailsTabChartDataControl
    {
        // 김정관 추가 시작
        // 기존 코드에서 double.TryParse로 변환할때 문자열 변환x
        // if문 제거 -> 데이터 필터링 제거함(101개) 대신 Take씀
        // FacilityDataControl클래스의 두 인스턴스 생성을 하나로 통합
        private ScatterPlot? voltScatter;
        private MarkerPlot? voltHighlightedPoint;
        private ScottPlot.Plottable.Tooltip? voltToolTip;
        private int LastHighlightedIndex = -1;
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
            //
            voltScatter = plt.AddScatter(datetimeValues, volts, color: ScottPlot.Palette.Frost.Colors[0]);
            //var scatter = plt.AddScatter(datetimeValues, volts, color: ColorTranslator.FromHtml("#84A7A1"));
            voltScatter.MarkerSize = 5;

            plt.XAxis.DateTimeFormat(true);

            plt.AxisAuto(); // 축 범위 자동 조정

            plt.Title(title);
            plt.XLabel("Date");
            plt.YLabel("Voltage");

            voltHighlightedPoint = chart.Plot.AddPoint(0, 0);
            voltHighlightedPoint.Color = System.Drawing.Color.Red;
            voltHighlightedPoint.MarkerSize = 10;
            voltHighlightedPoint.MarkerShape = ScottPlot.MarkerShape.openCircle;
            voltHighlightedPoint.IsVisible = false;

            return chart;
        }

        public WpfPlot Volt_MouseMove(WpfPlot plot, MouseEventArgs e)
        {
            if (voltScatter != null)
            {
                plot.Plot.Remove(voltToolTip);
                (double mouseCoordX, double mouseCoordY) = plot.GetMouseCoordinates();
                double xyRatio = plot.Plot.XAxis.Dims.PxPerUnit / plot.Plot.YAxis.Dims.PxPerUnit;
                (double pointX, double pointY, int pointIndex) = voltScatter.GetPointNearest(mouseCoordX, mouseCoordY, xyRatio);

                // place the highlight over the point of interest
                voltHighlightedPoint.X = pointX;
                voltHighlightedPoint.Y = pointY;
                voltHighlightedPoint.IsVisible = true;

                // render if the highlighted point chnaged
                if (LastHighlightedIndex != pointIndex)
                {
                    LastHighlightedIndex = pointIndex;
                }

                double mouseX = e.GetPosition(plot).X;
                double mouseY = e.GetPosition(plot).Y;
                voltToolTip = plot.Plot.AddTooltip(label: $"Volt: {pointY:N2}\r\nDate: {DateTime.FromOADate(pointX)}", x: pointX, y: pointY);

            }
            return plot;
        }
        public WpfPlot Volt_MouseLeave(WpfPlot plot, MouseEventArgs e)
        {
            if (voltScatter != null)
            {
                plot.Plot.Remove(voltToolTip);
                voltHighlightedPoint.IsVisible = false;
            }
            return plot;
        }

        private ScatterPlot? rotateScatter;
        private MarkerPlot? rotateHighlightedPoint;
        private ScottPlot.Plottable.Tooltip? rotateToolTip; 
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
            rotateScatter = plt.AddScatter(datetimeValues, rotates, color: ScottPlot.Palette.Frost.Colors[1]);
            //var scatter = plt.AddScatter(datetimeValues, volts, color: ColorTranslator.FromHtml("#2E8A99"));
            rotateScatter.MarkerSize = 5;

            plt.XAxis.DateTimeFormat(true);

            plt.AxisAuto(); // 축 범위 자동 조정

            plt.Title(title);
            plt.XLabel("Date");
            plt.YLabel("Rotate");

            rotateHighlightedPoint = chart.Plot.AddPoint(0, 0);
            rotateHighlightedPoint.Color = System.Drawing.Color.Red;
            rotateHighlightedPoint.MarkerSize = 10;
            rotateHighlightedPoint.MarkerShape = ScottPlot.MarkerShape.openCircle;
            rotateHighlightedPoint.IsVisible = false;

            return chart;
        }

        public WpfPlot Rotate_MouseMove(WpfPlot plot, MouseEventArgs e)
        {
            if (rotateScatter != null)
            {
                plot.Plot.Remove(rotateToolTip);
                (double mouseCoordX, double mouseCoordY) = plot.GetMouseCoordinates();
                double xyRatio = plot.Plot.XAxis.Dims.PxPerUnit / plot.Plot.YAxis.Dims.PxPerUnit;
                (double pointX, double pointY, int pointIndex) = rotateScatter.GetPointNearest(mouseCoordX, mouseCoordY, xyRatio);

                // place the highlight over the point of interest
                rotateHighlightedPoint.X = pointX;
                rotateHighlightedPoint.Y = pointY;
                rotateHighlightedPoint.IsVisible = true;

                // render if the highlighted point chnaged
                if (LastHighlightedIndex != pointIndex)
                {
                    LastHighlightedIndex = pointIndex;
                }

                double mouseX = e.GetPosition(plot).X;
                double mouseY = e.GetPosition(plot).Y;
                rotateToolTip = plot.Plot.AddTooltip(label: $"Rotate: {pointY:N2}\r\nDate: {DateTime.FromOADate(pointX)}", x: pointX, y: pointY);

            }
            return plot;
        }
        public WpfPlot Rotate_MouseLeave(WpfPlot plot, MouseEventArgs e)
        {
            if (rotateScatter != null)
            {
                plot.Plot.Remove(rotateToolTip);
                rotateHighlightedPoint.IsVisible = false;
            }
            return plot;
        }

        private ScatterPlot? pressureScatter;
        private MarkerPlot? pressureHighlightedPoint;
        private ScottPlot.Plottable.Tooltip? pressureToolTip;
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
            pressureScatter = plt.AddScatter(datetimeValues, pressures, color: ScottPlot.Palette.Frost.Colors[2]);
            //var scatter = plt.AddScatter(datetimeValues, volts, color: ColorTranslator.FromHtml("#1F6E8C"));
            pressureScatter.MarkerSize = 5;

            plt.XAxis.DateTimeFormat(true);

            plt.AxisAuto(); // 축 범위 자동 조정

            plt.Title(title);
            plt.XLabel("Date");
            plt.YLabel("Pressure");

            pressureHighlightedPoint = chart.Plot.AddPoint(0, 0);
            pressureHighlightedPoint.Color = System.Drawing.Color.Red;
            pressureHighlightedPoint.MarkerSize = 10;
            pressureHighlightedPoint.MarkerShape = ScottPlot.MarkerShape.openCircle;
            pressureHighlightedPoint.IsVisible = false;

            return chart;
        }

        public WpfPlot Pressure_MouseMove(WpfPlot plot, MouseEventArgs e)
        {
            if (pressureScatter != null)
            {
                plot.Plot.Remove(pressureToolTip);
                (double mouseCoordX, double mouseCoordY) = plot.GetMouseCoordinates();
                double xyRatio = plot.Plot.XAxis.Dims.PxPerUnit / plot.Plot.YAxis.Dims.PxPerUnit;
                (double pointX, double pointY, int pointIndex) = pressureScatter.GetPointNearest(mouseCoordX, mouseCoordY, xyRatio);

                // place the highlight over the point of interest
                pressureHighlightedPoint.X = pointX;
                pressureHighlightedPoint.Y = pointY;
                pressureHighlightedPoint.IsVisible = true;

                // render if the highlighted point chnaged
                if (LastHighlightedIndex != pointIndex)
                {
                    LastHighlightedIndex = pointIndex;
                }

                double mouseX = e.GetPosition(plot).X;
                double mouseY = e.GetPosition(plot).Y;
                pressureToolTip = plot.Plot.AddTooltip(label: $"Pressure: {pointY:N2}\r\nDate: {DateTime.FromOADate(pointX)}", x: pointX, y: pointY);

            }
            return plot;
        }

        public WpfPlot Pressure_MouseLeave(WpfPlot plot, MouseEventArgs e)
        {
            if (pressureScatter != null)
            {
                plot.Plot.Remove(pressureToolTip);
                pressureHighlightedPoint.IsVisible = false;
            }
            return plot;
        }

        private ScatterPlot? vibrationScatter;
        private MarkerPlot? vibrationHighlightedPoint;
        private ScottPlot.Plottable.Tooltip? vibrationToolTip;
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
            vibrationScatter = plt.AddScatter(datetimeValues, vibrations, color: ScottPlot.Palette.Frost.Colors[3]);
            //var scatter = plt.AddScatter(datetimeValues, volts, color: ColorTranslator.FromHtml("#0E2954"));
            vibrationScatter.MarkerSize = 5;

            plt.XAxis.DateTimeFormat(true);

            plt.AxisAuto(); // 축 범위 자동 조정

            plt.Title(title);
            plt.XLabel("Date");
            plt.YLabel("Vibration");

            vibrationHighlightedPoint = chart.Plot.AddPoint(0, 0);
            vibrationHighlightedPoint.Color = System.Drawing.Color.Red;
            vibrationHighlightedPoint.MarkerSize = 10;
            vibrationHighlightedPoint.MarkerShape = ScottPlot.MarkerShape.openCircle;
            vibrationHighlightedPoint.IsVisible = false;

            return chart;
        }

        public WpfPlot Vibration_MouseMove(WpfPlot plot, MouseEventArgs e)
        {
            if (vibrationScatter != null)
            {
                plot.Plot.Remove(vibrationToolTip);
                (double mouseCoordX, double mouseCoordY) = plot.GetMouseCoordinates();
                double xyRatio = plot.Plot.XAxis.Dims.PxPerUnit / plot.Plot.YAxis.Dims.PxPerUnit;
                (double pointX, double pointY, int pointIndex) = vibrationScatter.GetPointNearest(mouseCoordX, mouseCoordY, xyRatio);

                // place the highlight over the point of interest
                vibrationHighlightedPoint.X = pointX;
                vibrationHighlightedPoint.Y = pointY;
                vibrationHighlightedPoint.IsVisible = true;

                // render if the highlighted point chnaged
                if (LastHighlightedIndex != pointIndex)
                {
                    LastHighlightedIndex = pointIndex;
                }

                double mouseX = e.GetPosition(plot).X;
                double mouseY = e.GetPosition(plot).Y;
                vibrationToolTip = plot.Plot.AddTooltip(label: $"Vibration: {pointY:N2}\r\nDate: {DateTime.FromOADate(pointX)}", x: pointX, y: pointY);

            }
            return plot;
        }
        public WpfPlot Vibration_MouseLeave(WpfPlot plot, MouseEventArgs e)
        {
            if (vibrationScatter != null)
            {
                plot.Plot.Remove(vibrationToolTip);
                vibrationHighlightedPoint.IsVisible = false;
            }
            return plot;
        }
        public WpfPlot CreateWarningPieChart(WpfPlot chart, Dictionary<string, int> chartData)
        {
            string[] keys = chartData.Keys.OrderBy(key => key).ToArray();
            double[] valuesAsDouble = keys.OrderBy(key => key).Select(key => (double)chartData[key]).ToArray();

            Plot plt = chart.Plot;
            plt.Palette = ScottPlot.Palette.Frost;

            var pie = plt.AddPie(valuesAsDouble);

            pie.SliceLabels = keys;
            pie.ShowPercentages = true;
            pie.ShowValues = true;
            pie.ShowLabels = true;
            plt.Legend();
            chart.Configuration.ScrollWheelZoom = false;

            return chart;
        }


        public WpfPlot CreateMaintPieChart(WpfPlot chart, Dictionary<string, int> chartData)
        {
            string[] keys = chartData.Keys.OrderBy(key => key).ToArray();
            double[] valuesAsDouble = keys.OrderBy(key => key).Select(key => (double)chartData[key]).ToArray();

            Plot plt = chart.Plot;
            plt.Palette = ScottPlot.Palette.Frost;

            var pie = plt.AddPie(valuesAsDouble);

            pie.SliceLabels = keys;
            pie.ShowPercentages = true;
            pie.ShowValues = true;
            pie.ShowLabels = true;
            plt.Legend();
            chart.Configuration.ScrollWheelZoom = false;

            return chart;
        }

        // 김정관 끝
    }
}
