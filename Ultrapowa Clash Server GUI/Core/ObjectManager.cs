using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using Ultrapowa_Clash_Server_GUI.GameFiles;
using Ultrapowa_Clash_Server_GUI.Logic;
using Timer = System.Threading.Timer;

namespace Ultrapowa_Clash_Server_GUI.Core

{
    internal class ObjectManager
    {
        private static readonly object m_vDatabaseLock = new object();

        private static Dictionary<long, Alliance> m_vAlliances;

        private static long m_vAllianceSeed;

        private static long m_vAvatarSeed;

        private static string m_vHomeDefault;

        private static Random m_vRandomSeed;

        public bool m_vTimerCanceled;

        public Timer TimerReferenceA;

        public Timer TimerReferenceP;

        public ObjectManager()
        {
            m_vTimerCanceled = false;
            NpcLevels = new Dictionary<int, string>();
            DataTables = new DataTables();
            m_vAlliances = new Dictionary<long, Alliance>();

            if (Convert.ToBoolean(ConfigurationManager.AppSettings["useCustomPatch"]))
            {
                LoadFingerPrint();
            }

            using (var sr = new StreamReader(@"gamefiles/starting_home.json"))
            {
                m_vHomeDefault = sr.ReadToEnd();
            }

            m_vAvatarSeed = DatabaseManager.Singelton.GetMaxPlayerId() + 1;
            m_vAllianceSeed = DatabaseManager.Singelton.GetMaxAllianceId() + 1;
            LoadGameFiles();
            LoadNpcLevels();
            GetAllAlliancesFromDB();

            TimerCallback TimerDelegateA = SaveAlliance;
            var TimerAlliance = new Timer(TimerDelegateA, null, 60000, 60000);
            TimerReferenceA = TimerAlliance;

            var saving = int.Parse(ConfigurationManager.AppSettings["savingInterval"]);

            TimerCallback TimerDelegateP = SavePlayer;
            var TimerPlayer = new Timer(TimerDelegateP, null, saving, saving);
            TimerReferenceP = TimerPlayer;

            MainWindow.RemoteWindow.WriteConsoleDebug("Database synchronized!", (int)MainWindow.level.DEBUGLOG);
            m_vRandomSeed = new Random();
        }

        //public static ConcurrentDictionary<Client, Level> OnlinePlayers { get; set; }
        //public static ConcurrentDictionary<Level, Client> OnlineClients { get; set; }
        //public static ConcurrentDictionary<Socket, Client> Clients { get; set; }
        public static DataTables DataTables { get; set; }
        
        public static FingerPrint FingerPrint { get; set; }

        public static Dictionary<int, string> NpcLevels { get; set; }

        public static Alliance CreateAlliance(long seed)
        {
            Alliance alliance;
            lock (m_vDatabaseLock)
            {
                if (seed == 0)
                    seed = m_vAllianceSeed;
                alliance = new Alliance(seed);
                m_vAllianceSeed++;
            }
            DatabaseManager.Singelton.CreateAlliance(alliance);
            m_vAlliances.Add(alliance.GetAllianceId(), alliance);
            return alliance;
        }

        public static Level CreateAvatar(long seed)
        {
            Level pl;
            lock (m_vDatabaseLock)
            {
                if (seed == 0)
                    seed = m_vAvatarSeed;
                pl = new Level(seed);
                m_vAvatarSeed++;
            }
            pl.LoadFromJSON(m_vHomeDefault);
            DatabaseManager.Singelton.CreateAccount(pl);
            return pl;
        }

        public static void GetAllAlliancesFromDB()
        {
            foreach(Alliance a in DatabaseManager.Singelton.GetAllAlliances())
            {
                if (!m_vAlliances.ContainsKey(a.GetAllianceId()))
                {
                    m_vAlliances.Add(a.GetAllianceId(), a);
                }
            }
        }

        public static Alliance GetAlliance(long allianceId)
        {
            Alliance alliance = null;
            if (m_vAlliances.ContainsKey(allianceId))
            {
                alliance = m_vAlliances[allianceId];
            }
            else
            {
                alliance = DatabaseManager.Singelton.GetAlliance(allianceId);
                if (alliance != null)
                {
                    m_vAlliances.Add(alliance.GetAllianceId(), alliance);
                }
            }
            return alliance;
        }

        public static List<Alliance> GetInMemoryAlliances()
        {
            var alliances = new List<Alliance>();
            alliances.AddRange(m_vAlliances.Values);
            return alliances;
        }

