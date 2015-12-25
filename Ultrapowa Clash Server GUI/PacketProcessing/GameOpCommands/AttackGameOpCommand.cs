using System;
using Ultrapowa_Clash_Server_GUI.Core;
using Ultrapowa_Clash_Server_GUI.Logic;
using Ultrapowa_Clash_Server_GUI.Network;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    internal class AttackGameOpCommand : GameOpCommand
    {
        private readonly string[] m_vArgs;

        public AttackGameOpCommand(string[] args)
        {
            m_vArgs = args;
            SetRequiredAccountPrivileges(2);
        }

        public override void Execute(Level level)
        {
            if (level.GetAccountPrivileges() >= GetRequiredAccountPrivileges())
            {
                if (m_vArgs.Length >= 1)
                {
                    try
                    {
                        var id = Convert.ToInt64(m_vArgs[1]);
                        var l = ResourcesManager.GetPlayer(id);
                        if (l != null)
                        {
                            l.Tick();

                            //var p = new EnemyHomeDataMessage(level.GetClient(), l, level);
                            var p = new VisitedHomeDataMessage(level.GetClient(), l, level);
                            PacketManager.ProcessOutgoingPacket(p);
                        }
                        else
                        {
                            MainWindow.RemoteWindow.WriteConsoleDebug("Attack failed: id " + id + " not found", (int)MainWindow.level.DEBUGLOG);
                        }
                    }
                    catch (Exception ex)
                    {
                        MainWindow.RemoteWindow.WriteConsoleDebug("Attack failed with error: " + ex, (int)MainWindow.level.DEBUGFATAL);
                    }
                }
            }
            else
            {
                SendCommandFailedMessage(level.GetClient());
            }
        }
    }
}