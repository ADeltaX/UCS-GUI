using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Ultrapowa_Clash_Server_GUI.Core;
using Ultrapowa_Clash_Server_GUI.GameFiles;

namespace Ultrapowa_Clash_Server_GUI.Logic
{
    internal class GameObjectManager
    {
        private readonly ComponentManager m_vComponentManager;

        private readonly List<GameObject> m_vGameObjectRemoveList;

        private readonly List<List<GameObject>> m_vGameObjects;

        private readonly List<int> m_vGameObjectsIndex;

        private readonly Level m_vLevel;

        private readonly ObstacleManager m_vObstacleManager;

        public GameObjectManager(Level l)
        {
            m_vLevel = l;
            m_vGameObjects = new List<List<GameObject>>();
            m_vGameObjectRemoveList = new List<GameObject>();
            m_vGameObjectsIndex = new List<int>();
            for (var i = 0; i < 7; i++)
            {
                m_vGameObjects.Add(new List<GameObject>());
                m_vGameObjectsIndex.Add(0);
            }
            m_vComponentManager = new ComponentManager(m_vLevel);
            m_vObstacleManager = new ObstacleManager(m_vLevel);
        }

        public void AddGameObject(GameObject go)
        {
            go.GlobalId = GenerateGameObjectGlobalId(go);
            if (go.ClassId == 0)
            {
                var b = (Building) go;
                var bd = b.GetBuildingData();
                if (bd.IsWorkerBuilding())
                {
                    m_vLevel.WorkerManager.IncreaseWorkerCount();
                }
            }
            m_vGameObjects[go.ClassId].Add(go);
        }

        public List<List<GameObject>> GetAllGameObjects()
        {
            return m_vGameObjects;
        }

        public ComponentManager GetComponentManager()
        {
            return m_vComponentManager;
        }

        public GameObject GetGameObjectByID(int id)
        {
            var classId = GlobalID.GetClassID(id) - 500;
            return m_vGameObjects[classId].Find(g => g.GlobalId == id);
        }

        public List<GameObject> GetGameObjects(int id)
        {
            return m_vGameObjects[id];
        }

        public ObstacleManager GetObstacleManager()
        {
            return m_vObstacleManager;
        }

        public void Load(JObject jsonObject)
        {
            var jsonBuildings = (JArray) jsonObject["buildings"];
            foreach (JObject jsonBuilding in jsonBuildings)
            {
                var bd = (BuildingData) ObjectManager.DataTables.GetDataById(jsonBuilding["data"].ToObject<int>());
                var b = new Building(bd, m_vLevel);
                AddGameObject(b);
                b.Load(jsonBuilding);
            }

            var jsonTraps = (JArray) jsonObject["traps"];
            foreach (JObject jsonTrap in jsonTraps)
            {
                var td = (TrapData) ObjectManager.DataTables.GetDataById(jsonTrap["data"].ToObject<int>());
                var t = new Trap(td, m_vLevel);
                AddGameObject(t);
                t.Load(jsonTrap);
            }

            var jsonDecos = (JArray) jsonObject["decos"];
            foreach (JObject jsonDeco in jsonDecos)
            {
                var dd = (DecoData) ObjectManager.DataTables.GetDataById(jsonDeco["data"].ToObject<int>());
                var d = new Deco(dd, m_vLevel);
                AddGameObject(d);
                d.Load(jsonDeco);
            }

            var jsonObstacles = (JArray) jsonObject["obstacles"];
            foreach (JObject jsonObstacle in jsonObstacles)
            {
                var dd = (ObstacleData) ObjectManager.DataTables.GetDataById(jsonObstacle["data"].ToObject<int>());
                var d = new Obstacle(dd, m_vLevel);
                AddGameObject(d);
                d.Load(jsonObstacle);
            }

            m_vObstacleManager.Load(jsonObject);
        }

        public void RemoveGameObject(GameObject go)
        {
            m_vGameObjectRemoveList.Add(go);
        }

        public JObject Save()
        {
            m_vObstacleManager.Tick();

            var jsonData = new JObject();

            //Buildings
            var jsonBuildingsArray = new JArray();
            foreach (var go in new List<GameObject>(m_vGameObjects[0]))
            {
                var b = (Building) go;
                var jsonObject = new JObject();
                jsonObject.Add("data", b.GetBuildingData().GetGlobalID());
                b.Save(jsonObject);
                jsonBuildingsArray.Add(jsonObject);
            }
            jsonData.Add("buildings", jsonBuildingsArray);

            //Traps
            var jsonTrapsArray = new JArray();
            foreach (var go in new List<GameObject>(m_vGameObjects[4]))
            {
                var t = (Trap) go;
                var jsonObject = new JObject();
                jsonObject.Add("data", t.GetTrapData().GetGlobalID());
                t.Save(jsonObject);
                jsonTrapsArray.Add(jsonObject);
            }
            jsonData.Add("traps", jsonTrapsArray);

            //Decos
            var jsonDecosArray = new JArray();
            foreach (var go in new List<GameObject>(m_vGameObjects[6]))
            {
                var d = (Deco) go;
                var jsonObject = new JObject();
                jsonObject.Add("data", d.GetDecoData().GetGlobalID());
                d.Save(jsonObject);
                jsonDecosArray.Add(jsonObject);
            }
            jsonData.Add("decos", jsonDecosArray);

            //Obstacles
            var jsonobstaclesArray = new JArray();
            foreach (var go in new List<GameObject>(m_vGameObjects[3]))
            {
                var o = (Obstacle) go;
                var jsonObject = new JObject();
                jsonObject.Add("data", o.GetObstacleData().GetGlobalID());
                o.Save(jsonObject);
                jsonobstaclesArray.Add(jsonObject);
            }
            jsonData.Add("obstacles", jsonobstaclesArray);

            m_vObstacleManager.Save(jsonData);

            return jsonData;
        }

        public void Tick()
        {
            m_vComponentManager.Tick();
            foreach (var l in m_vGameObjects)
            {
                foreach (var go in l)
                    go.Tick();
            }
            foreach (var g in new List<GameObject>(m_vGameObjectRemoveList))
            {
                RemoveGameObjectTotally(g);
                m_vGameObjectRemoveList.Remove(g);
            }
        }

        private int GenerateGameObjectGlobalId(GameObject go)
        {
            var index = m_vGameObjectsIndex[go.ClassId];
            m_vGameObjectsIndex[go.ClassId]++;
            return GlobalID.CreateGlobalID(go.ClassId + 500, index);
        }

        private void RemoveGameObjectReferences(GameObject go)
        {
            m_vComponentManager.RemoveGameObjectReferences(go);
        }

        private void RemoveGameObjectTotally(GameObject go)
        {
            m_vGameObjects[go.ClassId].Remove(go);
            if (go.ClassId == 0)
            {
                var b = (Building) go;
                var bd = b.GetBuildingData();
                if (bd.IsWorkerBuilding())
                {
                    m_vLevel.WorkerManager.DecreaseWorkerCount();
                }
            }
            RemoveGameObjectReferences(go);
        }
    }
}