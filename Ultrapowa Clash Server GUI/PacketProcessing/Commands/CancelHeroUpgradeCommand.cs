using System.IO;
using Ultrapowa_Clash_Server_GUI.Helpers;
using Ultrapowa_Clash_Server_GUI.Logic;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    //Commande 0x0213
    internal class CancelHeroUpgradeCommand : Command
    {
        private readonly int m_vBuildingId;

        public CancelHeroUpgradeCommand(BinaryReader br)
        {
            m_vBuildingId = br.ReadInt32WithEndian();
            br.ReadInt32WithEndian();
        }

        //00 00 02 13 1D CD 65 06 00 01 8B 0F
        public override void Execute(Level level)
        {
            var go = level.GameObjectManager.GetGameObjectByID(m_vBuildingId);
            if (go != null)
            {
                if (go.ClassId == 0)
                {
                    var b = (Building) go;
                    var hbc = b.GetHeroBaseComponent();
                    if (hbc != null)
                    {
                        hbc.CancelUpgrade();
                    }
                }
            }
        }
    }
}