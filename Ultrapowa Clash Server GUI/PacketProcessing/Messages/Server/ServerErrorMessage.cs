using System.Collections.Generic;
using Ultrapowa_Clash_Server_GUI.Helpers;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    //Packet 24115
    internal class ServerErrorMessage : Message
    {
        private string m_vErrorMessage;

        public ServerErrorMessage(Client client)
            : base(client)
        {
            SetMessageType(24115);
        }

        public override void Encode()
        {
            var data = new List<byte>();
            data.AddString(m_vErrorMessage);
            SetData(data.ToArray());
        }

        public void SetErrorMessage(string message)
        {
            m_vErrorMessage = message;
        }
    }
}