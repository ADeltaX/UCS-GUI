using System.Collections.Generic;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    //Packet 24303
    internal class AllianceJoinOkMessage : Message
    {
        public AllianceJoinOkMessage(Client client) : base(client)
        {
            SetMessageType(24303);
        }

        public override void Encode()
        {
            var pack = new List<byte>();
            SetData(pack.ToArray());
        }
    }
}