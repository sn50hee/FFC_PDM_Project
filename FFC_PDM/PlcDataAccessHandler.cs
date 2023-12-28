using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using XGCommLib;
using System.Threading;
using Microsoft.Win32;
using ScottPlot.Styles;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Runtime.InteropServices;
using System.Collections.ObjectModel;

namespace FFC_PDM
{
    
    public enum XGCOMM_PRE_DEFINES : uint
    {
        MAX_RW_BIT_SIZE = 64,
        MAX_RW_BYTE_SIZE = 1000,
        MAX_RW_WORD_SIZE = 500,
        DEF_PLC_SERVER_TIME_OUT = 15000,
        DEF_PLC_KEEP_ALIVE_TIME = 10000
    }

    public enum DEF_DATA_TYPE : uint
    {
        DATA_TYPE_BIT = 0,
        DATA_TYPE_BYTE = 1,
        DATA_TYPE_WORD = 2
    }

    public enum XGCOMM_FUNC_RESULT : uint
    {
        RT_XGCOMM_SUCCESS = 0,                  // 함수가 수행 성공

        RT_XGCOMM_CAN_NOT_FIND_DLL = 1,         // XGCommLib.dll 파일을 찾을 수 없음, 윈도우 system32폴더의 regsvr32.exe를 이용해 등록필요
        RT_XGCOMM_FAILED_CONNECT = 2,           // PLC와 통신 접속 실패
        RT_XGCOMM_FAILED_KEEPALIVE = 3,         // PLC와 통신 접속 상태 유지 실패

        RT_XGCOMM_INVALID_COMM_DRIVER = 5,      // Comm Driver가 유효하지 않음, Connect함수를 호출하지않았거나 Disconnect를 호출한 상태
        RT_XGCOMM_INVALID_POINT = 6,	        // 함수의 인자로 전달한 배열 포인트가 NULL일 때   

        RT_XGCOMM_FAILED_RESULT = 10,           // XGCommLib.dll의 함수 실행이 실패했을 때
        RT_XGCOMM_FAILED_READ = 11,             // XGCommLib.dll의 ReadRandomDevice 함수의 반환값이 0으로 실패했을 때
        RT_XGCOMM_FAILED_WRITE = 12,            // XGCommLib.dll의 WriteRandomDevice 함수의 반환값이 0으로 실패했을 때

        RT_XGCOMM_ABOVE_MAX_BIT_SIZE = 20,      // Bit 함수의 Bit Size가 32를 초과했을 때(ReadDataBit, WriteDataBit)
        RT_XGCOMM_ABOVE_MAX_BYTE_SIZE = 21,     // Byte 함수의 Byte Size가 260를 초과했을 때(ReadDataByte, WriteDataByte)
        RT_XGCOMM_ABOVE_MAX_WORD_SIZE = 22,     // Word 함수의 Word Size가 130를 초과했을 때(ReadDataWord, WriteDataWord)
        RT_XGCOMM_BLOW_MIN_SIZE = 23,           // Size가 1보다 작을 때

        RT_XGCOMM_FAILED_GET_TIMEOUT = 25,	    // 타임아웃읽기 실패
        RT_XGCOMM_FAILED_SET_TIMEOUT = 26,	    // 타임아웃설정 실패
    }
    public class PlcDataAccessHandler
    {
        public List<CheckData> CheckDataList;
        public CommObject20 oCommDriver = null;
        public Int32 m_nLastCommTime = 0;
        public string m_strIP;
        public long m_lPortNo;
        int i = 0;
        public Object m_MonitorLock = new System.Object();

        

        public PlcDataAccessHandler(List<CheckData> checkDataList)
        {
            //MainWindow에서 작업한 CheckData 갖고오기
            CheckDataList = checkDataList;
        }

        private static byte LOBYTE(ushort a)
        {
            return (byte)(a & 0xff);
        }

        private static byte HIBYTE(ushort a)
        {
            return (byte)(a >> 8);
        }

