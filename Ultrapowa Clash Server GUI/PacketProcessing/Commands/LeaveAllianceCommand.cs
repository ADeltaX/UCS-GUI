using System.Collections.Generic;
using System.IO;
using Ultrapowa_Clash_Server_GUI.Helpers;
using Ultrapowa_Clash_Server_GUI.Logic;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    //Commande 0x0002
    internal class LeaveAllianceCommand : Command
    {
        private Alliance m_vAlliance;
        private int m_vReason;

        public LeaveAllianceCommand()
        {
            //AvailableServerCommandMessage
        }

        public LeaveAllianceCommand(BinaryReader br)
        {
            br.ReadInt64WithEndian();
            br.ReadInt32WithEndian();
            br.ReadInt32WithEndian();
        }

        public override byte[] Encode()
        {
            var data = new List<byte>();
            data.AddInt64(m_vAlliance.GetAllianceId());
            data.AddInt32(m_vReason);
            data.AddInt32(-1);
            return data.ToArray();
        }

        public void SetAlliance(Alliance alliance)
        {
            m_vAlliance = alliance;
        }

        public void SetReason(int reason)
        {
            m_vReason = reason;
        }

        //00 00 07 3A
        //00 00 00 01 ////reason? 1= leave, 2=kick

        //00 00 00 3B 00 0A 40 1E
    }
}