using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ultrapowa_Clash_Server_GUI.Logic;
using Ultrapowa_Clash_Server_GUI.Core;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    //Packet 20108
    class KeepAliveOkMessage : Message
    {
        public KeepAliveOkMessage(Client client, KeepAliveMessage cka)
            : base(client)
        {
            SetMessageType(20108);
        }

        public override void Encode()
        {
            List<Byte> data = new List<Byte>();
            SetData(data.ToArray());
        }
    }
}
