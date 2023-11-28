using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public Dictionary<string, int> RecentErrorsData()
        {

            DateTime referenceDate = new DateTime(2015, 12, 31, 23, 59, 59);

            List<Errors> getErrorsData = GetErrorsData();

            DateTime startDate = referenceDate.AddDays(-10);

            getErrorsData = getErrorsData
                .Where(error => DateTime.Parse(error.datetime) >= startDate && DateTime.Parse(error.datetime) <= referenceDate)
                .ToList();

            List<string> errorIDDataOnly = getErrorsData.Select(machineID => machineID.machineID).ToList();
            Dictionary<string, int> errorIDCountDictionary = errorIDDataOnly
                .GroupBy(machineID => machineID)
                .OrderByDescending(group => group.Count())
                .ToDictionary(group => group.Key, group => group.Count());

            return errorIDCountDictionary;
        }

    }
}
