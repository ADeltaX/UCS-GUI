using System.IO;
using Ultrapowa_Clash_Server_GUI.Core;
using Ultrapowa_Clash_Server_GUI.Logic;
using Ultrapowa_Clash_Server_GUI.Network;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    //Packet 14308
    internal class LeaveAllianceMessage : Message
    {
        public LeaveAllianceMessage(Client client, BinaryReader br) : base(client, br)
        {
        }

        public override void Decode()
        {
        }

        public override void Process(Level level)
        {
            var alliance = ObjectManager.GetAlliance(level.GetPlayerAvatar().GetAllianceId());
            level.GetPlayerAvatar().SetAllianceId(0);
            alliance.RemoveMember(level.GetPlayerAvatar().GetId());

            if (alliance.GetAllianceMembers().Count <= 0)
            {
                DatabaseManager.Singelton.RemoveAlliance(alliance);
            }

            // send messages to all members of departure if appoint a new head chef if member
            // alliance count = 0 , delete alliance
            PacketManager.ProcessOutgoingPacket(new LeaveAllianceOkMessage(Client, alliance));
        }
    }
}