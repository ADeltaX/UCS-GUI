using System.IO;
using Ultrapowa_Clash_Server_GUI.Helpers;
using Ultrapowa_Clash_Server_GUI.Logic;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    //Commande 503
    internal class SellBuildingCommand : Command
    {
        private readonly int m_vBuildingId;

        public SellBuildingCommand(BinaryReader br)
        {
            m_vBuildingId = br.ReadInt32WithEndian();
            br.ReadUInt32WithEndian();
        }

        public override void Execute(Level level)
        {
            var ca = level.GetPlayerAvatar();
            var go = level.GameObjectManager.GetGameObjectByID(m_vBuildingId);

            if (go != null)
            {
                if (go.ClassId == 4)
                {
                    var t = (Trap) go;
                    var upgradeLevel = t.GetUpgradeLevel();
                    var rd = t.GetTrapData().GetBuildResource(upgradeLevel);
                    var sellPrice = t.GetTrapData().GetSellPrice(upgradeLevel);
                    ca.CommodityCountChangeHelper(0, rd, sellPrice);
                    level.GameObjectManager.RemoveGameObject(t);
                }
                else if (go.ClassId == 6)
                {
                    var d = (Deco) go;
                    var rd = d.GetDecoData().GetBuildResource();
                    var sellPrice = d.GetDecoData().GetSellPrice();
                    if (rd.PremiumCurrency)
                    {
                        ca.SetDiamonds(ca.GetDiamonds() + sellPrice);
                    }
                    else
                    {
                        ca.CommodityCountChangeHelper(0, rd, sellPrice);
                    }
                    level.GameObjectManager.RemoveGameObject(d);
                }
            }
        }
    }
}