using System.IO;
using Ultrapowa_Clash_Server_GUI.Helpers;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    //Commande 0x20C
    internal class ToggleAttackModeCommand : Command
    {
        public ToggleAttackModeCommand(BinaryReader br)
        {
            BuildingId = br.ReadUInt32WithEndian(); //buildingId - 0x1DCD6500;
            Unknown1 = br.ReadByte();
            Unknown2 = br.ReadUInt32WithEndian();
            Unknown3 = br.ReadUInt32WithEndian();
        }

        //00 00 3B CE

        //00 00 02 0C 1D CD 65 09 00 00 00 00 02 00 00 3B CE

        public uint BuildingId { get; set; }

        public byte Unknown1 { get; set; } //00

        public uint Unknown2 { get; set; } //00 00 00 02

        public uint Unknown3 { get; set; }
    }
}