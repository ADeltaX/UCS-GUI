using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Ultrapowa_Clash_Server_GUI.Helpers;
using Ultrapowa_Clash_Server_GUI.Logic;
using Ultrapowa_Clash_Server_GUI.Network;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    //Packet 10108
    class KeepAliveMessage : Message
    {
        public KeepAliveMessage(Client client, BinaryReader br)
            : base(client, br)
        {
        }

        public override void Decode()
        {
        }

        public override void Process(Level level)
        {
            PacketManager.ProcessOutgoingPacket(new KeepAliveOkMessage(this.Client,this));
        }
    }
}
