using System.IO;
using Ultrapowa_Clash_Server_GUI.Core;
using Ultrapowa_Clash_Server_GUI.Helpers;
using Ultrapowa_Clash_Server_GUI.Logic;
using Ultrapowa_Clash_Server_GUI.Network;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    //Packet 14113
    internal class VisitHomeMessage : Message
    {
        public VisitHomeMessage(Client client, BinaryReader br)
            : base(client, br)
        {
        }

        public long AvatarId { get; set; }

        public override void Decode()
        {
            using (var br = new BinaryReader(new MemoryStream(GetData())))
            {
                AvatarId = br.ReadInt64WithEndian();
            }
        }

        public override void Process(Level level)
        {
            var targetLevel = ResourcesManager.GetPlayer(AvatarId);
            targetLevel.Tick();

            //Clan clan;
            PacketManager.ProcessOutgoingPacket(new VisitedHomeDataMessage(Client, targetLevel, level));

            //if (clan != null)
            //    PacketHandler.ProcessOutgoingPacket(new ServerAllianceChatHistory(this.Client, clan));
        }
    }
}