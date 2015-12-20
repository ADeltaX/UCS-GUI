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
                Console.WriteLine("Exception when attempting to host (" + port + "): " + e);

                Socket = null;

                return false;
            }

            return true;
        }

        public void Start()
        {
            if (Host(kPort))
            {
                Console.WriteLine("Gateway started on port " + kPort);
                Console.WriteLine("Message Manager started");
                Console.WriteLine("Packet Manager started");
            }
        }

        private void OnClientConnect(IAsyncResult result)
        {
            try
            {
                var clientSocket = Socket.EndAccept(result);
                ResourcesManager.AddClient(new Client(clientSocket));
                SocketRead.Begin(clientSocket, OnReceive, OnReceiveError);
                Console.WriteLine("Client connected (" + ((IPEndPoint) clientSocket.RemoteEndPoint).Address + ":" +
                                  ((IPEndPoint) clientSocket.RemoteEndPoint).Port + ")");
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception when accepting incoming connection: " + e);
            }
            try
            {
                Socket.BeginAccept(OnClientConnect, Socket);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception when starting new accept process: " + e);
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
               Debugger.WriteLine("Error when receiving packet from client : ", ex, 4, ConsoleColor.Red);
            }
        }

        private void OnReceiveError(SocketRead read, Exception exception)
        {
            Debugger.WriteLine("Error received: " + exception, null, 5);
        }
    }
}