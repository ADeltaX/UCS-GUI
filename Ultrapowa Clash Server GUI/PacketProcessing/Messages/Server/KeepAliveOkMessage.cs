using System.Collections.Generic;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    //Packet 20108
    internal class KeepAliveOkMessage : Message
    {
        public KeepAliveOkMessage(Client client, KeepAliveMessage cka)
            : base(client)
        {
            SetMessageType(20108);
        }

        public override void Encode()
        {
            var data = new List<byte>();
            SetData(data.ToArray());
        }
    }
}