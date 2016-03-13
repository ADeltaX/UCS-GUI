using System;
using Ultrapowa_Clash_Server_GUI.Core;
using Ultrapowa_Clash_Server_GUI.Logic;
using Ultrapowa_Clash_Server_GUI.Network;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    internal class ServerStatusGameOpCommand : GameOpCommand
    {
        private readonly string[] m_vArgs;

        public ServerStatusGameOpCommand(string[] args)
        {
            m_vArgs = args;
            SetRequiredAccountPrivileges(1);
        }

        public override void Execute(Level level)
        {
            if (level.GetAccountPrivileges() >= GetRequiredAccountPrivileges())
            {
                if (m_vArgs.Length >= 1)
                {
                    var mail = new AllianceMailStreamEntry();
                    mail.SetId((int) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
                    mail.SetSenderId(0);
                    mail.SetSenderAvatarId(0);
                    mail.SetSenderName("UCS System");
                    mail.SetIsNew(0);
                    mail.SetAllianceId(0);
                    mail.SetAllianceBadgeData(0);
                    mail.SetAllianceName("Legendary Administrator");
                    mail.SetMessage("Latest Server Status:\nConnected Players:" +
                                    ResourcesManager.GetOnlinePlayers().Count + "\nIn Memory Alliances:" +
                                    ObjectManager.GetInMemoryAlliances().Count + "\nIn Memory Levels:" +
                                    ResourcesManager.GetInMemoryLevels().Count + "\nIn Database Accounts:" +
                                    ResourcesManager.GetAllPlayerIds().Count);
                    mail.SetSenderLeagueId(22);

                    var p = new AvatarStreamEntryMessage(level.GetClient());
                    p.SetAvatarStreamEntry(mail);
                    PacketManager.ProcessOutgoingPacket(p);
                }
            }
            else
            {
                SendCommandFailedMessage(level.GetClient());
            }
        }
    }
}