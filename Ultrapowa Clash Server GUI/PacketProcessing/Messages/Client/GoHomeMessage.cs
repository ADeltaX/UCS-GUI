using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Configuration;
using Ultrapowa_Clash_Server_GUI.Helpers;
using Ultrapowa_Clash_Server_GUI.Core;
using Ultrapowa_Clash_Server_GUI.Network;
using Ultrapowa_Clash_Server_GUI.Logic;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    //Packet 14101
    class GoHomeMessage : Message
    {
        public GoHomeMessage(Client client, BinaryReader br) : base(client, br)
        {
        }

        public override void Decode()
        { 
        }

        public override void Process(Level level)
        {
            level.Tick();

            Alliance alliance = ObjectManager.GetAlliance(level.GetPlayerAvatar().GetAllianceId());
            //player.GetPlayerAvatar().Clean();
            PacketManager.ProcessOutgoingPacket(new OwnHomeDataMessage(this.Client, level));
            if (alliance != null)
                PacketManager.ProcessOutgoingPacket(new AllianceStreamMessage(this.Client, alliance));
        }
    }
}
