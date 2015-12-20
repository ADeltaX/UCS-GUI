using System.IO;
using Ultrapowa_Clash_Server_GUI.Helpers;
using Ultrapowa_Clash_Server_GUI.Logic;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    //Commande 0x1F9
    internal class CancelConstructionCommand : Command
    {
        public CancelConstructionCommand(BinaryReader br)
        {
            BuildingId = br.ReadInt32WithEndian(); //buildingId - 0x1DCD6500;
            Unknown1 = br.ReadUInt32WithEndian();
        }

        public int BuildingId { get; set; }

        public uint Unknown1 { get; set; }

        public override void Execute(Level level)
        {
            var go = level.GameObjectManager.GetGameObjectByID(BuildingId);
            if (go != null)
            {
                if (go.ClassId == 0 || go.ClassId == 4)
                {
                    var constructionItem = (ConstructionItem) go;
                    if (constructionItem.IsConstructing())
                    {
                        constructionItem.CancelConstruction();
                    }
                }
                else if (go.ClassId == 3)
                {
                    var obstacle = (Obstacle) go;
                    if (obstacle.IsClearingOnGoing())
                    {
                        obstacle.CancelClearing();
                    }
                }
            }
        }
    }
}