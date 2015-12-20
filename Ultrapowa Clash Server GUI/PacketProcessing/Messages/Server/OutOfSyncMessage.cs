using System.Collections.Generic;
using Ultrapowa_Clash_Server_GUI.Helpers;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    //Packet 24104
    internal class OutOfSyncMessage : Message
    {
        public OutOfSyncMessage(Client client) : base(client)
        {
            SetMessageType(24104);
        }

        public override void Encode()
        {
            var data = new List<byte>();
            data.AddInt32(0);
            data.AddInt32(0);
            data.AddInt32(0);
            SetData(data.ToArray());
        }
    }
}