        public static Level GetRandomOnlinePlayer()
        {
            var index = m_vRandomSeed.Next(0, ResourcesManager.GetInMemoryLevels().Count); //accès concurrent KO
            return ResourcesManager.GetInMemoryLevels().ElementAt(index);
        }

        public static Level GetRandomPlayerFromAll()
        {
            var index = m_vRandomSeed.Next(0, ResourcesManager.GetAllPlayerIds().Count); //accès concurrent KO
            return ResourcesManager.GetPlayer(ResourcesManager.GetAllPlayerIds()[index]);
        }   

        public static void LoadFingerPrint()
        {
            FingerPrint = new FingerPrint(@"gamefiles/fingerprint.json");
        }

        public static void LoadGameFiles()
        {
            var gameFiles = new List<Tuple<string, string, int>>();
            gameFiles.Add(new Tuple<string, string, int>("Achievements", @"gamefiles/logic/achievements.csv", 22));
            gameFiles.Add(new Tuple<string, string, int>("Buildings", @"gamefiles/logic/buildings.csv", 0));
            gameFiles.Add(new Tuple<string, string, int>("Characters", @"gamefiles/logic/characters.csv", 3));
            gameFiles.Add(new Tuple<string, string, int>("Decos", @"gamefiles/logic/decos.csv", 17));
            gameFiles.Add(new Tuple<string, string, int>("Experience Levels", @"gamefiles/logic/experience_levels.csv",
                10));
            gameFiles.Add(new Tuple<string, string, int>("Globals", @"gamefiles/logic/globals.csv", 13));
            gameFiles.Add(new Tuple<string, string, int>("Heroes", @"gamefiles/logic/heroes.csv", 27));
            gameFiles.Add(new Tuple<string, string, int>("Leagues", @"gamefiles/logic/leagues.csv", 12));
            gameFiles.Add(new Tuple<string, string, int>("NPCs", @"gamefiles/logic/npcs.csv", 16));
            gameFiles.Add(new Tuple<string, string, int>("Obstacles", @"gamefiles/logic/obstacles.csv", 7));
            gameFiles.Add(new Tuple<string, string, int>("Shields", @"gamefiles/logic/shields.csv", 19));
            gameFiles.Add(new Tuple<string, string, int>("Spells", @"gamefiles/logic/spells.csv", 25));
            gameFiles.Add(new Tuple<string, string, int>("Townhall Levels", @"gamefiles/logic/townhall_levels.csv", 14));
            gameFiles.Add(new Tuple<string, string, int>("Traps", @"gamefiles/logic/traps.csv", 11));
            gameFiles.Add(new Tuple<string, string, int>("Resources", @"gamefiles/logic/resources.csv", 2));
            gameFiles.Add(new Tuple<string, string, int>("Wars", @"gamefiles/logic/war.csv", 1));
            var dataCount = 0;
            Console.WriteLine("ObjectManager: Loading gamefiles...");
            foreach (var data in gameFiles)
            {
                Console.Write("\t" + data.Item1);
                dataCount++;
                DataTables.InitDataTable(new CSVTable(data.Item2), data.Item3);
                Console.WriteLine(" done");
            }
            Console.WriteLine("ObjectManager: " + dataCount + " objects successfully loaded on " +
                              ConfigurationManager.AppSettings["programThreadCount"] + " thread!");
        }

        public static void LoadNpcLevels()
        {
            if (Convert.ToBoolean(ConfigurationManager.AppSettings["expertPve"]))
            {
                for (var i = 0; i < 50; i++)
                {
                    using (var sr = new StreamReader(@"gamefiles/pve/expertPve/level" + (i + 1) + ".json"))
                    {
                        NpcLevels.Add(i, sr.ReadToEnd());
                    }
                }
            }
            else
            {
                for (var i = 0; i < 50; i++)
                {
                    using (var sr = new StreamReader(@"gamefiles/pve/normalPve/level" + (i + 1) + ".json"))
                    {
                        NpcLevels.Add(i, sr.ReadToEnd());
                    }
                }
            }
        }

        private void SaveAlliance(object state)
        {
            DatabaseManager.Singelton.Save(m_vAlliances.Values.ToList());
            if (m_vTimerCanceled)
            {
                TimerReferenceA.Dispose();
            }
        }
            private void SavePlayer(object state)
        {
            DatabaseManager.Singelton.Save(ResourcesManager.GetInMemoryLevels());
            if (m_vTimerCanceled)
            {
                TimerReferenceP.Dispose();
            }
        }
    }
}