using System.Collections.Generic;
using Ultrapowa_Clash_Server_GUI.Helpers;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    //Packet 20161
    internal class ShutdownStartedMessage : Message
    {
        private int m_vCode;

        public ShutdownStartedMessage(Client client)
            : base(client)
        {
            SetMessageType(20161);
        }

        public override void Encode()
        {
            var data = new List<byte>();
            data.AddInt32(m_vCode);
            SetData(data.ToArray());
        }

        public void SetCode(int code)
        {
            m_vCode = code;
        }
    }
}