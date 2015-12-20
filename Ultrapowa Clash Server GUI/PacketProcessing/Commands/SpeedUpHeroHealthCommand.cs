using System.IO;
using Ultrapowa_Clash_Server_GUI.Helpers;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    //Commande 0x0212
    internal class SpeedUpHeroHealthCommand : Command
    {
        private int m_vBuildingId;

        public SpeedUpHeroHealthCommand(BinaryReader br)
        {
            m_vBuildingId = br.ReadInt32WithEndian();
            br.ReadInt32WithEndian();
        }
    }
}