using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Configuration;
using Ultrapowa_Clash_Server_GUI.PacketProcessing;

namespace Ultrapowa_Clash_Server_GUI.Core
{
    class ApiManager
    {
        private static HttpListener m_vListener;

        public ApiManager()
        {
            string hostName = Dns.GetHostName();
            string IP = Dns.GetHostByName(hostName).AddressList[0].ToString();
            string DebugPort = ConfigurationManager.AppSettings["debugPort"];
            m_vListener = new HttpListener();
            {
                string FullDebugMode = "http://localhost:" + DebugPort + "/Debug/";
                m_vListener.Prefixes.Add("http://localhost:" + DebugPort + "/Debug/");
                //m_vListener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
                m_vListener.Start();
                Action action = RunServer;
                action.BeginInvoke(RunServerCallback, action);
            }
        }

        private void StartListener(object data)
        {
            
            // Note: The GetContext method blocks while waiting for a request. 
            HttpListenerContext context = m_vListener.GetContext();
            
            m_vListener.Stop();
        }

        private void RunServer()
        {
            string DebugPort = ConfigurationManager.AppSettings["debugPort"];
            Console.WriteLine("API Manager started on http://localhost:" + DebugPort + "/Debug/");
            while (m_vListener.IsListening)
            {
                IAsyncResult result = m_vListener.BeginGetContext(Handle, m_vListener);
                result.AsyncWaitHandle.WaitOne();
            }
        }

        private void Handle(IAsyncResult result)
        {
            HttpListener listener = (HttpListener)result.AsyncState;
            HttpListenerContext context = listener.EndGetContext(result);


            HttpListenerRequest request = context.Request;
            // Obtain a response object.
            HttpListenerResponse response = context.Response;
            // Construct a response.
            string responseString = "<HTML><BODY><PRE>";
            /*
            responseString += "Active Connections: ";
            int connectedUsers = 0;
            foreach(var client in ResourcesManager.GetConnectedClients())
            {
                var socket = client.Socket;
                if(socket != null)
                {
                    try
                    {
                        bool part1 = socket.Poll(1000, SelectMode.SelectRead);
                        bool part2 = (socket.Available == 0);
                        if (!(part1 && part2))
                            connectedUsers++;
                    }
                    catch(Exception){}
                }
            }
            responseString += connectedUsers + "\n";
            */
            responseString += "Established Connections: " + ResourcesManager.GetConnectedClients().Count + "\n";

            responseString += "<details><summary>";
            responseString += "In Memory Levels: " + ResourcesManager.GetInMemoryLevels().Count + "</summary>";
            foreach (var account in ResourcesManager.GetInMemoryLevels())
            {
                responseString += "    " + account.GetPlayerAvatar().GetId() + ", " + account.GetPlayerAvatar().GetAvatarName() + " \n";
            }
            responseString += "</details>";

            responseString += "<details><summary>";
            responseString += "Online Players: " + ResourcesManager.GetOnlinePlayers().Count + "</summary>";
            foreach (var account in ResourcesManager.GetOnlinePlayers())
            {
                responseString += "    " + account.GetPlayerAvatar().GetId() + ", " + account.GetPlayerAvatar().GetAvatarName() + " \n";
            }
            responseString += "</details>";

            responseString += "<details><summary>";
            responseString += "In Memory Alliances: " + ObjectManager.GetInMemoryAlliances().Count + "</summary>";
            foreach (var alliance in ObjectManager.GetInMemoryAlliances())
            {
                responseString += "    " + alliance.GetAllianceId() + ", " + alliance.GetAllianceName() + " \n";
            }
            responseString += "</details>";

            string hostName = Dns.GetHostName();
            string LIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
            responseString += "<center><p>Current local ip: " + LIP + "</p></center>";
            responseString += "<center><img src='https://d14.usercdn.com/i/02212/ea18nj5uxcll.png' style='width: 25%; height: 50%'></img></center></PRE></BODY></HTML>";
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            // Get a response stream and write the response to it.
            response.ContentLength64 = buffer.Length;
            System.IO.Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            // You must close the output stream.
            output.Close();
        }

        private void RunServerCallback(IAsyncResult ar)
        {
            try
            {
                Action target = (Action)ar.AsyncState;
                target.EndInvoke(ar);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void Stop()
        {
            m_vListener.Stop();
            m_vListener.Close();
        }    
    }
}