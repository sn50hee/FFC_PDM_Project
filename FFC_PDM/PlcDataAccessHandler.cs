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
        ~PlcDataAccessHandler()
        {

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
            List<int> plc_write = new List<int>();
            foreach (var checkData in CheckDataList)
            {
                double voltMin = checkData.VoltMin;
                plc_write.Add(int.Parse(voltMin.ToString()));
                double voltMax = checkData.VoltMax;
                plc_write.Add(int.Parse(voltMax.ToString()));
                double rotateMin = checkData.RotateMin;
                plc_write.Add(int.Parse(rotateMin.ToString()));
                double rotateMax = checkData.RotateMax;
                plc_write.Add(int.Parse(rotateMax.ToString()));
                double pressureMin = checkData.PressureMin;
                plc_write.Add(int.Parse(pressureMin.ToString()));
                double pressureMax = checkData.PressureMax;
                plc_write.Add(int.Parse(pressureMax.ToString()));
                double vibrationMin = checkData.VibrationMin;
                plc_write.Add(int.Parse(vibrationMin.ToString()));
                double vibrationMax = checkData.VibrationMax;
                plc_write.Add(int.Parse(vibrationMax.ToString()));
                
            }
            
            
            if (1 == this.oCommDriver.Connect(""))
            {
                MessageBox.Show("Connect Success");
                                
                DispatcherTimer timer = new DispatcherTimer();
                //List<StatisticsTabGridData> datagridItem
                // PLC_Write -> 24byte 쓰기
                // Plc_Write_Repeate -> 24byte 연속 쓰기 - /timer 통해서 쓸 것
               
                PLC_Write_Data(plc_write);
                Plc_Write_Repeat(plc_write, 0, 24, 0);
                //   Plc_Read(true);

               timer.Interval = TimeSpan.FromSeconds(10);  // 정해진 시간(1초)마다 
                //timer.Tick += Write_Repeat_Timer;
               timer.Tick += Keep_alive;// 함수를 이벤트 핸들러에 보내서 실행한다.
               timer.Start();
               
               UpdateKeepAlive();

            }

            /*
            else
            {
                Connect();
            }
            */
            m_nLastCommTime = Environment.TickCount;


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

        public void Keep_alive(object sender, EventArgs e)
        {

            PlcDataAccessHandler handler = new PlcDataAccessHandler(CheckDataList);
            uint uReturn = handler.UpdateKeepAlive();

            // Alive 시작!!!!
        }

        // 타이머 만들기
        public void Write_Repeat_Timer(object sender, EventArgs e)
        {
            PlcDataAccessHandler handler = new PlcDataAccessHandler(CheckDataList);
            List<int> plc_write = new List<int>();
            foreach (var checkData in CheckDataList)
            {
                double voltMin = checkData.VoltMin;
                plc_write.Add(int.Parse(voltMin.ToString()));
                double voltMax = checkData.VoltMax;
                plc_write.Add(int.Parse(voltMax.ToString()));
                double rotateMin = checkData.RotateMin;
                plc_write.Add(int.Parse(rotateMin.ToString()));
                double rotateMax = checkData.RotateMax;
                plc_write.Add(int.Parse(rotateMax.ToString()));
                double pressureMin = checkData.PressureMin;
                plc_write.Add(int.Parse(pressureMin.ToString()));
                double pressureMax = checkData.PressureMax;
                plc_write.Add(int.Parse(pressureMax.ToString()));
                double vibrationMin = checkData.VibrationMin;
                plc_write.Add(int.Parse(vibrationMin.ToString()));
                double vibrationMax = checkData.VibrationMax;
                plc_write.Add(int.Parse(vibrationMax.ToString()));

            }
            Plc_Write_Repeat(plc_write, 24*i, 24, i);
            i += 1;
        }



        public void Plc_Write()
        {

            long lRetValue = 0;
            byte[] bufWrite = new byte[24];
            // 실제 데이터의 길이를 저장할 변수

            CommObjectFactory20 factory = new CommObjectFactory20();
            //CommObject20 oCommDriver = factory.GetMLDPCommObject20("192.168.1.20:2004");
            this.oCommDriver.RemoveAll();
            XGCommLib.DeviceInfo oDevice = factory.CreateDevice();
            // 데이터 타입
            oDevice.ucDeviceType = (byte)'M';
            // 데이터 크기
            oDevice.ucDataType = (byte)'B';
            // 시작점 제시, byte 단위
            oDevice.lOffset = 0;
            // 크기 C#은 Byte 기준으로?
            oDevice.lSize = 2 * 12; //Word
            this.oCommDriver.AddDeviceInfo(oDevice);

            /*
            string data = Convert.ToString(7, 2);
            bufWrite[0] = HIBYTE(ushort.Parse(data));
            bufWrite[1] = LOBYTE(ushort.Parse(data));
            */
            //for (int i = 0; i < 12; i++)
            //{
            //    // HIBYTE, LOBYTE - 2byte
            //    // 1byte = HIBYTE로 지정
            //    // 1byte = LOBYTE로 지정
            //    bufWrite[2 * i] = LOBYTE((ushort)(65 * i)); // 값을 저장하기 위한 변수
            //    bufWrite[2 * i + 1] = HIBYTE((ushort)(65 * i)); 
            //}

            //m_nLastCommTime = Environment.TickCount;
            // 연결 성공

            if (null != this.oCommDriver)
            {
                for (int i = 0; i < 12; i++)
                {
                    // HIBYTE, LOBYTE - 2byte
                    // 1byte = HIBYTE로 지정
                    // 1byte = LOBYTE로 지정
                    bufWrite[2 * i] = LOBYTE((ushort)(85 * i));// 값을 저장하기 위한 변수
                    bufWrite[2 * i + 1] = HIBYTE((ushort)(85 * i));
                }

                //변경할 데이터의 값을 보관할 byte 배열
                lRetValue = this.oCommDriver.WriteRandomDevice(bufWrite);

                if (1 == lRetValue)
                {
                    MessageBox.Show("Write OK!");
                    m_nLastCommTime = Environment.TickCount;
                }

                else
                {
                    MessageBox.Show("Write Failed");
                    Re_Connect();
                    if(oCommDriver.Connect("") == 1)
                    {
                        lRetValue = this.oCommDriver.WriteRandomDevice(bufWrite);
                        if (0 == lRetValue)
                        {
                            //UnLock();
                            MessageBox.Show("Write Failed");
                        }
                    }

                    else
                    {
                        //UnLock();
                        Re_Connect();
                    }
                }
                //UnLock();
                m_nLastCommTime = Environment.TickCount;

            }

            else
            {
                MessageBox.Show("Connect Fail");
                Re_Connect();
            }
        }
        // 시험용 각자 쓰기
        public void PLC_Write_Data(List<int> plc_write)
        {
            FacilityDataControl Data = new FacilityDataControl();
            long lRetValue = 0;
            byte[] bufWrite = new byte[24];
            List<int> Plc_Value = new List<int>();
            // 실제 데이터의 길이를 저장할 변수

            CommObjectFactory20 factory = new CommObjectFactory20();
            //CommObject20 oCommDriver = factory.GetMLDPCommObject20("192.168.1.20:2004");
            this.oCommDriver.RemoveAll();
            XGCommLib.DeviceInfo oDevice = factory.CreateDevice();
            // 데이터 타입
            oDevice.ucDeviceType = (byte)'M';
            // 데이터 크기
            oDevice.ucDataType = (byte)'B';
            // 시작점 제시, byte 단위
            oDevice.lOffset = 0;
            // 크기 C#은 Byte 기준으로?
            oDevice.lSize = 2 * 12; //Word
            this.oCommDriver.AddDeviceInfo(oDevice);
            List<ParseTelemetry_PLC> Plc = Data.GetPLC_ParseTelemetryData();

            foreach (ParseTelemetry_PLC pl in Plc)
            {
                Plc_Value.Add(pl.volt);
                Plc_Value.Add(pl.rotate);
                Plc_Value.Add(pl.pressure);
                Plc_Value.Add(pl.vibration);
            }
              
            //m_nLastCommTime = Environment.TickCount;
            // 연결 성공

            if (null != this.oCommDriver)
            {
                // 0 ~ 5 Volt : 실제, 최소, 최대 순
                bufWrite[0] = LOBYTE((ushort)(Plc_Value[0]));
                bufWrite[1] = HIBYTE((ushort)(Plc_Value[0]));
                bufWrite[2] = LOBYTE((ushort)(plc_write[0]));
                bufWrite[3] = HIBYTE((ushort)(plc_write[0]));
                bufWrite[4] = LOBYTE((ushort)(plc_write[1]));
                bufWrite[5] = HIBYTE((ushort)(plc_write[1]));

                // 6 ~ 11 rotate : 실제, 최소, 최대 순
                bufWrite[6] = LOBYTE((ushort)(Plc_Value[1]));
                bufWrite[7] = HIBYTE((ushort)(Plc_Value[1]));
                bufWrite[8] = LOBYTE((ushort)(plc_write[2]));
                bufWrite[9] = HIBYTE((ushort)(plc_write[2]));
                bufWrite[10] = LOBYTE((ushort)(plc_write[3]));
                bufWrite[11] = HIBYTE((ushort)(plc_write[3]));

                // 12~17 pressure : 실제, 최소, 최대 순
                bufWrite[12] = LOBYTE((ushort)(Plc_Value[2]));
                bufWrite[13] = HIBYTE((ushort)(Plc_Value[2]));
                bufWrite[14] = LOBYTE((ushort)(plc_write[4]));
                bufWrite[15] = HIBYTE((ushort)(plc_write[4]));
                bufWrite[16] = LOBYTE((ushort)(plc_write[5]));
                bufWrite[17] = HIBYTE((ushort)(plc_write[5]));

                // 18~23 vibration : 실제, 최소, 최대 순
                bufWrite[18] = LOBYTE((ushort)(Plc_Value[3]));
                bufWrite[19] = HIBYTE((ushort)(Plc_Value[3]));
                bufWrite[20] = LOBYTE((ushort)(plc_write[6]));
                bufWrite[21] = HIBYTE((ushort)(plc_write[6]));
                bufWrite[22] = LOBYTE((ushort)(plc_write[7]));
                bufWrite[23] = HIBYTE((ushort)(plc_write[7]));

                //변경할 데이터의 값을 보관할 byte 배열
                lRetValue = this.oCommDriver.WriteRandomDevice(bufWrite);

                if (1 == lRetValue)
                {
                    MessageBox.Show("Write OK!");
                    m_nLastCommTime = Environment.TickCount;
                }

                else
                {
                    MessageBox.Show("Write Failed");
                    Re_Connect();
                    if (oCommDriver.Connect("") == 1)
                    {
                        lRetValue = this.oCommDriver.WriteRandomDevice(bufWrite);
                        if (0 == lRetValue)
                        {
                            //UnLock();
                            MessageBox.Show("Write Failed");
                        }
                    }

                    else
                    {
                        //UnLock();
                        Re_Connect();
                    }
                }
                //UnLock();
                m_nLastCommTime = Environment.TickCount;

            }

            else
            {
                MessageBox.Show("Connect Fail");
                Re_Connect();
            }
        }

        // 연속 쓰기
        // plc_write - checklist Data 중 최소, 최대 volt, rotate, pressure, vibrate 
        // Plc - 실제 volt, rotate, pressure, vibrate
        // long lSizeWord, a
        public void Plc_Write_Repeat(List<int> plc_write, long lOffsetWord, long lSizeWord, int num)
        {
            FacilityDataControl Data = new FacilityDataControl();
            long lRetValue = 0;
            byte[] bufWrite = new byte[lSizeWord];
            List<int> Plc_Value = new List<int>();
            // 실제 데이터의 길이를 저장할 변수

            CommObjectFactory20 factory = new CommObjectFactory20();
            //CommObject20 oCommDriver = factory.GetMLDPCommObject20("192.168.1.20:2004");
            this.oCommDriver.RemoveAll();
            XGCommLib.DeviceInfo oDevice = factory.CreateDevice();
            // 데이터 타입
            oDevice.ucDeviceType = (byte)'M';
            // 데이터 크기
            oDevice.ucDataType = (byte)'B';
            // 시작점 제시, byte 단위
            oDevice.lOffset = int.Parse(lOffsetWord.ToString());
            
            // 크기 C#은 Byte 기준으로?
            oDevice.lSize = int.Parse(lSizeWord.ToString()); //Word
            this.oCommDriver.AddDeviceInfo(oDevice);
            List<ParseTelemetry_PLC> Plc = Data.GetPLC_ParseTelemetryData();

            foreach (ParseTelemetry_PLC pl in Plc)
            {
                Plc_Value.Add(pl.volt);
                Plc_Value.Add(pl.rotate);
                Plc_Value.Add(pl.pressure);
                Plc_Value.Add(pl.vibration);
            }

            if (null != this.oCommDriver)
            {
                // 0 ~ 5 Volt : 실제, 최소, 최대 순
                bufWrite[int.Parse(lOffsetWord.ToString())] = LOBYTE((ushort)(Plc_Value[4*num]));
                bufWrite[int.Parse(lOffsetWord.ToString())+1] = HIBYTE((ushort)(Plc_Value[4*num]));
                bufWrite[int.Parse(lOffsetWord.ToString()) + 2] = LOBYTE((ushort)(plc_write[0]));
                bufWrite[int.Parse(lOffsetWord.ToString()) + 3] = HIBYTE((ushort)(plc_write[0]));
                bufWrite[int.Parse(lOffsetWord.ToString()) + 4] = LOBYTE((ushort)(plc_write[1]));
                bufWrite[int.Parse(lOffsetWord.ToString()) + 5] = HIBYTE((ushort)(plc_write[1]));

                // 6 ~ 11 rotate : 실제, 최소, 최대 순
                bufWrite[int.Parse(lOffsetWord.ToString()) + 6] = LOBYTE((ushort)(Plc_Value[4*num+1]));
                bufWrite[int.Parse(lOffsetWord.ToString()) + 7] = HIBYTE((ushort)(Plc_Value[4*num+1]));
                bufWrite[int.Parse(lOffsetWord.ToString()) + 8] = LOBYTE((ushort)(plc_write[2]));
                bufWrite[int.Parse(lOffsetWord.ToString()) + 9] = HIBYTE((ushort)(plc_write[2]));
                bufWrite[int.Parse(lOffsetWord.ToString()) + 10] = LOBYTE((ushort)(plc_write[3]));
                bufWrite[int.Parse(lOffsetWord.ToString()) + 11] = HIBYTE((ushort)(plc_write[3]));

                // 12~17 pressure : 실제, 최소, 최대 순
                bufWrite[int.Parse(lOffsetWord.ToString()) + 12] = LOBYTE((ushort)(Plc_Value[4*num+2]));
                bufWrite[int.Parse(lOffsetWord.ToString()) + 13] = HIBYTE((ushort)(Plc_Value[4*num+2]));
                bufWrite[int.Parse(lOffsetWord.ToString()) + 14] = LOBYTE((ushort)(plc_write[4]));
                bufWrite[int.Parse(lOffsetWord.ToString()) + 15] = HIBYTE((ushort)(plc_write[4]));
                bufWrite[int.Parse(lOffsetWord.ToString()) + 16] = LOBYTE((ushort)(plc_write[5]));
                bufWrite[int.Parse(lOffsetWord.ToString()) + 17] = HIBYTE((ushort)(plc_write[5]));

                // 18~23 vibration : 실제, 최소, 최대 순
                bufWrite[int.Parse(lOffsetWord.ToString()) + 18] = LOBYTE((ushort)(Plc_Value[4*num+3]));
                bufWrite[int.Parse(lOffsetWord.ToString()) + 19] = HIBYTE((ushort)(Plc_Value[4*num+3]));
                bufWrite[int.Parse(lOffsetWord.ToString()) + 20] = LOBYTE((ushort)(plc_write[6]));
                bufWrite[int.Parse(lOffsetWord.ToString()) + 21] = HIBYTE((ushort)(plc_write[6]));
                bufWrite[int.Parse(lOffsetWord.ToString()) + 22] = LOBYTE((ushort)(plc_write[7]));
                bufWrite[int.Parse(lOffsetWord.ToString()) + 23] = HIBYTE((ushort)(plc_write[7]));

                //변경할 데이터의 값을 보관할 byte 배열
                lRetValue = this.oCommDriver.WriteRandomDevice(bufWrite);

                if (1 == lRetValue)
                {
                    MessageBox.Show("Write OK!");
                    m_nLastCommTime = Environment.TickCount;
                }

                else
                {
                    MessageBox.Show("Write Failed");
                    Re_Connect();
                    if (oCommDriver.Connect("") == 1)
                    {
                        lRetValue = this.oCommDriver.WriteRandomDevice(bufWrite);
                        if (0 == lRetValue)
                        {
                            //UnLock();
                            MessageBox.Show("Write Failed");
                        }

                        else
                        {
                            MessageBox.Show("Write Success!");
                        }
                    }

                    else
                    {
                        //UnLock();
                        Re_Connect();
                    }
                }
                //UnLock();
                m_nLastCommTime = Environment.TickCount;

            }

            else
            {
                MessageBox.Show("Connect Fail");
                Re_Connect();
            }
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
