using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Ultrapowa_Clash_Server_GUI.Logic;
using Ultrapowa_Clash_Server_GUI.Helpers;
using Ultrapowa_Clash_Server_GUI.GameFiles;
using Ultrapowa_Clash_Server_GUI.Core;
using Ultrapowa_Clash_Server_GUI.Network;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    class SystemRestartMessageGameOpCommand : GameOpCommand
    {
        private string[] m_vArgs;

        public SystemRestartMessageGameOpCommand(string[] args)
        {
            m_vArgs = args;
            SetRequiredAccountPrivileges(4);
        }

        public override void Execute(Level level)
        {
            if (level.GetAccountPrivileges() >= GetRequiredAccountPrivileges())
            {
                if (m_vArgs.Length >= 1)
                {
                    var avatar = level.GetPlayerAvatar();
                    AllianceMailStreamEntry mail = new AllianceMailStreamEntry();
                    mail.SetId((int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
                    mail.SetSenderId(0);
                    mail.SetSenderAvatarId(0);
                    mail.SetSenderName("System Admin");
                    mail.SetIsNew(0);
                    mail.SetAllianceId(0);
                    mail.SetAllianceBadgeData(0);
                    mail.SetAllianceName("System Manager");
                    mail.SetMessage("System is restarting in a few moments");
                    mail.SetSenderLevel(500);
                    mail.SetSenderLeagueId(22);

                    foreach (var onlinePlayer in ResourcesManager.GetOnlinePlayers())
                    {
                        var p = new AvatarStreamEntryMessage(onlinePlayer.GetClient());
                        p.SetAvatarStreamEntry(mail);
                        PacketManager.ProcessOutgoingPacket(p);
                    }
                    System.Diagnostics.Process.Start(@"tools\Ultrapowa_Clash_Server_GUI-restart.bat");
                }
            }
            else
            {
                SendCommandFailedMessage(level.GetClient());
            }
        }
    }
}
