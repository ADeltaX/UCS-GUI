using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Ultrapowa_Clash_Server_GUI.Core;
using Ultrapowa_Clash_Server_GUI.PacketProcessing;

namespace Ultrapowa_Clash_Server_GUI.Network
{
    internal class Gateway
    {
        private const int kHostConnectionBacklog = 30;

        private static readonly int kPort = Program.port;

        private IPAddress ip;

        public static Socket Socket { get; private set; }

        public IPAddress IP
        {
            get
            {
                if (ip == null)
                {
                    ip = (
                        from entry in Dns.GetHostEntry(Dns.GetHostName()).AddressList
                        where entry.AddressFamily == AddressFamily.InterNetwork
                        select entry
                        ).FirstOrDefault();
                }

                return ip;
            }
        }

        public bool Host(int port)
        {
            MainWindow.RemoteWindow.WriteConsoleDebug("Hosting on port " + port,(int)MainWindow.level.DEBUGLOG);

            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                Socket.Bind(new IPEndPoint(IPAddress.Any, port));
                Socket.Listen(kHostConnectionBacklog);
                Socket.BeginAccept(OnClientConnect, Socket);
            }
            catch (Exception e)
            {
                MainWindow.RemoteWindow.WriteConsole("Exception when attempting to host (" + port + "): " + e, (int)MainWindow.level.FATAL);

                Socket = null;

                return false;
            }

            return true;
        }

        public void Start()
        {
            if (!Host(kPort))
            {
                MainWindow.RemoteWindow.WriteConsole("Gateway started on port " + kPort, (int)MainWindow.level.LOG);
                MainWindow.RemoteWindow.WriteConsole("Message Manager started", (int)MainWindow.level.LOG);
                MainWindow.RemoteWindow.WriteConsole("Packet Manager started", (int)MainWindow.level.LOG);
            }

            
        }

        private void OnClientConnect(IAsyncResult result)
        {
            try
            {
                var clientSocket = Socket.EndAccept(result);
                ResourcesManager.AddClient(new Client(clientSocket));
                SocketRead.Begin(clientSocket, OnReceive, OnReceiveError);
                MainWindow.RemoteWindow.WriteConsole("Client connected (" + ((IPEndPoint) clientSocket.RemoteEndPoint).Address + ":" +
                                  ((IPEndPoint) clientSocket.RemoteEndPoint).Port + ")", (int)MainWindow.level.LOG);
            }
            catch (Exception e)
            {
                MainWindow.RemoteWindow.WriteConsole("Exception when accepting incoming connection: " + e, (int)MainWindow.level.WARNING);
            }
            try
            {
                Socket.BeginAccept(OnClientConnect, Socket);
            }
            catch (Exception e)
            {
                MainWindow.RemoteWindow.WriteConsole("Exception when starting new accept process: " + e, (int)MainWindow.level.WARNING);
            }
        }

        private void OnReceive(SocketRead read, byte[] data)
        {
            try
            {
                var socketHandle = read.Socket.Handle.ToInt64();
                var c = ResourcesManager.GetClient(socketHandle);
                c.DataStream.AddRange(data);
                Message p;
                while (c.TryGetPacket(out p))
                {
                    PacketManager.ProcessIncomingPacket(p);
                }
            }
            catch (Exception ex)
            {
                MainWindow.RemoteWindow.WriteConsoleDebug("Error when receiving packet from client: " + ex, (int)MainWindow.level.DEBUGFATAL);
            }
        }

        private void OnReceiveError(SocketRead read, Exception exception)
        {
            MainWindow.RemoteWindow.WriteConsoleDebug("Error received: " + exception, (int)MainWindow.level.DEBUGLOG);
        }
    }
}