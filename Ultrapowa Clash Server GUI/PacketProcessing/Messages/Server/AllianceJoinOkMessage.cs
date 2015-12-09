using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ultrapowa_Clash_Server_GUI.Helpers;
using Ultrapowa_Clash_Server_GUI.Logic;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    //Packet 24303
    class AllianceJoinOkMessage : Message
    {

        public AllianceJoinOkMessage(Client client) : base(client)
        {
            SetMessageType(24303);
        }

        public override void Encode()
        {
            List<Byte> pack = new List<Byte>();
            SetData(pack.ToArray());
        }
    }
}
