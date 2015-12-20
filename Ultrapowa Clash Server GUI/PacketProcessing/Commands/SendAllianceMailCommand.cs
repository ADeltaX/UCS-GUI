using System;
using System.IO;
using Ultrapowa_Clash_Server_GUI.Core;
using Ultrapowa_Clash_Server_GUI.Helpers;
using Ultrapowa_Clash_Server_GUI.Logic;
using Ultrapowa_Clash_Server_GUI.Network;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    //Commande 0x219
    internal class SendAllianceMailCommand : Command
    {
        private readonly string m_vMailContent;

        public SendAllianceMailCommand(BinaryReader br)
        {
            m_vMailContent = br.ReadScString();
            br.ReadInt32WithEndian();
        }

        public override void Execute(Level level)
        {
            var avatar = level.GetPlayerAvatar();
            var allianceId = avatar.GetAllianceId();
            if (allianceId > 0)
            {
                var alliance = ObjectManager.GetAlliance(allianceId);
                if (alliance != null)
                {
                    var mail = new AllianceMailStreamEntry();
                    mail.SetId((int) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
                    mail.SetAvatar(avatar);
                    mail.SetIsNew(0);
                    mail.SetSenderId(avatar.GetId());
                    mail.SetAllianceId(allianceId);
                    mail.SetAllianceBadgeData(alliance.GetAllianceBadgeData());
                    mail.SetAllianceName(alliance.GetAllianceName());
                    mail.SetMessage(m_vMailContent);

                    foreach (var onlinePlayer in ResourcesManager.GetOnlinePlayers())
                    {
                        if (onlinePlayer.GetPlayerAvatar().GetAllianceId() == allianceId)
                        {
                            var p = new AvatarStreamEntryMessage(onlinePlayer.GetClient());
                            p.SetAvatarStreamEntry(mail);
                            PacketManager.ProcessOutgoingPacket(p);
                        }
                    }
                }
            }
        }
    }
}