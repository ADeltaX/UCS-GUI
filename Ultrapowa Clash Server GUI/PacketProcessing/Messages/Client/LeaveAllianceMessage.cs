using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Ultrapowa_Clash_Server_GUI.Helpers;
using Ultrapowa_Clash_Server_GUI.Network;
using Ultrapowa_Clash_Server_GUI.Logic;
using Ultrapowa_Clash_Server_GUI.Core;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    //Packet 14308
    class LeaveAllianceMessage : Message
    {
        public LeaveAllianceMessage(Client client, BinaryReader br) : base (client, br)
        {
        }

        public override void Decode()
        {
        }

        public override void Process(Level level)
        {
            Alliance alliance = ObjectManager.GetAlliance(level.GetPlayerAvatar().GetAllianceId());
            level.GetPlayerAvatar().SetAllianceId(0);
            alliance.RemoveMember(level.GetPlayerAvatar().GetId());
            //envoyer message départ à tous les membres
            //si chef nommer un nouveau chef
            //if alliance member count = 0, supprimer alliance
            PacketManager.ProcessOutgoingPacket(new LeaveAllianceOkMessage(this.Client, alliance));
        }
    }
}
