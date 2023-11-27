using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FFC_PDM
{
    class ViewDetailsTabChartData : FacilityDataControl
    {

        public List<ParseTelemetry> GetVoltageData()
        {
            List<Telemetry> getTelemetryData = GetTelemetryData();
            List<ParseTelemetry> parseTelemetryData = new List<ParseTelemetry> ();

            foreach (Telemetry telemetry in getTelemetryData)
            {
                ParseTelemetry parseTelemetry = new ParseTelemetry
                {
                    //datetime = DateTime.Parse(telemetry.datetime),
                    //volt = double.Parse(telemetry.volt),
                    machineID = telemetry.machineID
                };
                parseTelemetryData.Add(parseTelemetry);
            }

            // 추가: Voltage를 기반으로 플롯 데이터 생성
            Dictionary<string, double[]> chartData = parseTelemetryData
                .GroupBy(data => data.machineID)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(data => data.volt).ToArray()
                );

            return parseTelemetryData;
        }

    }

    public class ParseTelemetry
    {
        public DateTime datetime { get; set; }
        public string machineID { get; set; }
        public double volt { get; set; }
    }
}
