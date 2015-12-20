using System;
using Ultrapowa_Clash_Server_GUI.Core;
using Ultrapowa_Clash_Server_GUI.Logic;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    internal class UnbanGameOpCommand : GameOpCommand
    {
        private readonly string[] m_vArgs;

        public UnbanGameOpCommand(string[] args)
        {
            m_vArgs = args;
            SetRequiredAccountPrivileges(2);
        }

        public override void Execute(Level level)
        {
            if (level.GetAccountPrivileges() >= GetRequiredAccountPrivileges())
            {
                if (m_vArgs.Length >= 2)
                {
                    try
                    {
                        var id = Convert.ToInt64(m_vArgs[1]);
                        var l = ResourcesManager.GetPlayer(id);
                        if (l != null)
                        {
                            l.SetAccountStatus(0);
                        }
                        else
                        {
                            MainWindow.RemoteWindow.WriteConsoleDebug("Unban failed: id " + id + " not found", (int)MainWindow.level.DEBUGLOG);
                        }
                    }
                    catch (Exception ex)
                    {
                        MainWindow.RemoteWindow.WriteConsoleDebug("Unban failed with error: " + ex, (int)MainWindow.level.DEBUGFATAL);
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