using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Ultrapowa_Clash_Server_GUI.Core;
using Ultrapowa_Clash_Server_GUI.Helpers;
using Ultrapowa_Clash_Server_GUI.Logic;
using Ultrapowa_Clash_Server_GUI.Network;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    //14303
    class AskForJoinableAlliancesListMessage : Message
    {
        private const int m_vAllianceLimit = 40;

        public AskForJoinableAlliancesListMessage(Client client, BinaryReader br) : base(client, br)
        {
            //Empty pack
        }

        public override void Decode()
        {

        }

        public override void Process(Level level)
        {
            var alliances = ObjectManager.GetInMemoryAlliances();
            List<Alliance> joinableAlliances = new List<Alliance>(); 
            int i=0;
            int j=0;
            while (j < m_vAllianceLimit && i < alliances.Count)
            {
                if (alliances[i].GetAllianceMembers().Count != 0 && !alliances[i].IsAllianceFull())
                {
                    joinableAlliances.Add(alliances[i]);
                    j++;
                } 
                i++;
            }
            joinableAlliances = joinableAlliances.ToList();

            var p = new JoinableAllianceListMessage(this.Client);
            p.SetJoinableAlliances(joinableAlliances);
            PacketManager.ProcessOutgoingPacket(p);
        }
    }
}
