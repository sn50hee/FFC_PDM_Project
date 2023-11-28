using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FFC_PDM
{
    class StatisticsTabChartData: FacilityDataControl
    {

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

            var recentData = getErrorsData.OrderByDescending(e => DateTime.Parse(e.datetime)).Take(10);

            Dictionary<string, int> errorIDCountDictionary = recentData.GroupBy(e => e.machineID)
                                                                .ToDictionary(group => group.Key, group => group.Count());

            return errorIDCountDictionary;
        }

        public List<StatisticsTabGridData> GetFailuressListViewData()
        {

            List<StatisticsTabGridData> dataList = GetLatestTelemetryData();
            List<string> inputDataList = new List<string>();


            foreach (StatisticsTabGridData data in dataList)
            {
                inputDataList.Add($"[[{data.volt.ToString()},{data.rotate.ToString()},{data.pressure.ToString()},{data.vibration.ToString()},0,{data.modelId.ToString()},{data.age.ToString()}]]");
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

    }
}
