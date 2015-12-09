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
    //Packet 14113
    class VisitHomeMessage : Message
    {
        public VisitHomeMessage(Client client, BinaryReader br)
            : base(client, br)
        {
        }

        public override void Decode()
        {
            using (var br = new BinaryReader(new MemoryStream(GetData())))
            {
                AvatarId = br.ReadInt64WithEndian();
            }
        }

        public long AvatarId { get; set; }

        public override void Process(Level level)
        {
            Level targetLevel = ResourcesManager.GetPlayer(AvatarId);
            targetLevel.Tick();
            //Clan clan;
            PacketManager.ProcessOutgoingPacket(new VisitedHomeDataMessage(this.Client, targetLevel, level));
            //if (clan != null)
            //    PacketHandler.ProcessOutgoingPacket(new ServerAllianceChatHistory(this.Client, clan));
        }
    }
}
