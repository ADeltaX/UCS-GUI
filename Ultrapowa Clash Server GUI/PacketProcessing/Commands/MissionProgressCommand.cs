using System.IO;
using Ultrapowa_Clash_Server_GUI.Helpers;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    //Commande 0x207
    internal class MissionProgressCommand : Command
    {
        public MissionProgressCommand(BinaryReader br)
        {
            MissionId = br.ReadUInt32WithEndian(); //missionId - 0x1406F40;
            Unknown1 = br.ReadUInt32WithEndian();
        }

        //00 00 2D 7F some client tick

        //00 00 02 07 01 40 6F 4C 00 00 03 53

        public uint MissionId { get; set; }

        public uint Unknown1 { get; set; }
    }
}