using System;
using System.IO;
using Ultrapowa_Clash_Server_GUI.Core;
using Ultrapowa_Clash_Server_GUI.Helpers;
using Ultrapowa_Clash_Server_GUI.Logic;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    //Commande 0x201
    internal class SpeedUpTrainingCommand : Command
    {
        private readonly int m_vBuildingId;

        public SpeedUpTrainingCommand(BinaryReader br)
        {
            m_vBuildingId = br.ReadInt32WithEndian();
            br.ReadInt32WithEndian();
        }

        //00 00 02 01 1D CD 65 10 00 00 38 A6

        public override void Execute(Level level)
        {
            var ca = level.GetPlayerAvatar();
            var go = level.GameObjectManager.GetGameObjectByID(m_vBuildingId);

            if (go != null)
            {
                if (go.ClassId == 0)
                {
                    var b = (Building) go;
                    var upc = b.GetUnitProductionComponent();
                    if (upc != null)
                    {
                        var totalRemainingTime = upc.GetTotalRemainingSeconds();
                        var cost = GamePlayUtil.GetSpeedUpCost(totalRemainingTime);
                        if (upc.IsSpellForge())
                        {
                            var multiplier =
                                ObjectManager.DataTables.GetGlobals()
                                    .GetGlobalData("SPELL_SPEED_UP_COST_MULTIPLIER")
                                    .NumberValue;
                            cost = (int) ((cost*(long) multiplier*1374389535) >> 32);
                            cost = Math.Max((cost >> 5) + (cost >> 31), 1);
                        }
                        if (ca.HasEnoughDiamonds(cost))
                        {
                            if (upc.HasHousingSpaceForSpeedUp())
                            {
                                ca.UseDiamonds(cost);
                                upc.SpeedUp();
                            }
                        }
                    }
                }
            }
        }
    }
}