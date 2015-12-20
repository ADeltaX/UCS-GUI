using System;
using Ultrapowa_Clash_Server_GUI.Core;
using Ultrapowa_Clash_Server_GUI.Logic;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    internal class ReloadFilterGameOpCommand : GameOpCommand
    {
        private string[] m_vArgs;

        public ReloadFilterGameOpCommand(string[] args)
        {
            m_vArgs = args;
            SetRequiredAccountPrivileges(5);
        }

        public override void Execute(Level level)
        {
            if (level.GetAccountPrivileges() >= GetRequiredAccountPrivileges())
            {
                Message.ReloadChatFilterList();
                MainWindow.RemoteWindow.WriteConsoleDebug("Filterlist is reloaded!", (int)MainWindow.level.DEBUGLOG);
            }
            else
            {
                SendCommandFailedMessage(level.GetClient());
            }
        }
    }
}