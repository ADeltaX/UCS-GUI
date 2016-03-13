using System.IO;
using Ultrapowa_Clash_Server_GUI.Core;
using Ultrapowa_Clash_Server_GUI.Logic;
using Ultrapowa_Clash_Server_GUI.Network;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    internal class ResetHomeGameOpCommand : GameOpCommand
    {
        public ResetHomeGameOpCommand(string[] args)
        {
            SetRequiredAccountPrivileges(1);
        }

        public override void Execute(Level level)
        {
            if (level.GetPlayerAvatar().GetId() != null)
            {
                var id = level.GetPlayerAvatar().GetId();
                var l = ResourcesManager.GetPlayer(id);
                if (l != null)
                {
                    using (var sr = new StreamReader(@"gamefiles/default/home.json"))
                    {
                        level.SetHome(sr.ReadToEnd());
                    }
                    var p = new OutOfSyncMessage(l.GetClient());
                    PacketManager.ProcessOutgoingPacket(p);
                }
                else
                {
                    MainWindow.RemoteWindow.WriteConsoleDebug("ResetPlayer failed: id " + id + " not found", (int)MainWindow.level.DEBUGLOG);
                }
            }
            else
            {
                SendCommandFailedMessage(level.GetClient());
            }
        }
    }
}