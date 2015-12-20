using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading;
using Ultrapowa_Clash_Server_GUI.Core;
using Ultrapowa_Clash_Server_GUI.PacketProcessing;

namespace Ultrapowa_Clash_Server_GUI.Network
{
    internal class PacketManager
    {
        private static readonly EventWaitHandle m_vIncomingWaitHandle = new AutoResetEvent(false);

        private static readonly EventWaitHandle m_vOutgoingWaitHandle = new AutoResetEvent(false);

        private static ConcurrentQueue<Message> m_vIncomingPackets = new ConcurrentQueue<Message>();

        private static ConcurrentQueue<Message> m_vOutgoingPackets = new ConcurrentQueue<Message>();

        private bool m_vIsRunning;

        public PacketManager()
        {
            m_vIsRunning = false;
        }

        public static void ProcessIncomingPacket(Message p)
        {
            m_vIncomingPackets.Enqueue(p);
            m_vIncomingWaitHandle.Set();
        }

        public static void ProcessOutgoingPacket(Message p)
        {
            p.Encode();
            try
            {
                var pl = p.Client.GetLevel();
                var player = "";
                if (pl != null)
                    player += string.Format(" ({0}, {1})", pl.GetPlayerAvatar().GetId(),
                        pl.GetPlayerAvatar().GetAvatarName());
                MainWindow.RemoteWindow.WriteConsoleDebug(string.Format("[S] {0} {1}{2}", p.GetMessageType(), p.GetType().Name, player), (int)MainWindow.level.DEBUGLOG);
                //GuiConsoleWrite.Write(string.Format("[S] {0} {1}{2}", p.GetMessageType(), p.GetType().Name, player));
                m_vOutgoingPackets.Enqueue(p);
                m_vOutgoingWaitHandle.Set();
            }
            catch (Exception ex)
            {
                MainWindow.RemoteWindow.WriteConsoleDebug("Error with packet manager: " + ex, (int)MainWindow.level.DEBUGFATAL);
            }
        }

        public void Start()
        {
            m_vIsRunning = true;

            IncomingProcessingDelegate incomingProcessing = IncomingProcessing;
            incomingProcessing.BeginInvoke(null, null);

            OutgoingProcessingDelegate outgoingProcessing = OutgoingProcessing;
            outgoingProcessing.BeginInvoke(null, null);
        }

        public void Stop()
        {
            m_vIsRunning = false;
        }

        private void IncomingProcessing()
        {
            while (m_vIsRunning)
            {
                m_vIncomingWaitHandle.WaitOne();
                Message p;
                while (m_vIncomingPackets.TryDequeue(out p))
                {
                    p.Client.Decrypt(p.GetData());
                    Logger.WriteLine(p, "R");
                    MessageManager.ProcessPacket(p);
                }
            }
        }

        private void OutgoingProcessing()
        {
            while (m_vIsRunning)
            {
                m_vOutgoingWaitHandle.WaitOne();
                Message p;
                while (m_vOutgoingPackets.TryDequeue(out p))
                {
                    Logger.WriteLine(p, "S");
                    if (p.GetMessageType() == 20000)
                    {
                        var sessionKey = ((SessionKeyMessage) p).Key;
                        p.Client.Encrypt(p.GetData());
                        p.Client.UpdateKey(sessionKey);
                    }
                    else
                    {
                        p.Client.Encrypt(p.GetData());
                    }

                    try
                    {
                        if (p.Client.Socket != null)
                        {
                            p.Client.Socket.Send(p.GetRawData());
                        }
                        else
                        {
                            ResourcesManager.DropClient(p.Client.GetSocketHandle());
                        }
                    }
                    catch (Exception)
                    {
                        try
                        {
                            ResourcesManager.DropClient(p.Client.GetSocketHandle());
                            p.Client.Socket.Shutdown(SocketShutdown.Both);
                            p.Client.Socket.Close();
                        }
                        catch (Exception ex)
                        {
                            MainWindow.RemoteWindow.WriteConsoleDebug("Error when closing the connection from client: "+ ex, (int)MainWindow.level.DEBUGFATAL);
                        }
                    }
                }
            }
        }

        private delegate void IncomingProcessingDelegate();

        private delegate void OutgoingProcessingDelegate();
    }
}