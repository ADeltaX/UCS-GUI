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
    class SystemMessageGameOpCommand : GameOpCommand
    {
        private string[] m_vArgs;

        public SystemMessageGameOpCommand(string[] args)
        {
            m_vArgs = args;
            SetRequiredAccountPrivileges(1);
        }

        public override void Execute(Level level)
        {
            if(level.GetAccountPrivileges() >= GetRequiredAccountPrivileges())
            {
                if(m_vArgs.Length >= 1)
                {
                    string message = string.Join(" ", m_vArgs.Skip(1));
                    var avatar = level.GetPlayerAvatar();
                    AllianceMailStreamEntry mail = new AllianceMailStreamEntry();
                    mail.SetId((int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
                    mail.SetSenderId(avatar.GetId());
                    mail.SetSenderAvatarId(avatar.GetId());
                    mail.SetSenderName(avatar.GetAvatarName());
                    mail.SetIsNew(0);
                    mail.SetAllianceId(0);
                    mail.SetAllianceBadgeData(0);
                    mail.SetAllianceName("Legendary Administrator");
                    mail.SetMessage(message);
                    mail.SetSenderLevel(avatar.GetAvatarLevel());
                    mail.SetSenderLeagueId(avatar.GetLeagueId());

                    foreach (var onlinePlayer in ResourcesManager.GetOnlinePlayers())
                    {
                        var p = new AvatarStreamEntryMessage(onlinePlayer.GetClient());
                        p.SetAvatarStreamEntry(mail);
                        PacketManager.ProcessOutgoingPacket(p);
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
