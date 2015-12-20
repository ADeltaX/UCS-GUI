using System.Collections.Generic;
using System.IO;
using Ultrapowa_Clash_Server_GUI.Helpers;
using Ultrapowa_Clash_Server_GUI.Logic;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    //Commande 0x001
    internal class JoinAllianceCommand : Command
    {
        private Alliance m_vAlliance;

        //00 00 00 0B

        //00 00 00 46 00 03 46 FE

        public JoinAllianceCommand()
        {
            //AvailableServerCommandMessage
        }

        public JoinAllianceCommand(BinaryReader br)
        {
            br.ReadInt64WithEndian();
            br.ReadScString();
            br.ReadInt32WithEndian();
            br.ReadByte();
            br.ReadInt32WithEndian();
            br.ReadInt32WithEndian();
            br.ReadInt32WithEndian();
        }

        public override byte[] Encode()
        {
            var data = new List<byte>();
            data.AddRange(m_vAlliance.EncodeHeader());
            return data.ToArray();
        }

        public void SetAlliance(Alliance alliance)
        {
            m_vAlliance = alliance;
        }

        //4C 61 20 54 65 61 6D 20 54 44 41
        //5E 00 2C 5A
        //00
        //00 00 00 02

        //00 00 00 01
        //00 00 1C 35
    }
}