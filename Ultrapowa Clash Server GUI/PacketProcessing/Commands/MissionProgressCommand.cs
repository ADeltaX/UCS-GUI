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
    //Commande 0x207
    class MissionProgressCommand : Command
    {
        public MissionProgressCommand(BinaryReader br)
        {
            MissionId = br.ReadUInt32WithEndian(); //missionId - 0x1406F40;
            Unknown1 = br.ReadUInt32WithEndian();
        }

        //00 00 02 07 01 40 6F 4C 00 00 03 53

        public uint MissionId { get; set; } 
        public uint Unknown1 { get; set; } //00 00 2D 7F some client tick
    }
}
