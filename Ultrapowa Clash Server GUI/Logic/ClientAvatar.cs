using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Ultrapowa_Clash_Server_GUI.Core;
using Ultrapowa_Clash_Server_GUI.GameFiles;
using Ultrapowa_Clash_Server_GUI.Helpers;

namespace Ultrapowa_Clash_Server_GUI.Logic
{
    internal class ClientAvatar : Avatar
    {
        private long m_vAllianceId;

        private int m_vAvatarLevel;

        private string m_vAvatarName;

        private int m_vCurrentGems;

        private long m_vCurrentHomeId;

        private int m_vExperience;

        private long m_vId;

        private byte m_vIsAvatarNameSet;

        private int m_vLeagueId;

        private int m_vScore;

        public ClientAvatar()
        {
            var rleague = new Random();
            Achievements = new List<DataSlot>();
            AllianceUnits = new List<DataSlot>();
            NpcStars = new List<DataSlot>();
            NpcLootedGold = new List<DataSlot>();
            NpcLootedElixir = new List<DataSlot>();
            m_vLeagueId = 9;
        }

        public ClientAvatar(long id) : this()
        {
            var rnd = new Random();
            LastUpdate = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            Login = id + ((int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds).ToString();
            m_vId = id;
            m_vCurrentHomeId = id;
            m_vIsAvatarNameSet = 0x00;
            m_vAvatarLevel = Convert.ToInt32(ConfigurationManager.AppSettings["startingLevel"]);
            m_vAllianceId = 0;

            // Starting experience 
            if (ConfigurationManager.AppSettings["startingExperience"] == "random")
                m_vExperience = rnd.Next(100, 500000);
            else
                m_vExperience = Convert.ToInt32(ConfigurationManager.AppSettings["startingExperience"]);

            // Starting shield time 
            EndShieldTime =
                (int)
                    (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds +
                     Convert.ToInt32(ConfigurationManager.AppSettings["startingShieldTime"]));

            // Starting gems 
            if (ConfigurationManager.AppSettings["startingGems"] == "random")
                m_vCurrentGems = rnd.Next(100000, 999999);
            else
                m_vCurrentGems = Convert.ToInt32(ConfigurationManager.AppSettings["startingGems"]);

            // Starting score 
            if (ConfigurationManager.AppSettings["startingTrophies"] == "random")
                m_vScore = rnd.Next(500, 4000);
            else
                m_vScore = Convert.ToInt32(ConfigurationManager.AppSettings["startingTrophies"]);
            TutorialStepsCount = 0x0A;
            m_vAvatarName = "NoNameYet";

            // Gold 
            if (ConfigurationManager.AppSettings["startingGold"] == "random")
                SetResourceCount(ObjectManager.DataTables.GetResourceByName("Gold"), rnd.Next(1000, 10000000));
            else
                SetResourceCount(ObjectManager.DataTables.GetResourceByName("Gold"),
                    Convert.ToInt32(ConfigurationManager.AppSettings["startingGold"]));

            // Elixir 
            if (ConfigurationManager.AppSettings["startingElixir"] == "random")
                SetResourceCount(ObjectManager.DataTables.GetResourceByName("Elixir"), rnd.Next(1000, 10000000));
            else
                SetResourceCount(ObjectManager.DataTables.GetResourceByName("Elixir"),
                    Convert.ToInt32(ConfigurationManager.AppSettings["startingElixir"]));

            // Dark Elixir 
            if (ConfigurationManager.AppSettings["startingDarkElixir"] == "random")
                SetResourceCount(ObjectManager.DataTables.GetResourceByName("DarkElixir"), rnd.Next(1000, 1000000));
            else
                SetResourceCount(ObjectManager.DataTables.GetResourceByName("DarkElixir"),
                    Convert.ToInt32(ConfigurationManager.AppSettings["startingDarkElixir"]));

            // Starting gems 
            if (ConfigurationManager.AppSettings["startingGems"] == "random")
                SetResourceCount(ObjectManager.DataTables.GetResourceByName("Gems"), m_vCurrentGems);
            else
                SetResourceCount(ObjectManager.DataTables.GetResourceByName("Diamonds"),
                    Convert.ToInt32(ConfigurationManager.AppSettings["startingGems"]));
        }

        public List<DataSlot> Achievements { get; set; }

        public List<DataSlot> AllianceUnits { get; set; }

        public int EndShieldTime { get; set; }

        public int LastUpdate { get; set; }

        public string Login { get; set; }

        public List<DataSlot> NpcLootedElixir { get; set; }

        public List<DataSlot> NpcLootedGold { get; set; }

        public List<DataSlot> NpcStars { get; set; }

        public int RemainingShieldTime
        {
            get
            {
                var rest = EndShieldTime - (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
                return rest > 0 ? rest : 0;
            }
        }

        public uint TutorialStepsCount { get; set; }

        //public uint Region { get; set; }
        public Village Village { get; set; }

        /// <summary>
        ///     Adds Diamonds.
        /// </summary>
        /// <param name="diamondCount"></param>
        public void AddDiamonds(int diamondCount)
        {
            m_vCurrentGems += diamondCount;
        }

        /// <summary>
        ///     Adds experience and increase level on level up.
        /// </summary>
        /// <param name="exp"> experience to add. </param>
        public void AddExperience(int exp)
        {
            m_vExperience += exp;
            var experienceCap =
                ((ExperienceLevelData)ObjectManager.DataTables.GetTable(10).GetDataByName(m_vAvatarLevel.ToString()))
                    .ExpPoints;
            if (m_vExperience >= experienceCap)
            {
                if (ObjectManager.DataTables.GetTable(10).GetItemCount() > m_vAvatarLevel + 1)
                {
                    m_vAvatarLevel += 1;
                    m_vExperience = m_vExperience - experienceCap;
                }
                else
                {
                    m_vExperience = 0;
                }
            }
        }

        public byte[] Encode()
        {
            var data = new List<byte>();

            data.AddInt32(0);
            data.AddInt64(m_vId);
            data.AddInt64(m_vCurrentHomeId);
            if (m_vAllianceId != 0)
            {
                data.Add(1);
                data.AddInt64(m_vAllianceId);
                var alliance = ObjectManager.GetAlliance(m_vAllianceId);
                data.AddString(alliance.GetAllianceName());
                data.AddInt32(alliance.GetAllianceBadgeData());
                data.AddInt32(alliance.GetAllianceMember(m_vId).GetRole());
                data.AddInt32(alliance.GetAllianceLevel());
                data.Add(0);
                data.AddInt32(0);
            }
            else
            {
                data.Add(0);
                data.AddInt32(0);
            }

            //7.156
            data.AddInt32(0);
            data.AddInt32(0);
            data.AddInt32(0);
            data.AddInt32(0);
            data.AddInt32(0);
            data.AddInt32(0);
            data.AddInt32(0);
            data.AddInt32(0);
            data.AddInt32(0);
            data.AddInt32(0);
            data.AddInt32(m_vLeagueId); //league

            data.AddInt32(GetAllianceCastleLevel());
            data.AddInt32(GetAllianceCastleTotalCapacity());
            data.AddInt32(GetAllianceCastleUsedCapacity());
            data.AddInt32(GetTownHallLevel());
            data.AddString(m_vAvatarName);
            data.AddInt32(-1);
            data.AddInt32(m_vAvatarLevel);
            data.AddInt32(m_vExperience);
            data.AddInt32(m_vCurrentGems);
            data.AddInt32(m_vCurrentGems); //FreeDiamonds
            data.AddInt32(0x04B0);
            data.AddInt32(0x003C);
            data.AddInt32(m_vScore);

            data.AddRange(new byte[]
            {
                0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00,
                0x01,
                0x00, 0x00, 0x00, 0xDC, 0x6C, 0xF5, 0xEB, 0x48
            });

            data.Add(m_vIsAvatarNameSet);
            data.AddInt32(0); //Cumulative Purchased Diamonds
            data.AddInt32(0);

            //7.65
            data.AddInt32(0);

            //7.1
            data.AddInt32(1);

            data.AddDataSlots(GetResourceCaps());
            data.AddDataSlots(GetResources());
            data.AddDataSlots(GetUnits());
            data.AddDataSlots(GetSpells());
            data.AddDataSlots(m_vUnitUpgradeLevel);
            data.AddDataSlots(m_vSpellUpgradeLevel);
            data.AddDataSlots(m_vHeroUpgradeLevel);
            data.AddDataSlots(m_vHeroHealth);
            data.AddDataSlots(m_vHeroState);

            data.AddRange(BitConverter.GetBytes(AllianceUnits.Count).Reverse());
            foreach (var u in AllianceUnits)
            {
                data.AddRange(BitConverter.GetBytes(u.Data.GetGlobalID()).Reverse());
                data.AddRange(BitConverter.GetBytes(u.Value).Reverse());
                data.AddRange(BitConverter.GetBytes(0).Reverse()); //A CHANGER
            }

            data.AddRange(BitConverter.GetBytes(TutorialStepsCount).Reverse());
            for (uint i = 0; i < TutorialStepsCount; i++)
                data.AddRange(BitConverter.GetBytes(0x01406F40 + i).Reverse());

            //Unlocked Achievements
            data.AddRange(BitConverter.GetBytes(Achievements.Count).Reverse());
            foreach (var a in Achievements)
            {
                data.AddRange(BitConverter.GetBytes(a.Data.GetGlobalID()).Reverse());
            }

            //Achievement Progress
            data.AddRange(BitConverter.GetBytes(Achievements.Count).Reverse());
            foreach (var a in Achievements)
            {
                data.AddRange(BitConverter.GetBytes(a.Data.GetGlobalID()).Reverse());
                data.AddRange(BitConverter.GetBytes(0).Reverse()); //A CHANGER
            }

            data.AddDataSlots(NpcStars);
            data.AddDataSlots(NpcLootedGold);
            data.AddDataSlots(NpcLootedElixir);

            //7.65
            data.AddInt32(0);

            return data.ToArray();
        }

        public long GetAllianceId()
        {
            return m_vAllianceId;
        }

        public AllianceMemberEntry GetAllianceMemberEntry()
        {
            var alliance = ObjectManager.GetAlliance(m_vAllianceId);
            if (alliance != null)
            {
                return alliance.GetAllianceMember(m_vId);
            }
            return null;
        }

        public int GetAllianceRole()
        {
            var ame = GetAllianceMemberEntry();
            if (ame != null)
            {
                return ame.GetRole();
            }
            return -1;
        }

        public int GetAvatarLevel()
        {
            return m_vAvatarLevel;
        }

        public string GetAvatarName()
        {
            return m_vAvatarName;
        }

        public long GetCurrentHomeId()
        {
            return m_vCurrentHomeId;
        }

        public int GetDiamonds()
        {
            return m_vCurrentGems;
        }

        public int GetExperience()
        {
            return m_vExperience;
        }

        public long GetId()
        {
            return m_vId;
        }

        public int GetLeagueId()
        {
            return m_vLeagueId;
        }

        public int GetScore()
        {
            return m_vScore;
        }

        public int GetSecondsFromLastUpdate()
        {
            return (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds - LastUpdate;
        }

        public bool HasEnoughDiamonds(int diamondCount)
        {
            return m_vCurrentGems >= diamondCount;
        }

        public bool HasEnoughResources(ResourceData rd, int buildCost)
        {
            return GetResourceCount(rd) >= buildCost;
        }

        public void LoadFromJSON(string jsonString)
        {
            var jsonObject = JObject.Parse(jsonString);

            m_vId = jsonObject["avatar_id"].ToObject<long>();
            m_vCurrentHomeId = jsonObject["current_home_id"].ToObject<long>();
            m_vAllianceId = jsonObject["alliance_id"].ToObject<long>();
            SetAllianceCastleLevel(jsonObject["alliance_castle_level"].ToObject<int>());
            SetAllianceCastleTotalCapacity(jsonObject["alliance_castle_total_capacity"].ToObject<int>());
            SetAllianceCastleUsedCapacity(jsonObject["alliance_castle_used_capacity"].ToObject<int>());
            SetTownHallLevel(jsonObject["townhall_level"].ToObject<int>());
            m_vAvatarName = jsonObject["avatar_name"].ToObject<string>();
            m_vAvatarLevel = jsonObject["avatar_level"].ToObject<int>();
            m_vExperience = jsonObject["experience"].ToObject<int>();
            m_vCurrentGems = jsonObject["current_gems"].ToObject<int>();
            SetScore(jsonObject["score"].ToObject<int>());
            m_vIsAvatarNameSet = jsonObject["is_avatar_name_set"].ToObject<byte>();

            /*JArray jsonMaxResources = (JArray)jsonObject["max_resources"];
            foreach (JObject resource in jsonMaxResources)
            {
                var ds = new DataSlot(null, 0);
                ds.Load(resource);
                m_vResourceCaps.Add(ds);
            }*/

            var jsonResources = (JArray)jsonObject["resources"];
            foreach (JObject resource in jsonResources)
            {
                var ds = new DataSlot(null, 0);
                ds.Load(resource);
                GetResources().Add(ds);
            }

            var jsonUnits = (JArray)jsonObject["units"];
            foreach (JObject unit in jsonUnits)
            {
                var ds = new DataSlot(null, 0);
                ds.Load(unit);
                m_vUnitCount.Add(ds);
            }

            var jsonSpells = (JArray)jsonObject["spells"];
            foreach (JObject spell in jsonSpells)
            {
                var ds = new DataSlot(null, 0);
                ds.Load(spell);
                m_vSpellCount.Add(ds);
            }

            var jsonUnitLevels = (JArray)jsonObject["unit_upgrade_levels"];
            foreach (JObject unitLevel in jsonUnitLevels)
            {
                var ds = new DataSlot(null, 0);
                ds.Load(unitLevel);
                m_vUnitUpgradeLevel.Add(ds);
            }

            var jsonSpellLevels = (JArray)jsonObject["spell_upgrade_levels"];
            foreach (JObject data in jsonSpellLevels)
            {
                var ds = new DataSlot(null, 0);
                ds.Load(data);
                m_vSpellUpgradeLevel.Add(ds);
            }

            var jsonHeroLevels = (JArray)jsonObject["hero_upgrade_levels"];
            foreach (JObject data in jsonHeroLevels)
            {
                var ds = new DataSlot(null, 0);
                ds.Load(data);
                m_vHeroUpgradeLevel.Add(ds);
            }

            var jsonHeroHealth = (JArray)jsonObject["hero_health"];
            foreach (JObject data in jsonHeroHealth)
            {
                var ds = new DataSlot(null, 0);
                ds.Load(data);
                m_vHeroHealth.Add(ds);
            }

            var jsonHeroState = (JArray)jsonObject["hero_state"];
            foreach (JObject data in jsonHeroState)
            {
                var ds = new DataSlot(null, 0);
                ds.Load(data);
                m_vHeroState.Add(ds);
            }

            var jsonAllianceUnits = (JArray)jsonObject["alliance_units"];
            foreach (JObject data in jsonAllianceUnits)
            {
                var ds = new DataSlot(null, 0);
                ds.Load(data);
                AllianceUnits.Add(ds);
            }

            TutorialStepsCount = jsonObject["tutorial_step"].ToObject<uint>();

            /*JArray jsonUnlockedAchievements = (JArray)jsonObject["unlocked_achievements"];
            foreach (JObject data in jsonUnlockedAchievements)
            {
                var ds = new DataSlot(null, 0);
                ds.Load(data);
                Achievements.Add(ds);
            };*/

            var jsonAchievementsProgress = (JArray)jsonObject["achievements_progress"];
            foreach (JObject data in jsonAchievementsProgress)
            {
                var ds = new DataSlot(null, 0);
                ds.Load(data);
                Achievements.Add(ds);
            }

            var jsonNpcStars = (JArray)jsonObject["npc_stars"];
            foreach (JObject data in jsonNpcStars)
            {
                var ds = new DataSlot(null, 0);
                ds.Load(data);
                NpcStars.Add(ds);
            }

            var jsonNpcLootedGold = (JArray)jsonObject["npc_looted_gold"];
            foreach (JObject data in jsonNpcLootedGold)
            {
                var ds = new DataSlot(null, 0);
                ds.Load(data);
                NpcLootedGold.Add(ds);
            }

            var jsonNpcLootedElixir = (JArray)jsonObject["npc_looted_elixir"];
            foreach (JObject data in jsonNpcLootedElixir)
            {
                var ds = new DataSlot(null, 0);
                ds.Load(data);
                NpcLootedElixir.Add(ds);
            }
        }

        public string SaveToJSON()
        {
            var jsonData = new JObject();

            jsonData.Add("avatar_id", m_vId);
            jsonData.Add("current_home_id", m_vCurrentHomeId);
            jsonData.Add("alliance_id", m_vAllianceId);
            jsonData.Add("alliance_castle_level", GetAllianceCastleLevel());
            jsonData.Add("alliance_castle_total_capacity", GetAllianceCastleTotalCapacity());
            jsonData.Add("alliance_castle_used_capacity", GetAllianceCastleUsedCapacity());
            jsonData.Add("townhall_level", GetTownHallLevel());
            jsonData.Add("avatar_name", m_vAvatarName);
            jsonData.Add("avatar_level", m_vAvatarLevel);
            jsonData.Add("experience", m_vExperience);
            jsonData.Add("current_gems", m_vCurrentGems);
            jsonData.Add("score", m_vScore);
            jsonData.Add("is_avatar_name_set", m_vIsAvatarNameSet);

            /*JArray jsonResourceCapsArray = new JArray();
            foreach (var resource in GetResourceCaps())
                jsonResourceCapsArray.Add(resource.Save(new JObject()));
            jsonData.Add("max_resources", jsonResourceCapsArray);*/

            var jsonResourcesArray = new JArray();
            foreach (var resource in GetResources())
                jsonResourcesArray.Add(resource.Save(new JObject()));
            jsonData.Add("resources", jsonResourcesArray);

            var jsonUnitsArray = new JArray();
            foreach (var unit in GetUnits())
                jsonUnitsArray.Add(unit.Save(new JObject()));
            jsonData.Add("units", jsonUnitsArray);

            var jsonSpellsArray = new JArray();
            foreach (var spell in GetSpells())
                jsonSpellsArray.Add(spell.Save(new JObject()));
            jsonData.Add("spells", jsonSpellsArray);

            var jsonUnitUpgradeLevelsArray = new JArray();
            foreach (var unitUpgradeLevel in m_vUnitUpgradeLevel)
                jsonUnitUpgradeLevelsArray.Add(unitUpgradeLevel.Save(new JObject()));
            jsonData.Add("unit_upgrade_levels", jsonUnitUpgradeLevelsArray);

            var jsonSpellUpgradeLevelsArray = new JArray();
            foreach (var spellUpgradeLevel in m_vSpellUpgradeLevel)
                jsonSpellUpgradeLevelsArray.Add(spellUpgradeLevel.Save(new JObject()));
            jsonData.Add("spell_upgrade_levels", jsonSpellUpgradeLevelsArray);

            var jsonHeroUpgradeLevelsArray = new JArray();
            foreach (var heroUpgradeLevel in m_vHeroUpgradeLevel)
                jsonHeroUpgradeLevelsArray.Add(heroUpgradeLevel.Save(new JObject()));
            jsonData.Add("hero_upgrade_levels", jsonHeroUpgradeLevelsArray);

            var jsonHeroHealthArray = new JArray();
            foreach (var heroHealth in m_vHeroHealth)
                jsonHeroHealthArray.Add(heroHealth.Save(new JObject()));
            jsonData.Add("hero_health", jsonHeroHealthArray);

            var jsonHeroStateArray = new JArray();
            foreach (var heroState in m_vHeroState)
                jsonHeroStateArray.Add(heroState.Save(new JObject()));
            jsonData.Add("hero_state", jsonHeroStateArray);

            var jsonAllianceUnitsArray = new JArray();
            foreach (var allianceUnit in AllianceUnits)
                jsonAllianceUnitsArray.Add(allianceUnit.Save(new JObject()));
            jsonData.Add("alliance_units", jsonAllianceUnitsArray);

            jsonData.Add("tutorial_step", TutorialStepsCount);

            /* JArray jsonAchievementsArray = new JArray();
             foreach (var achievement in Achievements)
             {
                 JObject jsonObject = new JObject();
                 jsonObject.Add("global_id", achievement.Data.GetGlobalID());
                 jsonAchievementsArray.Add(jsonObject);
             }
             jsonData.Add("unlocked_achievements", jsonAchievementsArray);*/

            var jsonAchievementsProgressArray = new JArray();
            foreach (var achievement in Achievements)
                jsonAchievementsProgressArray.Add(achievement.Save(new JObject()));
            jsonData.Add("achievements_progress", jsonAchievementsProgressArray);

            var jsonNpcStarsArray = new JArray();
            foreach (var npcLevel in NpcStars)
                jsonNpcStarsArray.Add(npcLevel.Save(new JObject()));
            jsonData.Add("npc_stars", jsonNpcStarsArray);

            var jsonNpcLootedGoldArray = new JArray();
            foreach (var npcLevel in NpcLootedGold)
                jsonNpcLootedGoldArray.Add(npcLevel.Save(new JObject()));
            jsonData.Add("npc_looted_gold", jsonNpcLootedGoldArray);

            var jsonNpcLootedElixirArray = new JArray();
            foreach (var npcLevel in NpcLootedElixir)
                jsonNpcLootedElixirArray.Add(npcLevel.Save(new JObject()));
            jsonData.Add("npc_looted_elixir", jsonNpcLootedElixirArray);

            return JsonConvert.SerializeObject(jsonData);
        }

        public void SetAchievment(AchievementData ad, bool finished)
        {
            var index = GetDataIndex(Achievements, ad);
            var value = finished ? 1 : 0;
            if (index != -1)
                Achievements[index].Value = value;
            else
            {
                var ds = new DataSlot(ad, value);
                Achievements.Add(ds);
            }
        }

        public void SetAllianceId(long id)
        {
            m_vAllianceId = id;
        }

        public void SetDiamonds(int count)
        {
            m_vCurrentGems = count;
        }

        public void SetExperience(int newExp)
        {
            m_vExperience = newExp;
        }

        public void SetLeagueId(int id)
        {
            m_vLeagueId = id;
        }

        public void SetName(string name)
        {
            m_vAvatarName = name;
            m_vIsAvatarNameSet = 0x01;
            TutorialStepsCount = 0x0D;
        }

        public void SetScore(int newScore)
        {
            m_vScore = newScore;
            updateLeague();
        }

        public void UseDiamonds(int diamondCount)
        {
            m_vCurrentGems -= diamondCount;
        }

        private void updateLeague()
        {
            var table = ObjectManager.DataTables.GetTable(12);
            var i = 0;
            var found = false;
            while (!found)
            {
                var league = (LeagueData)table.GetItemAt(i);

                if (m_vScore <= league.BucketPlacementRangeHigh[league.BucketPlacementRangeHigh.Count - 1] &&
                    m_vScore >= league.BucketPlacementRangeLow[0])
                {
                    found = true;
                    SetLeagueId(i);
                }

                i++;
            }
        }
    }
}