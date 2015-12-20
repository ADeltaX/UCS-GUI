using System.IO;
using Ultrapowa_Clash_Server_GUI.Helpers;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    //Commande 0x0211
    internal class ToggleHeroSleepCommand : Command
    {
        public ToggleHeroSleepCommand(BinaryReader br)
        {
            BuildingId = br.ReadUInt32WithEndian(); //buildingId - 0x1DCD6500;
            FlagSleep = br.ReadByte();
            Unknown1 = br.ReadUInt32WithEndian();
        }

        //00 00 02 11 1D CD 65 06 00 00 01 04 CA

        public uint BuildingId { get; set; }

        public byte FlagSleep { get; set; }

        public uint Unknown1 { get; set; }
    }
}