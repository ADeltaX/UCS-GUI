using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Data.Entity;
using Ultrapowa_Clash_Server_GUI.Database;
using Ultrapowa_Clash_Server_GUI.PacketProcessing;

namespace Ultrapowa_Clash_Server_GUI.Logic
{
    internal class Level
    {
        private readonly ClientAvatar m_vClientAvatar;

        public GameObjectManager GameObjectManager; //a1 + 44
        private byte m_vAccountPrivileges;
        private byte m_vAccountStatus;
        private Client m_vClient;
        private DateTime m_vTime;
        public WorkerManager WorkerManager;

        //a1 + 40
        //MissionManager
        //AchievementManager
        //CooldownManager

        public Level()
        {
            WorkerManager = new WorkerManager();
            GameObjectManager = new GameObjectManager(this);
            m_vClientAvatar = new ClientAvatar();
            m_vAccountPrivileges = 0;
            m_vAccountStatus = 0;
        }

        public Level(long id)
        {
            WorkerManager = new WorkerManager();
            GameObjectManager = new GameObjectManager(this);
            m_vClientAvatar = new ClientAvatar(id);
            m_vTime = DateTime.UtcNow;
            m_vAccountPrivileges = 0;
            m_vAccountStatus = 0;
        }

        public byte GetAccountPrivileges()
        {
            return m_vAccountPrivileges;
        }

        public byte GetAccountStatus()
        {
            return m_vAccountStatus;
        }

        public Client GetClient()
        {
            return m_vClient;
        }

        public ComponentManager GetComponentManager()
        {
            return GameObjectManager.GetComponentManager();
        }

        public ClientAvatar GetHomeOwnerAvatar()
        {
            return m_vClientAvatar;
        }

        public ClientAvatar GetPlayerAvatar()
        {
            return m_vClientAvatar;
        }

        public DateTime GetTime()
        {
            return m_vTime;
        }

        public bool HasFreeWorkers()
        {
            return WorkerManager.GetFreeWorkers() > 0;
        }

        public bool isPermittedUser()
        {
            return m_vAccountPrivileges > 0;
        }

        public void LoadFromJSON(string jsonString)
        {
            var jsonObject = JObject.Parse(jsonString);
            GameObjectManager.Load(jsonObject);
        }

        public ucsdbEntities SaveToDatabse(ucsdbEntities context)
        {
            var p = context.player.Find(GetPlayerAvatar().GetId());
            if (p != null)
            {
                p.LastUpdateTime = GetTime();
                p.AccountStatus = GetAccountStatus();
                p.AccountPrivileges = GetAccountPrivileges();
                p.Avatar = GetPlayerAvatar().SaveToJSON();
                p.GameObjects = SaveToJSON();
                context.Entry(p).State = EntityState.Modified;
            }
            else
            {
                context.player.Add(
                    new player
                    {
                        PlayerId = GetPlayerAvatar().GetId(),
                        AccountStatus = GetAccountStatus(),
                        AccountPrivileges = GetAccountPrivileges(),
                        LastUpdateTime = GetTime(),
                        Avatar = GetPlayerAvatar().SaveToJSON(),
                        GameObjects = SaveToJSON()
                    }
                    );
            }
            return context;
        }

        public string SaveToJSON()
        {
            return JsonConvert.SerializeObject(GameObjectManager.Save());
        }

        public void SetAccountPrivileges(byte privileges)
        {
            m_vAccountPrivileges = privileges;
        }

        public void SetAccountStatus(byte status)
        {
            m_vAccountStatus = status;
        }

        public void SetClient(Client client)
        {
            m_vClient = client;
        }

        public void SetHome(string jsonHome)
        {
            GameObjectManager.Load(JObject.Parse(jsonHome));
        }

        public void SetTime(DateTime t)
        {
            m_vTime = t;
        }

        public void Tick()
        {
            SetTime(DateTime.UtcNow);
            GameObjectManager.Tick();

            //LogicMissionManager::tick(*(v1 + 48));
            //LogicAchievementManager::tick(*(v1 + 52));
            //LogicCooldownManager::tick(*(v1 + 68));
        }
    }
}