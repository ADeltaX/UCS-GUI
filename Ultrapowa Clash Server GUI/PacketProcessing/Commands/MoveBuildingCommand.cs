using System.IO;
using Ultrapowa_Clash_Server_GUI.Core;
using Ultrapowa_Clash_Server_GUI.Helpers;
using Ultrapowa_Clash_Server_GUI.Logic;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    //Commande 0x1F5
    internal class MoveBuildingCommand : Command
    {
        public MoveBuildingCommand(BinaryReader br)
        {
            X = br.ReadInt32WithEndian();
            Y = br.ReadInt32WithEndian();
            BuildingId = br.ReadInt32WithEndian(); //buildingId - 0x1DCD6500;
            Unknown1 = br.ReadUInt32WithEndian();
        }

        public int BuildingId { get; set; }

        //00 00 00 1F
        //00 00 2D 7F some client tick
        //30/08/2014 18:51;S;14102(0);32;00 00 2D BE 01 EB 32 0C 00 00 00 01 00 00 01 F5 00 00 00 13 00 00 00 1F 1D CD 65 06 00 00 2D 7F
        //1D CD 65 06 some unique id
        public uint Unknown1 { get; set; }

        public int X { get; set; }

        //00 00 00 13
        public int Y { get; set; }

        public override void Execute(Level level)
        {
            var go = level.GameObjectManager.GetGameObjectByID(BuildingId);
            Debugger.WriteLine(string.Format("X: {0} Y: {1}", X, Y));
            go.SetPositionXY(X, Y);
        }
    }
}