using System.IO;
using Ultrapowa_Clash_Server_GUI.Core;
using Ultrapowa_Clash_Server_GUI.GameFiles;
using Ultrapowa_Clash_Server_GUI.Helpers;
using Ultrapowa_Clash_Server_GUI.Logic;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    //Commande 0x200
    internal class BuyDecoCommand : Command
    {
        public BuyDecoCommand(BinaryReader br)
        {
            X = br.ReadInt32WithEndian();
            Y = br.ReadInt32WithEndian();
            DecoId = br.ReadInt32WithEndian();
            Unknown1 = br.ReadUInt32WithEndian();
        }

        public int DecoId { get; set; }

        //00 00 02 00 00 00 00 22 00 00 00 1C 01 12 A8 81 00 00 0C 4F
        //01 12 A8 81
        public uint Unknown1 { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public override void Execute(Level level)
        {
            var ca = level.GetPlayerAvatar();

            var dd = (DecoData) ObjectManager.DataTables.GetDataById(DecoId);

            if (ca.HasEnoughResources(dd.GetBuildResource(), dd.GetBuildCost()))
            {
                var rd = dd.GetBuildResource();
                ca.CommodityCountChangeHelper(0, rd, -dd.GetBuildCost());

                var d = new Deco(dd, level);
                d.SetPositionXY(X, Y);
                level.GameObjectManager.AddGameObject(d);
            }
        }
    }
}