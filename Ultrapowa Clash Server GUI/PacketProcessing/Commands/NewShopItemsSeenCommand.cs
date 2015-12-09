using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Ultrapowa_Clash_Server_GUI.Logic;
using Ultrapowa_Clash_Server_GUI.Helpers;
using Ultrapowa_Clash_Server_GUI.GameFiles;
using Ultrapowa_Clash_Server_GUI.Core;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    //Commande 0x0214
    class NewShopItemsSeenCommand : Command
    {
        public NewShopItemsSeenCommand(BinaryReader br)
        {
            uint NewShopItemNumber = br.ReadUInt32WithEndian();
            uint Unknown1 = br.ReadUInt32WithEndian();
            uint Unknown2 = br.ReadUInt32WithEndian();
            uint Unknown3 = br.ReadUInt32WithEndian();
        }

        //00 00 02 14 00 00 00 00 00 00 00 00 00 00 00 00 00 01 02 27
        //00 00 02 14 00 00 00 06 00 00 00 00 00 00 00 00 00 01 02 27

        public uint NewShopItemNumber { get; set; }
        public uint Unknown1 { get; set; } 
        public uint Unknown2 { get; set; }
        public uint Unknown3 { get; set; } 
    }
}
