using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows;
using XGCommLib;
using System.Diagnostics;

namespace FFC_PDM
{
    internal class GazePLCAccessHandler
    {
        public List<CheckData> CheckDataList;
        public CommObject20 oCommDriver = null;
        public Int32 m_nLastCommTime = 0;
        public string m_strIP;
        public long m_lPortNo;
        int i = 0;
        public Object m_MonitorLock = new System.Object();

        public void Connect_Start()
        {
            // 연결하기
            CommObjectFactory20 factory = new CommObjectFactory20();
            this.oCommDriver = factory.GetMLDPCommObject20("192.168.0.30:2004");
            // checkBox 리스트에 있는 volt, rotate, pressure, vibration 최대 최소값 넣을 리스트
            List<Dictionary<string, int>> plc_write = new List<Dictionary<string, int>>();


            FacilityDataControl facilityDataControl = new FacilityDataControl();
            List<ParseTelemetry_PLC> csvDataList = facilityDataControl.GetPLCParseTelemetryData();
            int index = 0;


            if (1 == this.oCommDriver.Connect(""))
            {
                MessageBox.Show("Connect Success");
                PlcWriteCsvData();
            }
            m_nLastCommTime = Environment.TickCount;

        }

        public void PlcWriteCsvData()
        {
            bool isGazeCenter = true;
            GetGazeTracking getGazeTracking = new GetGazeTracking();
            List<string> newDatas = new List<string>();
            getGazeTracking.GazeTracking();

            var updateResultTask = Task.Run(async () =>
            {
                while (true)
                {
                    bool _isGazeCenter = getGazeTracking.GetApiCheckResult()!="center"? false : true;
                    //Debug.WriteLine(_isGazeCenter);

                    long lRetValue = 0;
                    byte[] bufWrite = new byte[1];
                    bool writeSuccess = false;

                    CommObjectFactory20 factory = new CommObjectFactory20();
                    this.oCommDriver.RemoveAll();
                    XGCommLib.DeviceInfo oDevice = factory.CreateDevice();
                    // 데이터 타입
                    oDevice.ucDeviceType = (byte)'M';
                    // 데이터 크기
                    oDevice.ucDataType = (byte)'B';
                    // 시작점 제시, byte 단위
                    oDevice.lOffset = 220;
                    oDevice.lSize = 1;
                    this.oCommDriver.AddDeviceInfo(oDevice);

                    if (null != this.oCommDriver)
                    {
                        bufWrite[0] = Convert.ToByte(isGazeCenter);

                        lRetValue = this.oCommDriver.WriteRandomDevice(bufWrite);

                        // 성공 여부 체크
                        writeSuccess = (lRetValue == 1);
                    }

                    if (!writeSuccess)
                    {
                        MessageBox.Show("데이터 쓰기 실패");
                    }
                    await Task.Delay(1000); // 1초 대기
                }
            });
            
        }
    }
}
