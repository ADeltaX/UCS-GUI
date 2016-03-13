using System;
using System.IO;
using UCS.Core;
using UCS.Helpers;
using UCS.Logic;
using UCS.Network;

namespace UCS.PacketProcessing
{
    //Packet 14316

    internal class EditClanSettingsMessage : Message
    {
        private int m_vAllianceBadgeData;
        private string m_vAllianceDescription;
        private int m_vAllianceOrigin;
        private int m_vAllianceType;
        private int m_vRequiredScore;
        private int m_vWarFrequency;
        private int Unknown;

        public EditClanSettingsMessage(Client client, BinaryReader br) : base(client, br)
        {
        }

        public override void Decode()
        {
            using (var br = new BinaryReader(new MemoryStream(GetData())))
            {
                m_vAllianceDescription = br.ReadScString();
                Unknown = br.ReadInt32WithEndian();
                m_vAllianceBadgeData = br.ReadInt32WithEndian();
                m_vAllianceType = br.ReadInt32WithEndian();
                m_vRequiredScore = br.ReadInt32WithEndian();
                m_vWarFrequency = br.ReadInt32WithEndian();
                m_vAllianceOrigin = br.ReadInt32WithEndian();

                Debugger.WriteLine("\t" + m_vAllianceDescription, null, 5);
                Debugger.WriteLine("\t" + m_vAllianceBadgeData, null, 5);
                Debugger.WriteLine("\t" + m_vAllianceType, null, 5);
                Debugger.WriteLine("\t" + m_vRequiredScore, null, 5);
                Debugger.WriteLine("\t" + m_vWarFrequency, null, 5);
                Debugger.WriteLine("\t" + m_vAllianceOrigin, null, 5);
            }
        }

        public override void Process(Level level)
        {
            //Clans Edit Manager
            var alliance = ObjectManager.GetAlliance(level.GetPlayerAvatar().GetAllianceId());
            if (alliance != null)
            {
                alliance.SetAllianceDescription(m_vAllianceDescription);
                alliance.SetAllianceBadgeData(m_vAllianceBadgeData);
                alliance.SetAllianceType(m_vAllianceType);
                alliance.SetRequiredScore(m_vRequiredScore);
                alliance.SetWarFrequency(m_vWarFrequency);
                alliance.SetAllianceOrigin(m_vAllianceOrigin);

                var avatar = level.GetPlayerAvatar();
                var allianceId = avatar.GetAllianceId();
                if (allianceId > 0)
                {
                    var cm = new ChatStreamEntry();
                    cm.SetId((int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
                    cm.SetSenderId(0);
                    cm.SetHomeId(0);
                    cm.SetSenderLeagueId(22);
                    cm.SetSenderName("Clan Manager");
                    cm.SetSenderRole(2);
                    cm.SetMessage("Clan settings were updated!");

                    var alliance1 = ObjectManager.GetAlliance(allianceId);
                    if (alliance != null)
                    {
                        alliance1.AddChatMessage(cm);

                        foreach (var onlinePlayer in ResourcesManager.GetOnlinePlayers())
                        {
                            if (onlinePlayer.GetPlayerAvatar().GetAllianceId() == allianceId)
                            {
                                var p = new AllianceStreamEntryMessage(onlinePlayer.GetClient());
                                p.SetStreamEntry(cm);
                                PacketManager.ProcessOutgoingPacket(p);
                                PacketManager.ProcessOutgoingPacket(new OwnHomeDataMessage(Client, level));
                                PacketManager.ProcessOutgoingPacket(new AllianceDataMessage(Client, alliance));
                            }
                        }
                    }
                }
            }
        }
    }
}