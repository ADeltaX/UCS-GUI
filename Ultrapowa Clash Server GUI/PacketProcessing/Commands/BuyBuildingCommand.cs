using System.IO;
using Ultrapowa_Clash_Server_GUI.Core;
using Ultrapowa_Clash_Server_GUI.GameFiles;
using Ultrapowa_Clash_Server_GUI.Helpers;
using Ultrapowa_Clash_Server_GUI.Logic;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    //Commande 0x1F4
    internal class BuyBuildingCommand : Command
    {
        public BuyBuildingCommand(BinaryReader br)
        {
            X = br.ReadInt32WithEndian();
            Y = br.ReadInt32WithEndian();
            BuildingId = br.ReadInt32WithEndian();
            Unknown1 = br.ReadUInt32WithEndian();
        }

        public int BuildingId { get; set; }

        //00 00 2D 7F some client tick
        public uint Unknown1 { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public override void Execute(Level level)
        {
            var ca = level.GetPlayerAvatar();

            var bd = (BuildingData) ObjectManager.DataTables.GetDataById(BuildingId);
            var b = new Building(bd, level);

            if (ca.HasEnoughResources(bd.GetBuildResource(0), bd.GetBuildCost(0)))
            {
                if (bd.IsWorkerBuilding() || level.HasFreeWorkers())
                {
                    //Ajouter un check sur le réservoir d'élixir noir
                    var rd = bd.GetBuildResource(0);
                    ca.CommodityCountChangeHelper(0, rd, -bd.GetBuildCost(0));

                    b.StartConstructing(X, Y);
                    level.GameObjectManager.AddGameObject(b);
                }
            }
        }
    }
}