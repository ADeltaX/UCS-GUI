using Ultrapowa_Clash_Server_GUI.Core;
using Ultrapowa_Clash_Server_GUI.Logic;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    internal class SetUsersScoreGameOpCommand : GameOpCommand
    {
        private readonly string[] m_vArgs;

        public SetUsersScoreGameOpCommand(string[] args)
        {
            m_vArgs = args;
            SetRequiredAccountPrivileges(5);
        }

        public override void Execute(Level level)
        {
            if (level.GetAccountPrivileges() >= GetRequiredAccountPrivileges())
            {
                if (m_vArgs.Length >= 2)
                {
                    long id;
                    if (long.TryParse(m_vArgs[1], out id))
                    {
                        int newScore;
                        if (int.TryParse(m_vArgs[2], out newScore))
                        {
                            var l = ResourcesManager.GetPlayer(id);
                            if (l != null)
                            {
                                l.GetPlayerAvatar().SetScore(newScore);
                            }
                            else
                            {
                                MainWindow.RemoteWindow.WriteConsoleDebug("SetUserScore failed: id " + id + " not found", (int)MainWindow.level.DEBUGLOG);
                            }
                        }
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