using System.IO;
using Ultrapowa_Clash_Server_GUI.Core;
using Ultrapowa_Clash_Server_GUI.Helpers;
using Ultrapowa_Clash_Server_GUI.Logic;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    //Commande 0x20D
    internal class LoadTurretCommand : Command
    {
        public LoadTurretCommand(BinaryReader br)
        {
            m_vUnknown1 = br.ReadUInt32WithEndian();
            m_vBuildingId = br.ReadInt32WithEndian(); //buildingId - 0x1DCD6500;
            m_vUnknown2 = br.ReadUInt32WithEndian();
            Debugger.WriteLine(string.Format("U1: {0}, BId {1}, U2: {2}", m_vUnknown1, m_vBuildingId, m_vUnknown2), null,
                5);
        }

        //00 00 02 0D 00 00 00 01 1D CD 65 03 00 00 01 DF

        public int m_vBuildingId { get; set; }

        public uint m_vUnknown1 { get; set; }

        public uint m_vUnknown2 { get; set; }

        public override void Execute(Level level)
        {
            var ca = level.GetPlayerAvatar();
            var go = level.GameObjectManager.GetGameObjectByID(m_vBuildingId);

            if (go != null)
            {
                if (go.GetComponent(1, true) != null)
                {
                    ((CombatComponent) go.GetComponent(1, true)).FillAmmo();
                }
            }
        }
    }
}