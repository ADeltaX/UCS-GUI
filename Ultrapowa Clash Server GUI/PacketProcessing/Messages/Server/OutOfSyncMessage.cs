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
    //Packet 24104
    class OutOfSyncMessage : Message
    {
        public OutOfSyncMessage(Client client) : base(client) 
        {
            SetMessageType(24104);
        }

        public override void Encode()
        {
            List<Byte> data = new List<Byte>();
            data.AddInt32(0);
            data.AddInt32(0);
            data.AddInt32(0);
            SetData(data.ToArray());
        }
    }
}
