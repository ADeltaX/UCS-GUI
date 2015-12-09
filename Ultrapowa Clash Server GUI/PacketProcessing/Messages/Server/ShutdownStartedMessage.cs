using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ultrapowa_Clash_Server_GUI.Logic;
using Ultrapowa_Clash_Server_GUI.Core;
using Ultrapowa_Clash_Server_GUI.Helpers;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    //Packet 20161
    class ShutdownStartedMessage : Message
    {
        private int m_vCode;

        public ShutdownStartedMessage(Client client)
            : base(client)
        {
            SetMessageType(20161);
        }

        public override void Encode()
        {
            List<Byte> data = new List<Byte>();
            data.AddInt32(m_vCode);
            SetData(data.ToArray());
        }

        public void SetCode(int code)
        {
            m_vCode = code;
        }
    }
}
