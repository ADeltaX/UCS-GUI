using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Ultrapowa_Clash_Server_GUI.Logic;
using Ultrapowa_Clash_Server_GUI.Helpers;
using Ultrapowa_Clash_Server_GUI.GameFiles;
using Ultrapowa_Clash_Server_GUI.Core;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    //Commande 0x1F8
    class SpeedUpConstructionCommand : Command
    {
        private int m_vBuildingId;

        public SpeedUpConstructionCommand(BinaryReader br)
        {
            m_vBuildingId = br.ReadInt32WithEndian();
            br.ReadInt32WithEndian();
        }

        public override void Execute(Level level)
        {
            GameObject go = level.GameObjectManager.GetGameObjectByID(m_vBuildingId);
            if (go != null)
            {
                if (go.ClassId == 0 || go.ClassId == 4)
                {
                    ((ConstructionItem)go).SpeedUpConstruction();
                }
            }
        }
    }
}
