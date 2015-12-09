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
    //Commande 0x202
    class SpeedUpClearingCommand : Command
    {
        private int m_vObstacleId;

        public SpeedUpClearingCommand(BinaryReader br)
        {
            m_vObstacleId = br.ReadInt32WithEndian();
            br.ReadInt32WithEndian();
        }

        public override void Execute(Level level)
        {
            GameObject go = level.GameObjectManager.GetGameObjectByID(m_vObstacleId);
            if(go != null)
            {
                if(go.ClassId == 3)
                    ((Obstacle)go).SpeedUpClearing();
            }
        }
    }
}
