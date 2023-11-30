using ScottPlot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using static ScottPlot.Generate;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FFC_PDM
{
    class StatisticsTabChartData: FacilityDataControl
    {
        private int plotCurrentIndex;
        private int gridCurrentIndex;
        public StatisticsTabChartData() {
            plotCurrentIndex = 1;
            gridCurrentIndex = 0;
        }

        public Dictionary<string, int> HindranceRateData()
        {
            List<Failures> getFailuresData = GetFailuresData();
            List<string> failureDataOnly = getFailuresData.Select(failure => failure.failure).ToList();
            Dictionary<string, int> failureCountDictionary = failureDataOnly
                .GroupBy(failure => failure)
                .ToDictionary(group => group.Key, group => group.Count());

            return failureCountDictionary;
        }

        public Dictionary<string, int> ErrorRateData()
        {
            List<Errors> getErrorsData = GetErrorsData();
            List<string> errorIDDataOnly = getErrorsData.Select(errorID => errorID.errorID).ToList();
            Dictionary<string, int> errorIDCountDictionary = errorIDDataOnly
                .GroupBy(errorID => errorID)
                .ToDictionary(group => group.Key, group => group.Count());

            return errorIDCountDictionary;
        }

        public Dictionary<string, int> RecentFacilityData()
        {

            List<Errors> getErrorsData = GetErrorsData();

            var recentData = getErrorsData.OrderByDescending(e => System.DateTime.Parse(e.datetime)).Take(10);

            Dictionary<string, int> errorIDCountDictionary = recentData.GroupBy(e => e.machineID)
                                                                .ToDictionary(group => group.Key, group => group.Count());

            return errorIDCountDictionary;
        }

        public List<StatisticsTabGridData> GetFailuressListConnectModelViewData()
        {

            List<StatisticsTabGridData> dataList = GetLatestTelemetryData();
            List<string> inputDataList = new List<string>();

            // machineID	age	volt	rotate	pressure	vibration
            foreach (StatisticsTabGridData data in dataList)
            {
                inputDataList.Add($"[[{data.modelId.ToString()},{data.age.ToString()},{data.volt.ToString()},{data.rotate.ToString()},{data.pressure.ToString()},{data.vibration.ToString()}]]");
            }

            GetPythonModel getPythonModel = new GetPythonModel();
            List<string> checkList = getPythonModel.FailureCheck(inputDataList);

            List <StatisticsTabGridData> result = new List<StatisticsTabGridData >();

            for (int i = 0; i < checkList.Count; i++)
            {
                if (checkList[i] == "[1]\r\n")
                {
                    result.Add(dataList[i]);
                }
            }

            return result;
        }

        private List<ParseTelemetry_1> getFailuressListViewData;

        public List<(System.DateTime, double)> TelemetryDataListToDict()
        {
            getFailuressListViewData = GetParseTelemetryData();

            List<(System.DateTime, double)> resultList = getFailuressListViewData
                .GroupBy(data => data.datetime)
                .Select(group => (group.Key, (double)group.Count()))
                .OrderBy(item => item.Key)
                .ToList();

            return resultList;
        }

        public WpfPlot GetPointsToPlot(WpfPlot wpfPlot, List<(System.DateTime, double)> chartData,int count)
        {
            // 데이터를 count만큼 가져와서 반환
            List<(System.DateTime, double)> points = chartData.Take(count*plotCurrentIndex).ToList();
            plotCurrentIndex++;

            if (plotCurrentIndex >= chartData.Count)
            {
                // 데이터를 모두 사용한 경우 초기화
                plotCurrentIndex = 0;
            }

            FacilityDataChartControl chart = new FacilityDataChartControl();
            wpfPlot = chart.CreatePlottingDateTimeChart(wpfPlot,points);

            return wpfPlot;
        }

        public StatisticsTabGridData RiskOfFailuressDataPlot(DataGrid dataGrid, List<StatisticsTabGridData> chartData, int count)
        {
            StatisticsTabGridData data = chartData[count * gridCurrentIndex];
            gridCurrentIndex++;

            if (gridCurrentIndex >= chartData.Count)
            {
                // 데이터를 모두 사용한 경우 초기화
                gridCurrentIndex = 0;
            }

            return data;
        }
        

        public List<StatisticsTabGridData> GetFailuressListViewData()
        {
            List<StatisticsTabGridData> data = GetLatestTelemetryData();
            return data;
        }
        public List<StatisticsTabGridData> GetFailuressListViewData(double volt, double rotate, double pressure, double vibration)
        {
            List<StatisticsTabGridData> data = GetLatestTelemetryData(volt, rotate, pressure, vibration);
            return data;
        }

    }
}
