using System.Collections.Generic;
using Ultrapowa_Clash_Server_GUI.Core;

namespace Ultrapowa_Clash_Server_GUI.GameFiles
{
    internal class BuildingData : ConstructionItemData
    {
        public BuildingData(CSVRow row, DataTable dt) : base(row, dt)
        {
            LoadData(this, GetType(), row);
        }

        public bool AirTargets { get; set; }

        public bool AltAirTargets { get; set; }

        public bool AltAttackMode { get; set; }

        public int AltAttackRange { get; set; }

        public List<string> AltBuildResource { get; set; }

        public int AlternatePickNewTargetDelay { get; set; }

        public bool AltGroundTargets { get; set; }

        public bool AltMultiTargets { get; set; }

        public int AltNumMultiTargets { get; set; }

        public int AmmoCost { get; set; }

        public int AmmoCount { get; set; }

        public string AmmoResource { get; set; }

        public string AOESpell { get; set; }

        public string AOESpellAlternate { get; set; }

        public string AppearEffect { get; set; }

        public List<string> AttackEffect { get; set; }

        public List<string> AttackEffect2 { get; set; }

        public string AttackEffectLv2 { get; set; }

        public string AttackEffectLv3 { get; set; }

        public int AttackRange { get; set; }

        public int AttackSpeed { get; set; }

        public List<int> BoostCost { get; set; }

        public List<int> BuildCost { get; set; }

        public string BuildingClass { get; set; }

        public int BuildingH { get; set; }

        public int BuildingW { get; set; }

        public List<string> BuildResource { get; set; }

        public List<int> BuildTimeD { get; set; }

        public List<int> BuildTimeH { get; set; }

        public List<int> BuildTimeM { get; set; }

        public List<int> BuildTimeS { get; set; }

        public bool Bunker { get; set; }

        public bool CanNotSellLast { get; set; }

        public List<int> Damage { get; set; }

        public int DamageLv2 { get; set; }

        public int DamageLv3 { get; set; }

        public int DamageMulti { get; set; }

        public int DamageRadius { get; set; }

        public string DefenderCharacter { get; set; }

        public int DefenderCount { get; set; }

        public int DefenderZ { get; set; }

        public string DestroyEffect { get; set; }

        public List<int> DestructionXP { get; set; }

        public string ExportName { get; set; }

        public string ExportNameBase { get; set; }

        public string ExportNameBaseNpc { get; set; }

        public string ExportNameBaseWar { get; set; }

        public string ExportNameBuildAnim { get; set; }

        public string ExportNameConstruction { get; set; }

        public string ExportNameDamaged { get; set; }

        public string ExportNameNpc { get; set; }

        public string ExportNameTriggered { get; set; }

        public bool ForgesMiniSpells { get; set; }

        public bool ForgesSpells { get; set; }

        public bool GroundTargets { get; set; }

        public int Height { get; set; }

        public string HeroType { get; set; }

        public bool Hidden { get; set; }

        public List<string> HitEffect { get; set; }

        public List<int> Hitpoints { get; set; }

        public List<int> HousingSpace { get; set; }

        public string Icon { get; set; }

        public bool IncreasingDamage { get; set; }

        public string InfoTID { get; set; }

        public bool IsHeroBarrack { get; set; }

        public string LoadAmmoEffect { get; set; }

        public bool Locked { get; set; }

        public int Lv2SwitchTime { get; set; }

        public int Lv3SwitchTime { get; set; }

        public List<int> MaxStoredDarkElixir { get; set; }

        public List<int> MaxStoredElixir { get; set; }

        public List<int> MaxStoredGold { get; set; }

        public List<int> MaxStoredWarDarkElixir { get; set; }

        public List<int> MaxStoredWarElixir { get; set; }

        public List<int> MaxStoredWarGold { get; set; }

        public int MinAttackRange { get; set; }

        public string NoAmmoEffect { get; set; }

        public string PickUpEffect { get; set; }

        public string PlacingEffect { get; set; }

        public string PreferredTarget { get; set; }

        public int PreferredTargetDamageMod { get; set; }

        public bool PreventsHealing { get; set; }

        public string ProducesResource { get; set; }

        public List<int> ProducesUnitsOfType { get; set; }

        public List<string> Projectile { get; set; }

        public bool PushBack { get; set; }

        public bool RandomHitPosition { get; set; }

        public List<int> RegenTime { get; set; }

        public List<int> ResourceIconLimit { get; set; }

        public List<int> ResourceMax { get; set; }

        public List<int> ResourcePerHour { get; set; }

        public int StrengthWeight { get; set; }

        public string SWF { get; set; }

        public string TID { get; set; }

        public string TID_Instructor { get; set; }

        public string ToggleAttackModeEffect { get; set; }

        public List<int> TownHallLevel { get; set; }

        public string TransitionEffectLv2 { get; set; }

        public string TransitionEffectLv3 { get; set; }

        public int TriggerRadius { get; set; }

        public List<int> UnitProduction { get; set; }

        public bool UpgradesUnits { get; set; }

        public bool WallCornerPieces { get; set; }

        public int Width { get; set; }

        public ResourceData GetAltBuildResource(int level)
        {
            return ObjectManager.DataTables.GetResourceByName(AltBuildResource[level]);
        }

        public override int GetBuildCost(int level)
        {
            return BuildCost[level];
        }

        public string GetBuildingClass()
        {
            return BuildingClass;
        }

        public override ResourceData GetBuildResource(int level)
        {
            return ObjectManager.DataTables.GetResourceByName(BuildResource[level]);
        }

        public override int GetConstructionTime(int level)
        {
            return BuildTimeS[level] + BuildTimeM[level]*60 + BuildTimeH[level]*60*60 + BuildTimeD[level]*60*60*24;

            //TODO: Add BuildTimeMultipier
        }

        public List<int> GetMaxStoredResourceCounts(int level)
        {
            var maxStoredResourceCounts = new List<int>();
            var resourceDataTable = ObjectManager.DataTables.GetTable(2);
            for (var i = 0; i < resourceDataTable.GetItemCount(); i++)
            {
                var value = 0;
                var resourceData = (ResourceData) resourceDataTable.GetItemAt(i);
                var propertyName = "MaxStored" + resourceData.GetName();
                if (GetType().GetProperty(propertyName) != null)
                {
                    var obj = GetType().GetProperty(propertyName).GetValue(this, null);
                    value = ((List<int>) obj)[level];
                }
                maxStoredResourceCounts.Add(value);
            }
            return maxStoredResourceCounts;
        }

        public override int GetRequiredTownHallLevel(int level)
        {
            return TownHallLevel[level] - 1;

            //-1 à ajouter obligatoirement (checké il est retranché au moment de l'init client)
        }

        public int GetUnitProduction(int level)
        {
            return UnitProduction[level];
        }

        public int GetUnitStorageCapacity(int level)
        {
            return HousingSpace[level];
        }

        public override int GetUpgradeLevelCount()
        {
            return BuildCost.Count;
        }

        public bool IsSpellForge()
        {
            return ForgesSpells;
            return ForgesMiniSpells;
        }

        public override bool IsTownHall()
        {
            return BuildingClass == "Town Hall";
        }

        public bool IsWorkerBuilding()
        {
            return BuildingClass == "Worker";
        }
    }
}