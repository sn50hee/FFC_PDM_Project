using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

// 데이터 가공하는 코드
namespace FFC_PDM
{
    class ViewDetailsTabChartData : FacilityDataControl
    {
        public List<ParseTelemetry> GetVoltageData() //class의 인스턴스를 담고있는 List
        {
            List<Telemetry> getTelemetryData = GetTelemetryData(); // telemetry 데이터 받아오기
            List<ParseTelemetry> parseTelemetryData = new List<ParseTelemetry> (); // parsing된 텔레메트리 데이터를 저장할 리스트 초기화

            // HashSet을 사용해서 중복제거
            HashSet<ParseTelemetry> uniqueTelemetryData = new HashSet<ParseTelemetry>();

            // 각 텔레메트리 데이터 순회, ParseTelemetry객체로 변환해 HashSet추가
            foreach (Telemetry telemetry in getTelemetryData)
            {
                ParseTelemetry parseTelemetry = new ParseTelemetry // Telemetry 데이터를 ParseTelemetry객체로 변환하기
                {
                    datetime = DateTime.Parse(telemetry.datetime), // Telemetry의 날짜 및 시간을 Parsing 및 datetime에 할당
                    volt = double.Parse(telemetry.volt),
                    machineID = telemetry.machineID
                };

                //같은 날짜에 datetime과 machineID를 가진 데이터 있는경우 예외처리
                //Any메소드를 사용하면 시퀀스에 조건을 만족하는 요소가 하나라도 있는지 확인할 수 있음
                if (!parseTelemetryData.Any(data => data.datetime == parseTelemetry.datetime && data.machineID == parseTelemetry.machineID))
                {
                    // 중복되지 않은 경우에만 HashSet 및 리스트에 추가
                    uniqueTelemetryData.Add(parseTelemetry);
                    parseTelemetryData.Add(parseTelemetry);
                }
                else
                {
                    
                }
            }

            parseTelemetryData.AddRange(uniqueTelemetryData); //중복이 제거된 Hashset안의 데이터를 parseTelemetryData에 추가

            // 추가: Voltage를 기반으로 플롯 데이터 생성
            Dictionary<string, Dictionary<DateTime, double>> chartData = parseTelemetryData //(machineID, datetime, volt)
                .GroupBy(data => data.machineID)
                .ToDictionary( //딕셔너리 생성
                   group => group.Key, //maxhineID를 Key값으로 사용
                   group => group.ToDictionary(item => item.datetime, item => item.volt) // 시간별 전압 데이터를 포함하는 딕셔너리 생성
                );

            return parseTelemetryData; // 중복제거된 ParseTelemetry데이터 반환
        }

    }

    public class ParseTelemetry
    {
        public DateTime datetime { get; set; }
        public string machineID { get; set; }
        public double volt { get; set; }
    }
}
