﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace FFC_PDM
{
    internal class FacilityDataControl
    {
        private List<T> ReadData<T>(string filePath, Func<string[], T> createInstance)
        {
            List<T> dataList = new List<T>();

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(filePath))
            using (StreamReader reader = new StreamReader(stream))
            {
                reader.ReadLine(); // Skip header

                while (!reader.EndOfStream)
                {
                    string[] line = reader.ReadLine().Split(',');
                    for (int i = 0; i < line.Length; i++)
                    {
                        line[i] = line[i].Trim('\"');
                    }
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
            return ReadData(filePath, line => new Telemetry { datetime = line[0], machineID = line[1], volt = line[2], rotate = line[3], pressure = line[4], vibration = line[5] });
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