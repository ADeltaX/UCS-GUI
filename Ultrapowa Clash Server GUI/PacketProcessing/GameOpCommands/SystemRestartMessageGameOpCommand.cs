using System;
using Ultrapowa_Clash_Server_GUI.Core;
using Ultrapowa_Clash_Server_GUI.Logic;
using Ultrapowa_Clash_Server_GUI.Network;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    internal class SystemRestartMessageGameOpCommand : GameOpCommand
    {
        private readonly string[] m_vArgs;

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
                    var mail = new AllianceMailStreamEntry();
                    mail.SetId((int) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
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
                        var s = new ShutdownStartedMessage(onlinePlayer.GetClient());
                        var p = new AvatarStreamEntryMessage(onlinePlayer.GetClient());
                        p.SetAvatarStreamEntry(mail);
                        s.SetCode(5);
                        PacketManager.ProcessOutgoingPacket(s);
                        PacketManager.ProcessOutgoingPacket(p);
                    }
                    Program.RestartProgram();
                }
            }
            else
            {
                SendCommandFailedMessage(level.GetClient());
            }
        }
    }
}