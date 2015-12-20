using System.IO;
using Ultrapowa_Clash_Server_GUI.Helpers;
using Ultrapowa_Clash_Server_GUI.Logic;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    //Commande 0x1FB 507
    internal class ClearObstacleCommand : Command
    {
        public ClearObstacleCommand(BinaryReader br)
        {
            ObstacleId = br.ReadInt32WithEndian(); //ObstacleId - 0x1DFB2BC0;
            Unknown1 = br.ReadUInt32WithEndian();
        }

        public int ObstacleId { get; set; }

        //00 00 E1 83
        //1D FB 2B C1
        public uint Unknown1 { get; set; }

        public override void Execute(Level level)
        {
            var ca = level.GetPlayerAvatar();
            var go = level.GameObjectManager.GetGameObjectByID(ObstacleId);

            var o = (Obstacle) go;
            var od = o.GetObstacleData();
            if (ca.HasEnoughResources(od.GetClearingResource(), od.ClearCost))
            {
                if (level.HasFreeWorkers())
                {
                    var rd = od.GetClearingResource();
                    ca.SetResourceCount(rd, ca.GetResourceCount(rd) - od.ClearCost);
                    o.StartClearing();
                }
            }
        }
    }
}