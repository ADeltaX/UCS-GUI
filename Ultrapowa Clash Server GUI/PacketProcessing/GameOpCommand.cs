using Ultrapowa_Clash_Server_GUI.Core;
using Ultrapowa_Clash_Server_GUI.Logic;
using Ultrapowa_Clash_Server_GUI.Network;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    internal class GameOpCommand
    {
        private byte m_vRequiredAccountPrivileges;

        public virtual void Execute(Level level)
        {
        }

        public byte GetRequiredAccountPrivileges()
        {
            return m_vRequiredAccountPrivileges;
        }

        public void SendCommandFailedMessage(Client c)
        {
            MainWindow.RemoteWindow.WriteConsoleDebug("GameOp command failed. Insufficient privileges", (int)MainWindow.level.DEBUGFATAL);
            var p = new GlobalChatLineMessage(c);
            p.SetChatMessage("GameOp command failed. Insufficient privileges.");
            p.SetPlayerId(0);
            p.SetPlayerName("System Manager");
            PacketManager.ProcessOutgoingPacket(p);
        }

        public void SetRequiredAccountPrivileges(byte level)
        {
            m_vRequiredAccountPrivileges = level;
        }
    }
}