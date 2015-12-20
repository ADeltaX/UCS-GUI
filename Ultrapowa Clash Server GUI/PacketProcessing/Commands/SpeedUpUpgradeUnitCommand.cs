using System.IO;
using Ultrapowa_Clash_Server_GUI.Helpers;
using Ultrapowa_Clash_Server_GUI.Logic;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    //Commande 0x205
    internal class SpeedUpUpgradeUnitCommand : Command
    {
        private readonly int m_vBuildingId;

        public SpeedUpUpgradeUnitCommand(BinaryReader br)
        {
            m_vBuildingId = br.ReadInt32WithEndian(); //buildingId - 0x1DCD6500;
            br.ReadInt32WithEndian();
        }

        //00 00 02 05 1D CD 65 13 00 00 53 8F

        public override void Execute(Level level)
        {
            var ca = level.GetPlayerAvatar();
            var go = level.GameObjectManager.GetGameObjectByID(m_vBuildingId);
            if (go != null)
            {
                if (go.ClassId == 0)
                {
                    var b = (Building) go;
                    var uuc = b.GetUnitUpgradeComponent();
                    if (uuc != null)
                    {
                        if (uuc.GetCurrentlyUpgradedUnit() != null)
                        {
                            uuc.SpeedUp();
                        }
                    }
                }
            }
        }
    }
}