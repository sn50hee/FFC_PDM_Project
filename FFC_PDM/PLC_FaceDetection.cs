using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            this.oCommDriver = factory.GetMLDPCommObject20("192.168.1.2:2004");


        }
    }
}
