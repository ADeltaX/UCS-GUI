using Newtonsoft.Json.Linq;
using System;
using Ultrapowa_Clash_Server_GUI.Core;
using Ultrapowa_Clash_Server_GUI.GameFiles;
using Ultrapowa_Clash_Server_GUI.Helpers;

namespace Ultrapowa_Clash_Server_GUI.Logic
{
    internal class HeroBaseComponent : Component
    {
        private readonly HeroData m_vHeroData;

        //a1 + 20
        //a1 + 24
        //a1 + 28
        private Timer m_vTimer;

        //a1 + 16
        private int m_vUpgradeLevelInProgress;

        public HeroBaseComponent(GameObject go, HeroData hd) : base(go)
        {
            m_vHeroData = hd;
        }

        public override int Type
        {
            get { return 10; }
        }

        //a1 + 12
        public void CancelUpgrade()
        {
            if (m_vTimer != null)
            {
                var ca = GetParent().GetLevel().GetPlayerAvatar();
                var currentLevel = ca.GetUnitUpgradeLevel(m_vHeroData);
                var rd = m_vHeroData.GetUpgradeResource(currentLevel);
                var cost = m_vHeroData.GetUpgradeCost(currentLevel);
                var multiplier =
                    ObjectManager.DataTables.GetGlobals().GetGlobalData("HERO_UPGRADE_CANCEL_MULTIPLIER").NumberValue;
                var resourceCount = (int) ((cost*multiplier*(long) 1374389535) >> 32);
                resourceCount = Math.Max((resourceCount >> 5) + (resourceCount >> 31), 0);
                ca.CommodityCountChangeHelper(0, rd, resourceCount);
                GetParent().GetLevel().WorkerManager.DeallocateWorker(GetParent());

                //todo: setherostate (*(*v2 + 224))(v2, *(v1 + 16), 3);
                m_vTimer = null;
            }
        }

        public bool CanStartUpgrading()
        {
            var result = false;
            if (m_vTimer == null)
            {
                var currentLevel = GetParent().GetLevel().GetPlayerAvatar().GetUnitUpgradeLevel(m_vHeroData);
                if (!IsMaxLevel())
                {
                    var requiredThLevel = m_vHeroData.GetRequiredTownHallLevel(currentLevel + 1);
                    result = GetParent().GetLevel().GetPlayerAvatar().GetTownHallLevel() >= requiredThLevel;
                }
            }
            return result;
        }

        public void FinishUpgrading()
        {
            var ca = GetParent().GetLevel().GetPlayerAvatar();
            var currentLevel = ca.GetUnitUpgradeLevel(m_vHeroData);
            ca.SetUnitUpgradeLevel(m_vHeroData, currentLevel + 1);

            //(*(*v3 + 160))(v3, 1, *(v1 + 16), 1);
            GetParent().GetLevel().WorkerManager.DeallocateWorker(GetParent());

            //SetHeroState (*(*v3 + 224))(v3, *(v1 + 16), 3);
            m_vTimer = null;
        }

        public int GetRemainingUpgradeSeconds()
        {
            return m_vTimer.GetRemainingSeconds(GetParent().GetLevel().GetTime());
        }

        public int GetTotalSeconds()
        {
            var currentLevel = GetParent().GetLevel().GetPlayerAvatar().GetUnitUpgradeLevel(m_vHeroData);
            return m_vHeroData.GetUpgradeTime(currentLevel);
        }

        public bool IsMaxLevel()
        {
            var ca = GetParent().GetLevel().GetPlayerAvatar();
            var currentLevel = ca.GetUnitUpgradeLevel(m_vHeroData);
            var maxLevel = m_vHeroData.GetUpgradeLevelCount() - 1;
            return currentLevel >= maxLevel;
        }

        public bool IsUpgrading()
        {
            return m_vTimer != null;
        }

        public override void Load(JObject jsonObject)
        {
            var unitUpgradeObject = (JObject) jsonObject["hero_upg"];
            if (unitUpgradeObject != null)
            {
                m_vTimer = new Timer();
                var remainingTime = unitUpgradeObject["t"].ToObject<int>();
                m_vTimer.StartTimer(remainingTime, GetParent().GetLevel().GetTime());

                m_vUpgradeLevelInProgress = unitUpgradeObject["level"].ToObject<int>();
            }
        }

        public override JObject Save(JObject jsonObject)
        {
            //{"data":1000022,"lvl":0,"x":21,"y":25,"hero_upg":{"level":21,"t":257639},"l1x":24,"l1y":21}

            if (m_vTimer != null)
            {
                var unitUpgradeObject = new JObject();
                unitUpgradeObject.Add("level", m_vUpgradeLevelInProgress);
                unitUpgradeObject.Add("t", m_vTimer.GetRemainingSeconds(GetParent().GetLevel().GetTime()));
                jsonObject.Add("hero_upg", unitUpgradeObject);
            }
            return jsonObject;
        }

        public void SpeedUpUpgrade()
        {
            var remainingSeconds = 0;
            if (IsUpgrading())
            {
                remainingSeconds = m_vTimer.GetRemainingSeconds(GetParent().GetLevel().GetTime());
            }
            var cost = GamePlayUtil.GetSpeedUpCost(remainingSeconds);
            var ca = GetParent().GetLevel().GetPlayerAvatar();
            if (ca.HasEnoughDiamonds(cost))
            {
                ca.UseDiamonds(cost);
                FinishUpgrading();
            }
        }

        public void StartUpgrading()
        {
            if (CanStartUpgrading())
            {
                GetParent().GetLevel().WorkerManager.AllocateWorker(GetParent());
                m_vTimer = new Timer();
                m_vTimer.StartTimer(GetTotalSeconds(), GetParent().GetLevel().GetTime());
                m_vUpgradeLevelInProgress = GetParent().GetLevel().GetPlayerAvatar().GetUnitUpgradeLevel(m_vHeroData) +
                                            1;

                //SetHeroState v27(v24, v26, 1);
                //Not 100% done
            }
        }

        public override void Tick()
        {
            if (m_vTimer != null)
            {
                if (m_vTimer.GetRemainingSeconds(GetParent().GetLevel().GetTime()) <= 0)
                {
                    FinishUpgrading();
                }
            }
        }
    }
}