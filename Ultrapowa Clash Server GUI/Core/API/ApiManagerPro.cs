using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Ultrapowa_Clash_Server_GUI.Core
{
    public class ApiManagerPro
    {
        public static string jsonapp;

        private static readonly HttpListener _listener = new HttpListener();
        private readonly Func<HttpListenerRequest, string> _responderMethod;

        public ApiManagerPro(string[] prefixes, Func<HttpListenerRequest, string> method)
        {
            try
            {
                if (!HttpListener.IsSupported)
                    throw new NotSupportedException(
                        "Windows XP SP2, Server 2003 or higher needed.Please Disable Api Manager");

                if (prefixes == null || prefixes.Length == 0)
                    throw new ArgumentException("prefixes");

                if (method == null)
                    throw new ArgumentException("method");

                foreach (var s in prefixes)
                    _listener.Prefixes.Add(s);

                _responderMethod = method;
                _listener.Start();
            }
            catch (Exception e)
            {
                MainWindow.RemoteWindow.WriteConsoleDebug("Exception at ApiManagerPro "+ e, (int)MainWindow.level.DEBUGFATAL);
            }
        }

        public ApiManagerPro(Func<HttpListenerRequest, string> method, params string[] prefixes)
            : this(prefixes, method)
        {
        }

        public HttpListenerTimeoutManager TimeoutManager { get; }

        public IPEndPoint RemoteEndPoint { get; }

        public static void JsonMain()
        {
            var UcsVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            var f = new JsonApi
            {
                UCS = new Dictionary<string, string>
                {
                    {"StartingLevel", ConfigurationManager.AppSettings["startingLevel"]},
                    {"StartingExperience", ConfigurationManager.AppSettings["startingExperience"]},
                    {"StartingGold", ConfigurationManager.AppSettings["startingGold"]},
                    {"StartingElixir", ConfigurationManager.AppSettings["startingElixir"]},
                    {"StartingDarkElixir", ConfigurationManager.AppSettings["startingDarkElixir"]},
                    {"StartingTrophies", ConfigurationManager.AppSettings["startingTrophies"]},
                    {"StartingShieldTime", ConfigurationManager.AppSettings["startingShieldTime"]},
                    {"PatchingServer", ConfigurationManager.AppSettings["patchingServer"]},
                    {"Maintenance", ConfigurationManager.AppSettings["maintenanceMode"]},
                    {"MaintenanceTimeLeft", ConfigurationManager.AppSettings["maintenanceTimeLeft"]},
                    {"ServerPort", ConfigurationManager.AppSettings["serverPort"]},
                    {"ClientVersion", ConfigurationManager.AppSettings["clientVersion"]},
                    {"ServerVersion", UcsVersion},
                    {"LoggingLevel", ConfigurationManager.AppSettings["loggingLevel"]},
                    {"OldClientVersion", ConfigurationManager.AppSettings["oldClientVersion"]},
                    {"DatabaseType", ConfigurationManager.AppSettings["databaseConnectionName"]},
                    {"ExpertPVE", ConfigurationManager.AppSettings["expertPve"]},
                    {"SaveThreadCount", ConfigurationManager.AppSettings["saveThreadCount"]},
                    {"OnlinePlayers", Convert.ToString(ResourcesManager.GetOnlinePlayers().Count)},
                    {"InMemoryPlayers", Convert.ToString(ResourcesManager.GetInMemoryLevels().Count)},
                    {"InMemoryClans", Convert.ToString(ObjectManager.GetInMemoryAlliances().Count)},
                    {"TotalPlayer", Convert.ToString(ResourcesManager.GetAllPlayerIds().Count)},
                    {"TotalClans", Convert.ToString(ObjectManager.GetInMemoryAlliances().Count)},
                    {"TotalConnectedClients", Convert.ToString(ResourcesManager.GetConnectedClients().Count)}
                }
            };

            jsonapp = JsonConvert.SerializeObject(f);
        }

        public static string SendResponse(HttpListenerRequest request)
        {
            JsonMain();
            return jsonapp;
        }

        public static void Stop()
        {
            _listener.Stop();
            _listener.Close();
        }

        public void Run()
        {
            ThreadPool.QueueUserWorkItem(o =>
            {
                try
                {
                    Console.WriteLine("Pro API Manager : Online");
                    while (_listener.IsListening)
                    {
                        ThreadPool.QueueUserWorkItem(c =>
                        {
                            var ctx = c as HttpListenerContext;
                            try
                            {
                                MainWindow.RemoteWindow.WriteConsoleDebug("New API Request!", (int)MainWindow.level.DEBUGLOG);
                                var rstr = _responderMethod(ctx.Request);
                                var buf = Encoding.UTF8.GetBytes(rstr);
                                ctx.Response.ContentLength64 = buf.Length;
                                ctx.Response.OutputStream.Write(buf, 0, buf.Length);
                            }
                            finally
                            {
                                ctx.Response.OutputStream.Close();
                            }
                        }, _listener.GetContext());
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("APIManagerPro : Error when starting API => " + ex);
                }
            });
        }

        private class JsonApi
        {
            public Dictionary<string, string> UCS { get; set; }
        }
    }
}