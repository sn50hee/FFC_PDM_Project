using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace FFC_PDM
{
    internal class FacilityDataControl
    {
        // 함수는 리스트 형태로 불러온다. 
        // 인자는 delegate 형태로 받아온다. 그래서 화살표 함수로 파라미터를 전하는 것이 가능하다. 
        // 데이터를 읽어오는 함수
        private List<T> ReadData<T>(string filePath, Func<string[], T> createInstance)
        {
            List<T> dataList = new List<T>();

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(filePath))
            using (StreamReader reader = new StreamReader(stream))
            {
                // 데이터 읽어오기 
                reader.ReadLine(); // Skip header

                while (!reader.EndOfStream)
                {
                    // csv 파일.. , 기준으로 분리하기
                    string[] line = reader.ReadLine().Split(',');
                    for (int i = 0; i < line.Length; i++)
                    {
                        line[i] = line[i].Trim('\"');
                    }
                    // 리스트에 추가
                    dataList.Add(createInstance(line));
                }
            }

            return dataList;
        }

        public List<Failures> GetFailuresData()
        {
            string filePath = "FFC_PDM.Resources.PdM_failures.csv";
            return ReadData(filePath, line => new Failures { datetime = line[0], machineID = line[1], failure = line[2] });
        }

        public List<Errors> GetErrorsData()
        {
            string filePath = "FFC_PDM.Resources.PdM_errors.csv";
            return ReadData(filePath, line => new Errors { datetime = line[0], machineID = line[1], errorID = line[2] });
        }
        
        // 밑에 있는 클래스 -> 불러오는 데이터
        // return ReadData(filePath, line => new Machines {machineID = line[0], model = line[1], age = line[2]});
        // list => (machineID, model, age)
        public List<Machines> GetMachinesData()
        {
            string filePath = "FFC_PDM.Resources.PdM_machines.csv";
            return ReadData(filePath, line => new Machines { machineID = line[0], model = line[1], age = line[2] });
        }

        public List<Maint> GetMaintData()
        {
            string filePath = "FFC_PDM.Resources.PdM_maint.csv";
            return ReadData(filePath, line => new Maint { datetime = line[0], machineID = line[1], comp = line[2] });
        }

        public List<Telemetry> GetTelemetryData()
        {
            string filePath = "FFC_PDM.Resources.PdM_telemetry.csv";
            //return ReadData(filePath, line => new Telemetry { datetime = line[0], machineID = line[1], volt = line[2], rotate = line[3], pressure = line[4], vibration = line[5] });
            List<Telemetry> telemetryData = ReadData(filePath, line => new Telemetry { datetime = line[0], machineID = line[1], volt = line[2], rotate = line[3], pressure = line[4], vibration = line[5] });

            // 최대 20,000개의 데이터만 가져오기
            return telemetryData.Take(20000).ToList();
        }


        // 파싱 위치 변환(ViewDetailsTabChartData.cs 안씀)
        public List<ParseTelemetry_1> GetParseTelemetryData()
        {
            string filePath = "FFC_PDM.Resources.PdM_telemetry_no_duplicates.csv";
            List<ParseTelemetry_1> telemetryData = ReadData(filePath, line => new ParseTelemetry_1
            {
                datetime = DateTime.Parse(line[0]),
                machineID = double.Parse(line[1]),
                volt = double.Parse(line[2]),
                rotate = double.Parse(line[3]),
                pressure = double.Parse(line[4]),
                vibration = double.Parse(line[5])
            });

            return telemetryData;
        }
    }
}

    public class Failures
    {
        public string datetime { get; set; }
        public string machineID { get; set; }
        public string failure { get; set; }
    }

    public class Errors
    {
        public string datetime { get; set; }
        public string machineID { get; set; }
        public string errorID { get; set; }
    }

    public class Machines
    {
        //받아오는 데이터 형식 {machineID ; 1}, {model ; 1}
        public string machineID { get; set; }
        public string model { get; set; }
        public string age { get; set; }
    }

    public class Maint
    {
        public string datetime { get; set; }
        public string machineID { get; set; }
        public string comp { get; set; }
    }

    public class Telemetry
    {
    public string datetime { get; set; }
    public string machineID { get; set; }
    public string volt { get; set; }
    public string rotate { get; set; }
    public string pressure { get; set; }
    public string vibration { get; set; }
    }

public class ParseTelemetry_1
{
    public DateTime datetime { get; set; }
    public double machineID { get; set; }
    public double volt { get; set; }
    public double rotate { get; set; }
    public double pressure { get; set; }
    public double vibration { get; set; }
}