        public uint UpdateKeepAlive()
        {
            uint dwTimeSpen; // 내가 측정한 시간 - 마지막 접속시간
            uint dwReturn; // 성공인지 아닌지 한번 보기!

            if (this.oCommDriver == null)
            {
                MessageBox.Show("Fail");
            }

            dwTimeSpen = (uint)TICKS_DIFF(m_nLastCommTime, Environment.TickCount);

            if (dwTimeSpen > (uint)XGCOMM_PRE_DEFINES.DEF_PLC_KEEP_ALIVE_TIME)
            {
                // 한번 읽어와!
                dwReturn = ReadDataBit('F', 0, 1, null);
                // 그래도 실패해?
                if (dwReturn != (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_SUCCESS)
                {
                    // 타임 아웃!
                    if (dwTimeSpen > (uint)XGCOMM_PRE_DEFINES.DEF_PLC_SERVER_TIME_OUT)
                    {
                        // KeepAlive에 완벽히 실패했습니다.
                        return (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_FAILED_KEEPALIVE;
                        MessageBox.Show("Keep Failed");
                    }
                }
                else
                {
                    m_nLastCommTime = Environment.TickCount;
                }
            }
            if(this.oCommDriver.Connect("") == 1)
            {
                MessageBox.Show("Keep Success");
            }
            
            return (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_SUCCESS;

        }
        //DataGrid DG_checkData.Items
        public void Connect_Start()
        {
            // 연결하기
            CommObjectFactory20 factory = new CommObjectFactory20();
            this.oCommDriver = factory.GetMLDPCommObject20("192.168.1.2:2004");
            // checkBox 리스트에 있는 volt, rotate, pressure, vibration 최대 최소값 넣을 리스트
            List<Dictionary<string, int>> plc_write = new List<Dictionary<string, int>>();

            foreach (CheckData checkData in CheckDataList)
            {
                Dictionary<string, int> checkDataDict = new Dictionary<string, int>()
                {
                    { "modelId", int.Parse(checkData.ModelId) },
                    { "voltMin", int.Parse(checkData.VoltMin.ToString()) },
                    { "voltMax", int.Parse(checkData.VoltMax.ToString()) },
                    { "rotateMin", int.Parse(checkData.RotateMin.ToString()) },
                    { "rotateMax", int.Parse(checkData.RotateMax.ToString()) },
                    { "pressureMin", int.Parse(checkData.PressureMin.ToString()) },
                    { "pressureMax", int.Parse(checkData.PressureMax.ToString()) },
                    { "vibrationMin", int.Parse(checkData.VibrationMin.ToString()) },
                    { "vibrationMax", int.Parse(checkData.VibrationMax.ToString()) }

                };
                plc_write.Add(checkDataDict);
            }

            FacilityDataControl facilityDataControl = new FacilityDataControl();
            List<ParseTelemetry_PLC> csvDataList = facilityDataControl.GetPLCParseTelemetryData();
            int index = 0;


            if (1 == this.oCommDriver.Connect(""))
            {
                MessageBox.Show("Connect Success");
                DispatcherTimer timer = new DispatcherTimer();

                PlcWriteInitData(plc_write, 16);

                timer.Tick += (sender, e) => {
                    PlcWriteCsvData(csvDataList[index], 2);
                    index = (index < csvDataList.Count - 1) ? index + 1 : 0;
                    //KeepAlive();
                };
                timer.Interval = TimeSpan.FromSeconds(1);
                timer.Start();
            }
            m_nLastCommTime = Environment.TickCount;


        }

        public void PlcWriteCsvData(ParseTelemetry_PLC plcWriteCsvData, long lSizeWord)
        {

            long lRetValue = 0;
            byte[] bufWrite = new byte[2];
            bool writeSuccess = false;


            for (int j = 0; j < 4; j++)
            {
                CommObjectFactory20 factory = new CommObjectFactory20();
                this.oCommDriver.RemoveAll();
                XGCommLib.DeviceInfo oDevice = factory.CreateDevice();
                // 데이터 타입
                oDevice.ucDeviceType = (byte)'M';
                // 데이터 크기
                oDevice.ucDataType = (byte)'B';
                // 시작점 제시, byte 단위
                oDevice.lOffset = j * 6 + (plcWriteCsvData.modelID-1)*24;
                // 크기 C#은 Byte 기준으로?
                oDevice.lSize = 2; //Word
                this.oCommDriver.AddDeviceInfo(oDevice);

                if (null != this.oCommDriver)
                {
                    if ( j == 0 )
                    {
                        bufWrite[0] = LOBYTE((ushort)(plcWriteCsvData.volt));
                        bufWrite[1] = HIBYTE((ushort)(plcWriteCsvData.volt));
                    }
                    else if (j == 1)
                    {
                        bufWrite[0] = LOBYTE((ushort)(plcWriteCsvData.rotate));
                        bufWrite[1] = HIBYTE((ushort)(plcWriteCsvData.rotate));
                    }
                    else if (j == 2)
                    {
                        bufWrite[0] = LOBYTE((ushort)(plcWriteCsvData.pressure));
                        bufWrite[1] = HIBYTE((ushort)(plcWriteCsvData.pressure));
                    }
                    else if (j == 3)
                    {
                        bufWrite[0] = LOBYTE((ushort)(plcWriteCsvData.vibration));
                        bufWrite[1] = HIBYTE((ushort)(plcWriteCsvData.vibration));
                    }
                    lRetValue = this.oCommDriver.WriteRandomDevice(bufWrite);

                    // 성공 여부 체크
                    writeSuccess = (lRetValue == 1);

                }
            }

            if (!writeSuccess)
            {
                MessageBox.Show("데이터 쓰기 실패");
            }
        }

        public void PlcWriteInitData(List<Dictionary<string, int>> plcWriteDataList, long lSizeWord)
        {

            long lRetValue = 0;
            byte[] bufWrite = new byte[4];
            bool writeSuccess = false;

            for (int i = 0; i < plcWriteDataList.Count; i++)
            {
                for( int j = 0; j<4; j++)
                {
                    CommObjectFactory20 factory = new CommObjectFactory20();
                    this.oCommDriver.RemoveAll();
                    XGCommLib.DeviceInfo oDevice = factory.CreateDevice();
                    // 데이터 타입
                    oDevice.ucDeviceType = (byte)'M';
                    // 데이터 크기
                    oDevice.ucDataType = (byte)'B';
                    // 시작점 제시, byte 단위
                    oDevice.lOffset = i * 24 + 2 + j * 6;
                    // 크기 C#은 Byte 기준으로?
                    oDevice.lSize = 4; //Word
                    this.oCommDriver.AddDeviceInfo(oDevice);

                    if (null != this.oCommDriver)
                    {
                        string propertyPrefix = "";
                        if (j == 0) propertyPrefix = "volt";
                        else if (j == 1) propertyPrefix = "rotate";
                        else if (j == 2) propertyPrefix = "pressure";
                        else if (j == 3) propertyPrefix = "vibration";

                        bufWrite[0] = LOBYTE((ushort)(plcWriteDataList[i][$"{propertyPrefix}Min"]));
                        bufWrite[1] = HIBYTE((ushort)(plcWriteDataList[i][$"{propertyPrefix}Min"]));
                        bufWrite[2] = LOBYTE((ushort)(plcWriteDataList[i][$"{propertyPrefix}Max"]));
                        bufWrite[3] = HIBYTE((ushort)(plcWriteDataList[i][$"{propertyPrefix}Max"]));

                        lRetValue = this.oCommDriver.WriteRandomDevice(bufWrite);

                        // 성공 여부 체크
                        writeSuccess = (lRetValue == 1);

                    }
                }
            }

            if (writeSuccess)
            {
                MessageBox.Show("Write OK!");
            }
        }

        

        // 연결용
        public void Re_Connect()
        {
            // 연결하기
            CommObjectFactory20 factory = new CommObjectFactory20();
            this.oCommDriver = factory.GetMLDPCommObject20("192.168.1.2:2004");
            if (1 == oCommDriver.Connect(""))
            {
                MessageBox.Show("Connect Success");
            }

            m_nLastCommTime = Environment.TickCount;

        }

        // 접속 끊기
        public uint Disconnect()
        {
            if (this.oCommDriver != null)
            {
                this.oCommDriver.Disconnect();

                return (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_SUCCESS;
            }
            return (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_INVALID_COMM_DRIVER;
        }

        // Plc 접속 지속
        // Timer

        public void KeepAlive()
        {
            PlcDataAccessHandler handler = new PlcDataAccessHandler(CheckDataList);
            uint uReturn = handler.UpdateKeepAlive();
        }

        
        public void Plc_Read(bool bByteSwap)
        {
            // 연결 확인
            if (this.oCommDriver == null)
            {
                MessageBox.Show("Read Fail");
            }

            else
            {
                CommObjectFactory20 factory = new CommObjectFactory20();
                this.oCommDriver.RemoveAll();
                XGCommLib.DeviceInfo oDevice = factory.CreateDevice();
                byte[] bufRead = new byte[12];
                UInt16[] pwRead = new UInt16[12];
                long dwReturn;
                long lRetValue = 0;
                // 데이터 위치
                oDevice.ucDeviceType = (byte)'M';
                // 데이터 크기
                oDevice.ucDataType = (byte)'B';
                // 시작점 제시, byte 단위
                oDevice.lOffset = 3000;
                
                // 크기 C#은 Byte 기준으로?
                oDevice.lSize = 12; //Word
                dwReturn = ReadDataByte('M', 'B', 12, bufRead);

                if (dwReturn == (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_SUCCESS)
                {
                    if (pwRead != null)
                    {
                        if (bByteSwap == true)
                        {
                            for (int i = 0; i < 12; i++)
                            {
                                
                                pwRead[i] = MakeWord(bufRead[2*i + 1], bufRead[2*i]);
                            }
                        }
                        else
                        {
                            System.Buffer.BlockCopy(bufRead, 0, pwRead, 0, (Int32)24);
                        }
                    }
                }
                if (dwReturn == (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_SUCCESS)
                {
                    m_nLastCommTime = Environment.TickCount;
                    MessageBox.Show("Connect Success");
                }
                
            }



        }

        // 실험
        public uint ReadDataBit(char szDeviceType, long lOffsetBit, long lSizeBit, Byte[] pbyRead)
        {
            if (oCommDriver == null)
            {
                return (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_INVALID_COMM_DRIVER;
            }

            uint dwReteurn;
            long lRetValue = 0, lCount = 0, lByteOffset, lBitOffset;
            CommObjectFactory20 factory = new CommObjectFactory20();
            XGCommLib.DeviceInfo oDevice;

            Lock();
            this.oCommDriver.RemoveAll();

            for (lCount = 0; lCount < lSizeBit; lCount++)
            {
                oDevice = factory.CreateDevice();

                oDevice.ucDataType = (byte)'X';
                oDevice.ucDeviceType = (byte)szDeviceType;

                lByteOffset = (lOffsetBit + lCount) / 8;	// byte offset
                lBitOffset = (lOffsetBit + lCount) % 8;	// bit offset

                oDevice.lOffset = (Int32)lByteOffset;
                oDevice.lSize = (Int32)lBitOffset;

                this.oCommDriver.AddDeviceInfo(oDevice);
            }
            byte[] bufRead = new byte[lSizeBit];
            lRetValue = this.oCommDriver.ReadRandomDevice(bufRead);
            if (0 == lRetValue)
            {
                //++ 재 연결 시도
                Re_Connect();
                if (this.oCommDriver.Connect("") == 1)
                {
                    lRetValue = this.oCommDriver.ReadRandomDevice(bufRead);
                    if (0 == lRetValue)
                    {
                        UnLock();
                        return (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_FAILED_READ;
                    }
                }
                else
                {
                    UnLock();
                    MessageBox.Show("Read Fail");
                }
            }
            UnLock();

            if (pbyRead != null)
            {
                bufRead.CopyTo(pbyRead, 0);
            }

            m_nLastCommTime = Environment.TickCount;

            return (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_SUCCESS;

        }

        public uint ReadDataByte(char szDeviceType, long lOffsetByte, long lSizeByte, Byte[] pbyRead)
        {
            if (this.oCommDriver == null)
            {
                return (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_INVALID_COMM_DRIVER;
            }

            if (lSizeByte > (uint)XGCOMM_PRE_DEFINES.MAX_RW_BYTE_SIZE)
            {
                return (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_ABOVE_MAX_BYTE_SIZE;
            }

            if (lSizeByte < 1)
            {
                return (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_BLOW_MIN_SIZE;
            }

            uint dwReteurn;
            long lRetValue = 0;
            CommObjectFactory20 factory = new CommObjectFactory20();

            Lock();
            this.oCommDriver.RemoveAll();

            XGCommLib.DeviceInfo oDevice = factory.CreateDevice();

            oDevice.ucDataType = (byte)'B';
            oDevice.ucDeviceType = (byte)szDeviceType;

            oDevice.lOffset = (Int32)lOffsetByte;
            oDevice.lSize = (Int32)lSizeByte;

            this.oCommDriver.AddDeviceInfo(oDevice);

            byte[] bufRead = new byte[lSizeByte];
            lRetValue = this.oCommDriver.ReadRandomDevice(bufRead);
            if (0 == lRetValue)
            {
                //++ 재 연결 시도
                Re_Connect();
                if (this.oCommDriver != null)
                {
                    lRetValue = this.oCommDriver.ReadRandomDevice(bufRead);
                    if (0 == lRetValue)
                    {
                        //UnLock();
                        return (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_FAILED_READ;
                    }
                }
                else
                {
                   //UnLock();
                    MessageBox.Show("Read Failed");
                }
            }
            //UnLock();

            if (pbyRead != null)
            {
                bufRead.CopyTo(pbyRead, 0);
            }

            m_nLastCommTime = Environment.TickCount;

            return (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_SUCCESS;
        }


        public Int32 TICKS_DIFF(int prev, int cur)
        {
            Int32 nReturn;
            if (cur >= prev)
            {
                nReturn = cur - prev;
            }

            else
            {
                unchecked
                {
                    nReturn = ((int)0xFFFFFFFF - prev) + 1 + cur;
                }
            }
            return nReturn;
        }

        public UInt16 MakeWord(byte low, byte high)
        {
            return (UInt16)((high << 8) | low);
        }


        private void Lock()
        {
            Monitor.Enter(m_MonitorLock);
        }

        private void UnLock()
        {
            Monitor.Exit(m_MonitorLock);
        }



    }
}
