using System.IO;
using Ultrapowa_Clash_Server_GUI.GameFiles;
using Ultrapowa_Clash_Server_GUI.Helpers;
using Ultrapowa_Clash_Server_GUI.Logic;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    //Commande 0x204
    internal class UpgradeUnitCommand : Command
    {
        public UpgradeUnitCommand(BinaryReader br)
        {
            BuildingId = br.ReadInt32WithEndian(); //buildingId - 0x1DCD6500;
            Unknown1 = br.ReadUInt32WithEndian();
            UnitData = (CombatItemData) br.ReadDataReference(); //.ReadInt32WithEndian();
            Unknown2 = br.ReadUInt32WithEndian();
        }

        public int BuildingId { get; set; }

        //00 00 02 04 1D CD 65 13 00 00 00 00 00 3D 09 00 00 00 51 E9
        public CombatItemData UnitData { get; set; }

        public uint Unknown1 { get; set; }

        //00 3D 09 00
        public uint Unknown2 { get; set; }

        public override void Execute(Level level)
        {
            var ca = level.GetPlayerAvatar();
            var go = level.GameObjectManager.GetGameObjectByID(BuildingId);
            var b = (Building) go;
            var uuc = b.GetUnitUpgradeComponent();
            var unitLevel = ca.GetUnitUpgradeLevel(UnitData);
            if (uuc.CanStartUpgrading(UnitData))
            {
                var cost = UnitData.GetUpgradeCost(unitLevel);
                var rd = UnitData.GetUpgradeResource(unitLevel);
                if (ca.HasEnoughResources(rd, cost))
                {
                    ca.SetResourceCount(rd, ca.GetResourceCount(rd) - cost);
                    uuc.StartUpgrading(UnitData);
                }
            }
        }

        //00 00 00 00
    }
}