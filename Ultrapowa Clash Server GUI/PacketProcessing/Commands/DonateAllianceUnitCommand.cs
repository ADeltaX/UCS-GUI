using System.IO;
using Ultrapowa_Clash_Server_GUI.Helpers;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    //Commande 0x04
    internal class DonateAllianceUnitCommand : Command
    {
        public DonateAllianceUnitCommand(BinaryReader br)
        {
            Unknown1 = br.ReadUInt32WithEndian();
            PlayerId = br.ReadUInt32WithEndian();
            UnitType = br.ReadUInt32WithEndian();
            Unknown2 = br.ReadUInt32WithEndian();
            Unknown3 = br.ReadUInt32WithEndian();
        }

        //00 00 48 7A

        //00 00 00 04 00 00 00 00 49 79 1C DD 00 3D 09 00 00 00 00 08 00 00 48 7A

        public uint PlayerId { get; set; }

        //49 79 1C DD
        public uint UnitType { get; set; }

        public uint Unknown1 { get; set; } //00 00 00 00

        //00 3D 09 00
        public uint Unknown2 { get; set; } //00 00 00 08

        public uint Unknown3 { get; set; }
    }
}