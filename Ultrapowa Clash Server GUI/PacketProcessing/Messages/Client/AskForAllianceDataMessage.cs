using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Ultrapowa_Clash_Server_GUI.Core;
using Ultrapowa_Clash_Server_GUI.Helpers;
using Ultrapowa_Clash_Server_GUI.Network;
using Ultrapowa_Clash_Server_GUI.Logic;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    //14302
    class AskForAllianceDataMessage : Message
    {
        private long m_vAllianceId;

        public AskForAllianceDataMessage(Client client, BinaryReader br) : base(client, br)
        {
        }

        public override void Decode()
        {
            using (var br = new BinaryReader(new MemoryStream(GetData())))
            {
                m_vAllianceId = br.ReadInt64WithEndian();
            }
        }

        public override void Process(Level level)
        {
            Alliance alliance = ObjectManager.GetAlliance(m_vAllianceId);
            if (alliance != null)
            {
                PacketManager.ProcessOutgoingPacket(new AllianceDataMessage(this.Client, alliance));
            }
        }
    }
}
