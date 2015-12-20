using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Net.Sockets;
using System.Threading;
using Ultrapowa_Clash_Server_GUI.Helpers;
using Ultrapowa_Clash_Server_GUI.Logic;
using Ultrapowa_Clash_Server_GUI.PacketProcessing;
using Timer = System.Threading.Timer;

namespace Ultrapowa_Clash_Server_GUI.Core
{
    internal class ResourcesManager
    {
        private static readonly object m_vOnlinePlayersLock = new object();

        private static ConcurrentDictionary<long, Client> m_vClients;
        private static ConcurrentDictionary<long, Level> m_vInMemoryLevels;

        private static List<Level> m_vOnlinePlayers;

        private readonly bool m_vTimerCanceled;

        private readonly Timer TimerReference;

        public ResourcesManager()
        {
            m_vClients = new ConcurrentDictionary<long, Client>();
            m_vOnlinePlayers = new List<Level>();
            m_vInMemoryLevels = new ConcurrentDictionary<long, Level>();
            m_vTimerCanceled = false;
            TimerCallback TimerDelegate = ReleaseOrphans;
            var TimerItem = new Timer(TimerDelegate, null, 1000, 1000);
            TimerReference = TimerItem;
            
        }

        private static void CheckClients()
        {
            foreach (Client c in GetConnectedClients())
            {
                if (!c.IsClientSocketConnected())
                {
                    DropClient(c.GetSocketHandle());
                    try {
                        c.Socket.Shutdown(SocketShutdown.Both);
                        c.Socket.Close();
                    }
                    catch (Exception ex)
                    {
                        Debugger.WriteLine("Error when dropping client.. : ", ex, 4, ConsoleColor.Red);
                    }
                }
            }
        }

        public static void AddClient(Client c)
        {
            var socketHandle = c.Socket.Handle.ToInt64();
            if (!m_vClients.ContainsKey(socketHandle))
                m_vClients.TryAdd(socketHandle, c);
        }

        public static void DropClient(long socketHandle)
        {
            Client c;
            m_vClients.TryRemove(socketHandle, out c);
            if (c.GetLevel() != null)
                LogPlayerOut(c.GetLevel());
        }

        public static List<long> GetAllClanIds()
        {
            return DatabaseManager.Singelton.GetAllPlayerIds();
        }

        public static List<long> GetAllPlayerIds()
        {
            return DatabaseManager.Singelton.GetAllPlayerIds();
        }

        public static Client GetClient(long socketHandle)
        {
            return m_vClients[socketHandle];
        }

        public static List<Client> GetConnectedClients()
        {
            var clients = new List<Client>();
            clients.AddRange(m_vClients.Values);
            return clients;
        }

        public static List<Level> GetInMemoryLevels()
        {
            var levels = new List<Level>();
            lock (m_vOnlinePlayersLock)
            {
                levels.AddRange(m_vInMemoryLevels.Values);
            }
            return levels;
        }

        public static List<Level> GetOnlinePlayers()
        {
            var onlinePlayers = new List<Level>();
            lock (m_vOnlinePlayersLock)
            {
                onlinePlayers = m_vOnlinePlayers.ToList();
            }
            return onlinePlayers;
        }

        public static Level GetPlayer(long id, bool persistent = false)
        {
            var result = GetInMemoryPlayer(id);
            if (result == null)
            {
                result = DatabaseManager.Singelton.GetAccount(id);
                if (persistent)
                    LoadLevel(result);
            }
            return result;
        }

        public static bool IsClientConnected(long socketHandle)
        {
            return m_vClients.ContainsKey(socketHandle);
        }

        public static bool IsPlayerOnline(Level l)
        {
            return m_vOnlinePlayers.Contains(l);
        }

        public static void LoadLevel(Level level)
        {
            var id = level.GetPlayerAvatar().GetId();
            if (!m_vInMemoryLevels.ContainsKey(id))
                m_vInMemoryLevels.TryAdd(id, level);
        }

        public static void LogPlayerIn(Level level, Client client)
        {
            level.SetClient(client);
            client.SetLevel(level);
            lock (m_vOnlinePlayersLock)
            {
                if (!m_vOnlinePlayers.Contains(level))
                {
                    m_vOnlinePlayers.Add(level);
                    LoadLevel(level);
                }
            }
        }

        public static void LogPlayerOut(Level level)
        {
            lock (m_vOnlinePlayersLock)
            {
                m_vOnlinePlayers.Remove(level);
            }
            DatabaseManager.Singelton.Save(level);
            m_vInMemoryLevels.TryRemove(level.GetPlayerAvatar().GetId());
        }

        private static Level GetInMemoryPlayer(long id)
        {
            Level result = null;
            lock (m_vOnlinePlayersLock)
            {
                if (m_vInMemoryLevels.ContainsKey(id))
                    result = m_vInMemoryLevels[id];
            }
            return result;
        }

        private void ReleaseOrphans(object state)
        {
            CheckClients();
            if (m_vTimerCanceled)
            {
                TimerReference.Dispose();
            }
        }
    }
}