using System;
using Ultrapowa_Clash_Server_GUI.Logic;
using Ultrapowa_Clash_Server_GUI.Network;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    internal class HelpGameOpCommand : GameOpCommand
    {
        private readonly string[] m_vArgs;

        public HelpGameOpCommand(string[] args)
        {
            m_vArgs = args;
            SetRequiredAccountPrivileges(1);
        }

        public override void Execute(Level level)
        {
            if (level.GetAccountPrivileges() >= GetRequiredAccountPrivileges())
            {
                if (m_vArgs.Length <= 1)
                {
                    var mail = new AllianceMailStreamEntry();
                    mail.SetId((int) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
                    mail.SetSenderId(0);
                    mail.SetSenderAvatarId(0);
                    mail.SetSenderName("Shard Info Bot");
                    mail.SetIsNew(0);
                    mail.SetAllianceId(0);
                    mail.SetAllianceBadgeData(0);
                    mail.SetAllianceName("UCS Shard Bots");
                    mail.SetMessage(
                        "Your Security Level: Moderator\n\nHere are the available commands for you:\n\n/rename #ID - This will rename a user\n/kick #ID - This will kick a user");
                    mail.SetSenderLeagueId(22);

                    var p = new AvatarStreamEntryMessage(level.GetClient());
                    p.SetAvatarStreamEntry(mail);
                    PacketManager.ProcessOutgoingPacket(p);
                }
                if (m_vArgs.Length <= 2)
                {
                    var mail = new AllianceMailStreamEntry();
                    mail.SetId((int) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
                    mail.SetSenderId(0);
                    mail.SetSenderAvatarId(0);
                    mail.SetSenderName("Shard Info Bot");
                    mail.SetIsNew(0);
                    mail.SetAllianceId(0);
                    mail.SetAllianceBadgeData(0);
                    mail.SetAllianceName("UCS Shard Bots");
                    mail.SetMessage(
                        "Your Security Level: High Moderator\n\nHere are the available commands for you:\n\n/rename #ID - This will rename a user\n/kick #ID - This will kick a user\n/Ban #ID - This will ban a user\n/unban #ID - This will unban a user");
                    mail.SetSenderLeagueId(22);

                    var p = new AvatarStreamEntryMessage(level.GetClient());
                    p.SetAvatarStreamEntry(mail);
                    PacketManager.ProcessOutgoingPacket(p);
                }
                if (m_vArgs.Length <= 3)
                {
                    var mail = new AllianceMailStreamEntry();
                    mail.SetId((int) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
                    mail.SetSenderId(0);
                    mail.SetSenderAvatarId(0);
                    mail.SetSenderName("Shard Info Bot");
                    mail.SetIsNew(0);
                    mail.SetAllianceId(0);
                    mail.SetAllianceBadgeData(0);
                    mail.SetAllianceName("UCS Shard Bots");
                    mail.SetMessage(
                        "Your Security Level: Unused\n\nHere are the available commands for you:\n\n/rename #ID - This will rename a user\n/kick #ID - This will kick a user\n/Ban #ID - This will ban a user\n/unban #ID - This will unban a user\n");
                    mail.SetSenderLeagueId(22);

                    var p = new AvatarStreamEntryMessage(level.GetClient());
                    p.SetAvatarStreamEntry(mail);
                    PacketManager.ProcessOutgoingPacket(p);
                }
                if (m_vArgs.Length <= 4)
                {
                    var mail = new AllianceMailStreamEntry();
                    mail.SetId((int) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
                    mail.SetSenderId(0);
                    mail.SetSenderAvatarId(0);
                    mail.SetSenderName("Shard Info Bot");
                    mail.SetIsNew(0);
                    mail.SetAllianceId(0);
                    mail.SetAllianceBadgeData(0);
                    mail.SetAllianceName("UCS Shard Bots");
                    mail.SetMessage(
                        "Your Security Level: Administrator\n\nHere are the available commands for you:\n\n/rename #ID - This will rename a user\n/kick #ID - This will kick a user\n/Ban #ID - This will ban a user\n/unban #ID - This will unban a user");
                    mail.SetSenderLeagueId(22);

                    var p = new AvatarStreamEntryMessage(level.GetClient());
                    p.SetAvatarStreamEntry(mail);
                    PacketManager.ProcessOutgoingPacket(p);
                }
                if (m_vArgs.Length <= 5)
                {
                    var mail = new AllianceMailStreamEntry();
                    mail.SetId((int) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
                    mail.SetSenderId(0);
                    mail.SetSenderAvatarId(0);
                    mail.SetSenderName("Shard Info Bot");
                    mail.SetIsNew(0);
                    mail.SetAllianceId(0);
                    mail.SetAllianceBadgeData(0);
                    mail.SetAllianceName("UCS Shard Bots");
                    mail.SetMessage(
                        "Your Security Level: Server Owner\n\nHere are the available commands for you:\n\n/rename #ID - This will rename a user\n/kick #ID - This will kick a user\n/Ban #ID - This will ban a user\n/unban #ID - This will unban a user\n/SetLeague #ID #League - Set a league\n/Setscore #ID #Score - This will set trophies\n/Setprivileges #ID #Rank - Set the rank of another player\n/Shutdown - Initiates Shutdown\n/restart - This will initiate the restart of the server.");
                    mail.SetSenderLeagueId(22);

                    var p = new AvatarStreamEntryMessage(level.GetClient());
                    p.SetAvatarStreamEntry(mail);
                    PacketManager.ProcessOutgoingPacket(p);
                }
                else
                {
                    SendCommandFailedMessage(level.GetClient());
                }
            }
        }
    }
}