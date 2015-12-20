using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Ultrapowa_Clash_Server_GUI.Core;
using Ultrapowa_Clash_Server_GUI.GameFiles;

namespace Ultrapowa_Clash_Server_GUI.Logic
{
    internal class UnitStorageComponent : Component
    {
        private readonly List<UnitSlot> m_vUnits;

        public bool IsSpellForge;

        //a1 + 12;
        //a1 + 16
        //a1 + 20
        private int m_vMaxCapacity;

        public UnitStorageComponent(GameObject go, int capacity) : base(go)
        {
            m_vUnits = new List<UnitSlot>();
            m_vMaxCapacity = capacity;
            SetStorageType(go);
        }

        public override int Type
        {
            get { return 0; }
        }

        public void AddUnit(CombatItemData cd)

        {
            AddUnitImpl(cd, -1);
        }

        public void AddUnitImpl(CombatItemData cd, int level)
        {
            if (CanAddUnit(cd))
            {
                var unitIndex = GetUnitTypeIndex(cd, level);
                if (unitIndex == -1)
                {
                    var us = new UnitSlot(cd, level, 1);
                    m_vUnits.Add(us);
                }
                else
                {
                    m_vUnits[unitIndex].Count++;
                }
                var ca = GetParent().GetLevel().GetPlayerAvatar();
                var unitCount = ca.GetUnitCount(cd);
                ca.SetUnitCount(cd, unitCount + 1);
            }
        }

        public bool CanAddUnit(CombatItemData cd)
        {
            var result = false;
            if (cd != null)
            {
                if (IsSpellForge)
                {
                    result = GetMaxCapacity() >= GetUsedCapacity() + cd.GetHousingSpace();
                }
                else
                {
                    var cm = GetParent().GetLevel().GetComponentManager();
                    var maxCapacity = cm.GetTotalMaxHousing(); //GetMaxCapacity();
                    var usedCapacity = cm.GetTotalUsedHousing(); //GetUsedCapacity();
                    var housingSpace = cd.GetHousingSpace();
                    if (GetUsedCapacity() < GetMaxCapacity())
                        result = maxCapacity >= usedCapacity + housingSpace;
                }
            }
            return result;
        }

        public int GetMaxCapacity()
        {
            return m_vMaxCapacity;
        }

        public int GetUnitCount(int index)
        {
            return m_vUnits[index].Count;
        }

        public int GetUnitCountByData(CombatItemData cd)
        {
            var count = 0;
            for (var i = 0; i < m_vUnits.Count; i++)
            {
                if (m_vUnits[i].UnitData == cd)
                    count += m_vUnits[i].Count;
            }
            return count;
        }

        public int GetUnitLevel(int index)
        {
            return m_vUnits[index].Level;
        }

        public CombatItemData GetUnitType(int index)
        {
            return m_vUnits[index].UnitData;
        }

        public int GetUnitTypeIndex(CombatItemData cd)
        {
            var index = -1;
            for (var i = 0; i < m_vUnits.Count; i++)
            {
                if (m_vUnits[i].UnitData == cd)
                {
                    index = i;
                    break;
                }
            }
            return index;
        }

        public int GetUnitTypeIndex(CombatItemData cd, int level)
        {
            var index = -1;
            for (var i = 0; i < m_vUnits.Count; i++)
            {
                if (m_vUnits[i].UnitData == cd)
                {
                    if (m_vUnits[i].Level == level)
                    {
                        index = i;
                        break;
                    }
                }
            }
            return index;
        }

        public int GetUsedCapacity()
        {
            var count = 0;
            if (m_vUnits.Count >= 1)
            {
                for (var i = 0; i < m_vUnits.Count; i++)
                {
                    var cnt = m_vUnits[i].Count;
                    var housingSpace = m_vUnits[i].UnitData.GetHousingSpace();
                    count += cnt*housingSpace;
                }
            }
            return count;
        }

        public override void Load(JObject jsonObject)
        {
            if (jsonObject["storage_type"] != null)
            {
                IsSpellForge = jsonObject["storage_type"].ToObject<int>() == 1;
            }
            else
            {
                IsSpellForge = false;
            }
            var unitArray = (JArray) jsonObject["units"];
            if (unitArray != null)
            {
                if (unitArray.Count > 0)
                {
                    foreach (JArray unitSlotArray in unitArray)
                    {
                        var id = unitSlotArray[0].ToObject<int>();
                        var cnt = unitSlotArray[1].ToObject<int>();
                        m_vUnits.Add(new UnitSlot((CombatItemData) ObjectManager.DataTables.GetDataById(id), -1, cnt));
                    }
                }
            }
        }

        public void RemoveUnits(CombatItemData cd, int count)
        {
            RemoveUnitsImpl(cd, -1, count);
        }

        public void RemoveUnitsImpl(CombatItemData cd, int level, int count)
        {
            var unitIndex = GetUnitTypeIndex(cd, level);
            if (unitIndex == -1)
            {
                //Do nothing, should be empty yet
            }
            else
            {
                var us = m_vUnits[unitIndex];
                if (us.Count <= count)
                {
                    m_vUnits.Remove(us);
                }
                else
                {
                    us.Count -= count;
                }
                var ca = GetParent().GetLevel().GetPlayerAvatar();
                var unitCount = ca.GetUnitCount(cd);
                ca.SetUnitCount(cd, unitCount - count);
            }
        }

        public override JObject Save(JObject jsonObject)
        {
            //{"data":1000000,"lvl":7,"x":10,"y":35,"units":[[4000005,11],[4000000,1],[4000001,1]],"storage_type":0,"l1x":10,"l1y":35}

            var unitJsonArray = new JArray();
            if (m_vUnits.Count > 0)
            {
                foreach (var unit in m_vUnits)
                {
                    var unitSlotJsonArray = new JArray();
                    unitSlotJsonArray.Add(unit.UnitData.GetGlobalID());
                    unitSlotJsonArray.Add(unit.Count);
                    unitJsonArray.Add(unitSlotJsonArray);
                }
            }
            jsonObject.Add("units", unitJsonArray);

            if (IsSpellForge)
                jsonObject.Add("storage_type", 1);
            else
                jsonObject.Add("storage_type", 0);

            return jsonObject;
        }

        public void SetMaxCapacity(int capacity)
        {
            m_vMaxCapacity = capacity;
        }

        public void SetStorageType(GameObject go)
        {
            var b = (Building) GetParent();
            var bd = b.GetBuildingData();
            IsSpellForge = bd.IsSpellForge();
        }
    }
}