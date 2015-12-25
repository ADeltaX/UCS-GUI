using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Ultrapowa_Clash_Server_GUI.Core
{
    internal class ApiManager
    {
        private static HttpListener m_vListener;

        public ApiManager()
        {
            var hostName = Dns.GetHostName();
            var IP = Dns.GetHostByName(hostName).AddressList[0].ToString();
            var DebugPort = ConfigurationManager.AppSettings["debugPort"];
            m_vListener = new HttpListener();
            {
                m_vListener.Prefixes.Add("http://localhost:" + DebugPort + "/Debug/");

                //m_vListener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
                m_vListener.Start();
                Action action = RunServer;
                action.BeginInvoke(RunServerCallback, action);
            }
        }

        public void Stop()
        {
            m_vListener.Stop();
            m_vListener.Close();
        }

        private void Handle(IAsyncResult result)
        {
            var direction = "";
            var requestip = (HttpWebRequest) WebRequest.Create("http://checkip.dyndns.org/");
            requestip.UserAgent =
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/49.0.2573.0 Safari/537.36";
            requestip.Method = "GET";
            requestip.Referer = "http://ultrapowa.com/";
            using (var stream = new StreamReader(requestip.GetResponse().GetResponseStream()))
            {
                direction = stream.ReadToEnd();
            }

            //Search for the ip in the html
            var first = direction.IndexOf("Address: ") + 9;
            var last = direction.LastIndexOf("</body>");
            direction = direction.Substring(first, last - first);
            var listener = (HttpListener) result.AsyncState;
            var context = listener.EndGetContext(result);

            var request = context.Request;

            // Obtain a response object. 
            var response = context.Response;

            // Construct a response. 
            var responseString = "<HTML><BODY><PRE>";

            responseString += "Active Connections: ";
            var connectedUsers = 0;
            foreach (var client in ResourcesManager.GetConnectedClients())
            {
                var socket = client.Socket;
                if (socket != null)
                {
                    try
                    {
                        var part1 = socket.Poll(1000, SelectMode.SelectRead);
                        var part2 = socket.Available == 0;
                        if (!(part1 && part2))
                            connectedUsers++;
                    }
                    catch (Exception ex)
                    {
                        Debugger.WriteLine("Error in ApiManager : ", ex, 4, ConsoleColor.Red);
                    }
                }
            }
            responseString += connectedUsers + "\n";

            responseString += "Established Connections: " + ResourcesManager.GetConnectedClients().Count + "\n";

            responseString += "<details><summary>";
            responseString += "In Memory Levels: " + ResourcesManager.GetInMemoryLevels().Count + "</summary>";
            foreach (var account in ResourcesManager.GetInMemoryLevels())
            {
                responseString += "    " + account.GetPlayerAvatar().GetId() + ", " +
                                  account.GetPlayerAvatar().GetAvatarName() + " \n";
            }
            responseString += "</details>";

            responseString += "<details><summary>";
            responseString += "Online Players: " + ResourcesManager.GetOnlinePlayers().Count + "</summary>";
            foreach (var account in ResourcesManager.GetOnlinePlayers())
            {
                responseString += "    " + account.GetPlayerAvatar().GetId() + ", " +
                                  account.GetPlayerAvatar().GetAvatarName() + " \n";
            }
            responseString += "</details>";

            responseString += "<details><summary>";
            responseString += "In Memory Alliances: " + ObjectManager.GetInMemoryAlliances().Count + "</summary>";
            foreach (var alliance in ObjectManager.GetInMemoryAlliances())
            {
                responseString += "    " + alliance.GetAllianceId() + ", " + alliance.GetAllianceName() + " \n";
            }
            responseString += "</details>";

            var hostName = Dns.GetHostName();
            var LIP = Dns.GetHostByName(hostName).AddressList[0].ToString();

            responseString += "<center><p>Current local ip: " + LIP + "</p></center>";
            responseString += "<center><p>Current public ip: " + direction + "</p></center>";
            responseString +=
                "<center><img src='https://d14.usercdn.com/i/02212/ea18nj5uxcll.png' style='width: 25%; height: 50%'></img></center></PRE></BODY></HTML>";
            var buffer = Encoding.UTF8.GetBytes(responseString);

            // Get a response stream and write the response to it. 
            response.ContentLength64 = buffer.Length;
            var output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);

            // You must close the output stream. 
            output.Close();
        }

        private void RunServer()
        {
            var DebugPort = ConfigurationManager.AppSettings["debugPort"];
            MainWindow.RemoteWindow.WriteConsole("API Manager started on http://localhost:" + DebugPort + "/Debug/", (int)MainWindow.level.LOG);
            while (m_vListener.IsListening)
            {
                var result = m_vListener.BeginGetContext(Handle, m_vListener);
                result.AsyncWaitHandle.WaitOne();
            }
        }

        private void RunServerCallback(IAsyncResult ar)
        {
            try
            {
                var target = (Action) ar.AsyncState;
                target.EndInvoke(ar);
            }
            catch (Exception ex)
            {
                MainWindow.RemoteWindow.WriteConsole(ex.Message, (int)MainWindow.level.WARNING);
            }
        }

        private void StartListener(object data)
        {
            // Note: The GetContext method blocks while waiting for a request. 
            var context = m_vListener.GetContext();

            m_vListener.Stop();
        }
    }
}