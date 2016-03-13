using System;
using Ultrapowa_Clash_Server_GUI.Logic;
using Ultrapowa_Clash_Server_GUI.Network;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    internal class FaqGameOpCommand : GameOpCommand
    {
        private readonly string[] m_vArgs;

        public FaqGameOpCommand(string[] args)
        {
            m_vArgs = args;
            SetRequiredAccountPrivileges(0);
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
                    mail.SetSenderName("UCS Information Bot");
                    mail.SetIsNew(0);
                    mail.SetAllianceId(0);
                    mail.SetAllianceBadgeData(0);
                    mail.SetAllianceName("UCS System");
                    mail.SetMessage(
                        "UCS Game Server FAQ:\n\nHow long is this server online?\nThis Server is 24/7 online every day in the week.\n\nWhat is UCS Shard?\nUCS Shard is the OFFICIAL Test server of the famous CoC Emulator: Ultrapowa Clash Server.\n\nIs my progress saved?\nYes, we save everything automatically.\n\nWhere can I find the homepage with the forum?\nHomePage: http://ultrapowa.com/shard \n\nMy Friends want to player here too, how can they play here?\nGive them our APK or Server IP, you can find both at the forums!\n\n\nGreetings,\n\nUCS Shard Team");
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