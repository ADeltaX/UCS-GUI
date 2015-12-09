using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Ultrapowa_Clash_Server_GUI.Helpers;
using Ultrapowa_Clash_Server_GUI.Logic;
using Ultrapowa_Clash_Server_GUI.Network;
using Ultrapowa_Clash_Server_GUI.Core;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    //Packet 14324
    class SearchAlliancesMessage : Message
    {
        private const int m_vAllianceLimit = 40;
        private string m_vSearchString;
        private int m_vWarFrequency;
        private int m_vAllianceOrigin;
        private int m_vMinimumAllianceMembers;
        private int m_vMaximumAllianceMembers;
        private int m_vAllianceScore;
        private byte m_vShowOnlyJoinableAlliances;
        private int m_vMinimumAllianceLevel;

        public SearchAlliancesMessage(Client client, BinaryReader br) : base (client, br)
        {
        }

        //00 00 00 03 
        //61 61 61
        //00 00 00 01 
        //00 00 00 00 
        //00 00 00 01 
        //00 00 00 29 
        //00 00 07 D0 
        //01 
        //00 00 00 00 //???
        //00 00 00 06

        public override void Decode()
        {
            using (var br = new BinaryReader(new MemoryStream(GetData())))
            {
                m_vSearchString = br.ReadScString();
                m_vWarFrequency = br.ReadInt32WithEndian();
                m_vAllianceOrigin = br.ReadInt32WithEndian();
                m_vMinimumAllianceMembers = br.ReadInt32WithEndian();
                m_vMaximumAllianceMembers = br.ReadInt32WithEndian();
                m_vAllianceScore = br.ReadInt32WithEndian();
                m_vShowOnlyJoinableAlliances = br.ReadByte();
                br.ReadInt32WithEndian();
                m_vMinimumAllianceLevel = br.ReadInt32WithEndian();
            }
        }

        public override void Process(Level level)
        {

            var alliances = ObjectManager.GetInMemoryAlliances();
            List<Alliance> joinableAlliances = new List<Alliance>();
            int i = 0;
            int j = 0;
            while (j < m_vAllianceLimit && i < alliances.Count)
            {
                if (alliances[i].GetAllianceMembers().Count != 0
                    && alliances[i].GetAllianceName().Contains(m_vSearchString))
                {
                    joinableAlliances.Add(alliances[i]);
                    j++;
                }
                i++;
            }
            joinableAlliances = joinableAlliances.ToList();

            var p = new AllianceListMessage(this.Client);
            p.SetAlliances(joinableAlliances);
            p.SetSearchString(m_vSearchString);
            PacketManager.ProcessOutgoingPacket(p);
        }
    }
}
