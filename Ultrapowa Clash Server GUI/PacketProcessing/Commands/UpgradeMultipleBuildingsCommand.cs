using System.Collections.Generic;
using System.IO;
using Ultrapowa_Clash_Server_GUI.GameFiles;
using Ultrapowa_Clash_Server_GUI.Helpers;
using Ultrapowa_Clash_Server_GUI.Logic;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    //Commande 0x225
    internal class UpgradeMultipleBuildingsCommand : Command
    {
        private readonly List<int> m_vBuildingIdList;

        private readonly byte m_vIsAltResource;

        public UpgradeMultipleBuildingsCommand(BinaryReader br)
        {
            m_vIsAltResource = br.ReadByte();
            m_vBuildingIdList = new List<int>();
            var buildingCount = br.ReadInt32WithEndian();
            for (var i = 0; i < buildingCount; i++)
            {
                var buildingId = br.ReadInt32WithEndian(); //= buildingId - 0x1DCD6500;
                m_vBuildingIdList.Add(buildingId);
            }
            br.ReadInt32WithEndian();
        }

        //00 00 02 25 00 00 00 00 07 1D CD 65 0A 1D CD 65 09 1D CD 65 0B 1D CD 65 08 1D CD 65 0C 1D CD 65 07 1D CD 65 06 00 00 1B 07
        //public uint Unknown1 { get; set; } //00 00 2D 7F some client tick

        public override void Execute(Level level)
        {
            var ca = level.GetPlayerAvatar();

            foreach (var buildingId in m_vBuildingIdList)
            {
                var b = (Building) level.GameObjectManager.GetGameObjectByID(buildingId);
                if (b.CanUpgrade())
                {
                    var bd = b.GetBuildingData();
                    var cost = bd.GetBuildCost(b.GetUpgradeLevel() + 1);
                    ResourceData rd;
                    if (m_vIsAltResource == 0)
                        rd = bd.GetBuildResource(b.GetUpgradeLevel() + 1);
                    else
                        rd = bd.GetAltBuildResource(b.GetUpgradeLevel() + 1);
                    if (ca.HasEnoughResources(rd, cost))
                    {
                        if (level.HasFreeWorkers())
                        {
                            ca.SetResourceCount(rd, ca.GetResourceCount(rd) - cost);
                            b.StartUpgrading();
                        }
                    }
                }
            }
        }
    }
}