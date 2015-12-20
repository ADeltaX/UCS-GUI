using System.IO;
using Ultrapowa_Clash_Server_GUI.Core;
using Ultrapowa_Clash_Server_GUI.GameFiles;
using Ultrapowa_Clash_Server_GUI.Helpers;
using Ultrapowa_Clash_Server_GUI.Logic;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    //Commande 0x1FE
    internal class BuyTrapCommand : Command
    {
        public BuyTrapCommand(BinaryReader br)
        {
            X = br.ReadInt32WithEndian();
            Y = br.ReadInt32WithEndian();
            TrapId = br.ReadInt32WithEndian();
            Unknown1 = br.ReadUInt32WithEndian();
        }

        public int TrapId { get; set; }

        //00 00 01 FE 00 00 00 02 00 00 00 28 00 B7 1B 02 00 00 06 56
        public uint Unknown1 { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public override void Execute(Level level)
        {
            var ca = level.GetPlayerAvatar();

            var td = (TrapData) ObjectManager.DataTables.GetDataById(TrapId);
            var t = new Trap(td, level);

            if (ca.HasEnoughResources(td.GetBuildResource(0), td.GetBuildCost(0)))
            {
                if (level.HasFreeWorkers())
                {
                    var rd = td.GetBuildResource(0);
                    ca.CommodityCountChangeHelper(0, rd, -td.GetBuildCost(0));

                    t.StartConstructing(X, Y);
                    level.GameObjectManager.AddGameObject(t);
                }
            }
        }
    }
}