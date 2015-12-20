using System;
using System.Collections.Concurrent;
using System.Threading;
using Ultrapowa_Clash_Server_GUI.PacketProcessing;

namespace Ultrapowa_Clash_Server_GUI.Core
{
    internal class MessageManager
    {
        private static readonly EventWaitHandle m_vWaitHandle = new AutoResetEvent(false);

        private static readonly ConcurrentQueue<Message> m_vPackets = new ConcurrentQueue<Message>();

        private bool m_vIsRunning;

        public MessageManager()
        {
            m_vIsRunning = false;
        }

        public static void ProcessPacket(Message p)
        {
            m_vPackets.Enqueue(p);
            m_vWaitHandle.Set();
        }

        public void Start()
        {
            PacketProcessingDelegate packetProcessing = PacketProcessing;
            packetProcessing.BeginInvoke(null, null);

            m_vIsRunning = true;
        }

        public void Stop()
        {
            m_vIsRunning = false;
        }

        private void PacketProcessing()
        {
            while (m_vIsRunning)
            {
                m_vWaitHandle.WaitOne();

                Message p;
                while (m_vPackets.TryDequeue(out p))
                {
                    var pl = p.Client.GetLevel();
                    var player = "";
                    if (pl != null)
                        player += " (" + pl.GetPlayerAvatar().GetId() + ", " + pl.GetPlayerAvatar().GetAvatarName() +
                                  ")";
                    try
                    {
                        Debugger.WriteLine("[R] " + p.GetMessageType() + " " + p.GetType().Name + player);
                        p.Decode();
                        p.Process(pl);

                        //Debugger.WriteLine("finished processing of message " + p.GetType().Name + player);
                    }
                    catch (Exception ex)
                    {
                        Debugger.WriteLine("An exception occured during processing of message " + p.GetType().Name + player, ex, 4, ConsoleColor.Red);
                    }
                }
            }
        }

        private delegate void PacketProcessingDelegate();
    }
}