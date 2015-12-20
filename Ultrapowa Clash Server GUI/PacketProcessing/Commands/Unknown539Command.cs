using System.IO;
using Ultrapowa_Clash_Server_GUI.Helpers;
using Ultrapowa_Clash_Server_GUI.Logic;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    //Commande 0x21B
    internal class Unknown539Command : Command
    {
        public Unknown539Command(BinaryReader br)
        {
            Unknown1 = br.ReadUInt32WithEndian();
            Unknown2 = br.ReadUInt32WithEndian();
        }

        public uint Unknown1 { get; set; }

        //00 00 00 00
        //00 00 00 0C
        public uint Unknown2 { get; set; }

        public override void Execute(Level level)
        {
        }

        //00 00 00 02 00 00 02 1B 00 00 00 0C 00 00 00 00 00 00 02 1B 00 00 00 0D 00 00 00 00
    }
}