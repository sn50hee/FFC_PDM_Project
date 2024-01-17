using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows;
using XGCommLib;

namespace FFC_PDM
{
    internal class PLC_FaceDetection
    {
        public CommObject20 oCommDriver = null;
        public Int32 m_nLastCommTime = 0;

        public void Connect()
        {
            CommObjectFactory20 factory = new CommObjectFactory20();
            this.oCommDriver = factory.GetMLDPCommObject20("192.168.0.30:2004");

            if (1 == this.oCommDriver.Connect(""))
            {
                
                Plc_write(230, 1);

                

               
            }
            m_nLastCommTime = Environment.TickCount;
        }

        public void Plc_write(int start, int size)
        {
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
            oDevice.lOffset = (Int32)start;

            // 크기 C#은 Byte 기준으로?
            oDevice.lSize = (Int32)size; //Word
            this.oCommDriver.AddDeviceInfo(oDevice);
            bufWrite[0] = 1;
            lRetValue = this.oCommDriver.WriteRandomDevice(bufWrite);

            // 성공 여부 체크
            writeSuccess = (lRetValue == 1);
        }
    }
}
