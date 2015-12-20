using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ultrapowa_Clash_Server_GUI.Database;
using Ultrapowa_Clash_Server_GUI.Logic;

namespace Ultrapowa_Clash_Server_GUI.Core
{
    internal static class LinqExtensions
    {
        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> list, int parts)
        {
            var i = 0;
            var splits = from item in list
                group item by i++%parts
                into part
                select part.AsEnumerable();
            return splits;
        }
    }

    internal class DatabaseManager
    {
        private static DatabaseManager singelton;
        public static DatabaseManager Singelton
        {
            get
            {
                if (singelton == null)
                {
                    singelton = new DatabaseManager();
                }
                return singelton;
            }
        }

        private readonly string m_vConnectionString;

        private readonly int saveThreadCount = 4;

        public DatabaseManager()
        {
            m_vConnectionString = ConfigurationManager.AppSettings["databaseConnectionName"];
        }

        public void CreateAccount(Level l)
        {
            try
            {
                MainWindow.RemoteWindow.WriteConsoleDebug("Saving new account to database (player id: " + l.GetPlayerAvatar().GetId() + ")",
                    (int)MainWindow.level.DEBUGLOG);
                using (var db = new ucsdbEntities(m_vConnectionString))
                {
                    db.player.Add(
                        new player
                        {
                            PlayerId = l.GetPlayerAvatar().GetId(),
                            AccountStatus = l.GetAccountStatus(),
                            AccountPrivileges = l.GetAccountPrivileges(),
                            LastUpdateTime = l.GetTime(),
                            Avatar = l.GetPlayerAvatar().SaveToJSON(),
                            GameObjects = l.SaveToJSON()
                        }
                        );
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                MainWindow.RemoteWindow.WriteConsoleDebug("An exception occured during CreateAccount processing: "+ ex, (int)MainWindow.level.DEBUGFATAL);
            }
        }

        public void CreateAlliance(Alliance a)
        {
            try
            {
                MainWindow.RemoteWindow.WriteConsoleDebug("Saving new Alliance to database (alliance id: " + a.GetAllianceId() + ")",(int)MainWindow.level.DEBUGLOG);
                using (var db = new ucsdbEntities(m_vConnectionString))
                {
                    db.clan.Add(
                        new clan
                        {
                            ClanId = a.GetAllianceId(),
                            LastUpdateTime = DateTime.Now,
                            Data = a.SaveToJSON()
                        }
                        );
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                MainWindow.RemoteWindow.WriteConsoleDebug("An exception occured during CreateAlliance processing: " + ex, (int)MainWindow.level.DEBUGFATAL);
            }
        }

        public Level GetAccount(long playerId)
        {
            Level account = null;
            try
            {
                using (var db = new ucsdbEntities(m_vConnectionString))
                {
                    var p = db.player.Find(playerId);
                    
                    if (p != null)
                    {
                        account = new Level();
                        account.SetAccountStatus(p.AccountStatus);
                        account.SetAccountPrivileges(p.AccountPrivileges);
                        account.SetTime(p.LastUpdateTime);
                        account.GetPlayerAvatar().LoadFromJSON(p.Avatar);
                        account.LoadFromJSON(p.GameObjects);
                    }
                }
            }
            catch (Exception ex)
            {
                Debugger.WriteLine("An exception occured during GetAccount processing:", ex, 0, ConsoleColor.DarkRed);
            }
            return account;
        }

        public List<Alliance> GetAllAlliances()
        {
            var alliances = new List<Alliance>();
            try
            {

                List<clan> clans;
                using (var db = new ucsdbEntities(m_vConnectionString))
                {
                    clans = db.clan.ToList();
                }

                foreach (clan c in clans)
                {
                    var alliance = new Alliance();
                    alliance.LoadFromJSON(c.Data);
                    alliances.Add(alliance);
                }
            }
            catch (Exception ex)
            {
                Debugger.WriteLine("An exception occured during GetAlliance processing:", ex, 0, ConsoleColor.DarkRed);
            }
            return alliances;
        }

        public Alliance GetAlliance(long allianceId)
        {
            Alliance alliance = null;
            try
            {
                using (var db = new ucsdbEntities(m_vConnectionString))
                {
                    var p = db.clan.Find(allianceId);
                    
                    if (p != null)
                    {
                        alliance = new Alliance();
                        alliance.LoadFromJSON(p.Data);
                    }
                }
            }
            catch (Exception ex)
            {
                Debugger.WriteLine("An exception occured during GetAlliance processing:", ex, 0, ConsoleColor.DarkRed);
            }
            return alliance;
        }

        public List<long> GetAllPlayerIds()
        {
            var ids = new List<long>();
            List<player> players;
            using (var db = new ucsdbEntities(m_vConnectionString))
            {
                players = db.player.ToList();
                db.Dispose();
            }
            players.ForEach(p => ids.Add(p.PlayerId));
            return ids;
        }

        public long GetMaxAllianceId()
        {
            long max = 0;
            using (var db = new ucsdbEntities(m_vConnectionString))
            {
                max = (from alliance in db.clan
                    select (long?) alliance.ClanId ?? 0).DefaultIfEmpty().Max();
            }
            return max;
        }

        public long GetMaxPlayerId()
        {
            long max = 0;
            using (var db = new ucsdbEntities(m_vConnectionString))
            {
                max = (from ep in db.player
                    select (long?) ep.PlayerId ?? 0).DefaultIfEmpty().Max();
            }
            return max;
        }

        public void Save(Level avatar)
        {
            Debugger.WriteLine(
                "Starting saving player " + avatar.GetPlayerAvatar().GetAvatarName() + " from memory to database at " +
                DateTime.Now, null, 5, ConsoleColor.DarkGreen);
            var context = new ucsdbEntities(m_vConnectionString);
            context.Configuration.AutoDetectChangesEnabled = false;
            context.Configuration.ValidateOnSaveEnabled = false;
            context = avatar.SaveToDatabse(context);
            context.SaveChanges();
            Debugger.WriteLine(
                "Finished saving player " + avatar.GetPlayerAvatar().GetAvatarName() + " from memory to database at " +
                DateTime.Now, null, 5, ConsoleColor.DarkGreen);
        }

        public void RemoveAlliance(Alliance alliance)
        {
            using (var db = new ucsdbEntities(m_vConnectionString))
            {
                db.clan.Remove(db.clan.Find((int)alliance.GetAllianceId()));
                db.SaveChanges();
            }
        }

        public void Save(List<Level> avatars)
        {
            Debugger.WriteLine("Starting saving players from memory to database at " + DateTime.Now, null, 0);
             try
             {
                 var parts = avatars.Split(saveThreadCount);

                 var saveThreads = new List<Thread>();
                Parallel.ForEach(parts, part =>
                    {
                        var threadObject = new SaveLevelThread(part.ToList(), m_vConnectionString);
                        var t = new Thread(threadObject.DoSaveWork);
                        saveThreads.Add(t);
                        t.Start();
                    });
                var workerArentFinished = true;

                 while (workerArentFinished)
                 {
                     workerArentFinished = false;
                    Parallel.ForEach(saveThreads, t =>
                        {
                            if (t.IsAlive)
                            {
                                workerArentFinished = true;
                            }
                        });
                }

                 Debugger.WriteLine("Finished saving players from memory to database at " + DateTime.Now, null, 0);
             }
             catch (Exception ex)
             {
                 Debugger.WriteLine("An exception occured during Save processing for avatars:", ex, 0,
                     ConsoleColor.DarkRed);
             }
        }

        public void Save(List<Alliance> alliances)
         {
            MainWindow.RemoteWindow.WriteConsoleDebug("Starting saving alliances from memory to database at " + DateTime.Now, (int)MainWindow.level.DEBUGLOG);
             try
             {
                 var parts = alliances.Split(saveThreadCount);

                 var saveThreads = new List<Thread>();
                Parallel.ForEach(parts, part =>
                    {
                        var threadObject = new SaveAllianceThread(part.ToList(), m_vConnectionString);
                        var t = new Thread(threadObject.DoSaveWork);
                        saveThreads.Add(t);
                        t.Start();
                    });
                var workerArentFinished = true;

                 while (workerArentFinished)
                 {
                     workerArentFinished = false;
                    Parallel.ForEach(saveThreads, t =>
                        {
                            if (t.IsAlive)
                            {
                                workerArentFinished = true;
                            }
                        });
                }

                 Debugger.WriteLine("Finished saving alliances from memory to database at " + DateTime.Now, null, 0);
             }
             catch (Exception ex)
             {
                 Debugger.WriteLine("An exception occured during Save processing for alliances:", ex, 0,
                     ConsoleColor.DarkRed);
             }
         }
        }
    }