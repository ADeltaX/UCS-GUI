using System;
using Ultrapowa_Clash_Server_GUI.Core;
using Ultrapowa_Clash_Server_GUI.Logic;
using Ultrapowa_Clash_Server_GUI.Network;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    internal class RenameAvatarGameOpCommand : GameOpCommand
    {
        private readonly string[] m_vArgs;

        public RenameAvatarGameOpCommand(string[] args)
        {
            m_vArgs = args;
            SetRequiredAccountPrivileges(1);
        }

        public override void Execute(Level level)
        {
            if (level.GetAccountPrivileges() >= GetRequiredAccountPrivileges())
            {
                if (m_vArgs.Length >= 3)
                {
                    try
                    {
                        var id = Convert.ToInt64(m_vArgs[1]);
                        var l = ResourcesManager.GetPlayer(id, true);
                        if (l != null)
                        {
                            l.GetPlayerAvatar().SetName(m_vArgs[2]);
                            if (ResourcesManager.IsPlayerOnline(l))
                            {
                                var p = new AvatarNameChangeOkMessage(l.GetClient());
                                p.SetAvatarName(m_vArgs[2]);
                                PacketManager.ProcessOutgoingPacket(p);
                            }
                        }
                        else
                        {
                            MainWindow.RemoteWindow.WriteConsoleDebug("RenameAvatar failed: id " + id + " not found", (int)MainWindow.level.DEBUGLOG);
                        }
                    }
                    catch (Exception ex)
                    {
                        MainWindow.RemoteWindow.WriteConsoleDebug("RenameAvatar failed with error: " + ex, (int)MainWindow.level.DEBUGFATAL);
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