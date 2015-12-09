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
    //Commande 0x1FB 507
    class ClearObstacleCommand : Command
    {
        public ClearObstacleCommand(BinaryReader br)
        {
            ObstacleId = br.ReadUInt32WithEndian(); //ObstacleId - 0x1DFB2BC0;
            Unknown1 = br.ReadUInt32WithEndian();
        }

        public uint ObstacleId { get; set; }//1D FB 2B C1
        public uint Unknown1 { get; set; } //00 00 E1 83
    }
}
