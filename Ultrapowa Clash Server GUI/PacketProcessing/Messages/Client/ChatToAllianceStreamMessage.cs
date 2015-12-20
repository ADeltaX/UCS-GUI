using System;
using System.IO;
using Ultrapowa_Clash_Server_GUI.Core;
using Ultrapowa_Clash_Server_GUI.Helpers;
using Ultrapowa_Clash_Server_GUI.Logic;
using Ultrapowa_Clash_Server_GUI.Network;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    //14315
    internal class ChatToAllianceStreamMessage : Message
    {
        private string m_vChatMessage;

        public ChatToAllianceStreamMessage(Client client, BinaryReader br) : base(client, br)
        {
        }

        public override void Decode()
        {
            using (var br = new BinaryReader(new MemoryStream(GetData())))
            {
                m_vChatMessage = br.ReadScString();
            }
        }

        public override void Process(Level level)
        {
            var avatar = level.GetPlayerAvatar();
            var allianceId = avatar.GetAllianceId();
            if (allianceId > 0)
            {
                if (m_vChatMessage.Length > 0)
                {
                    if (m_vChatMessage[0] == '/')
                    {
                        var obj = GameOpCommandFactory.Parse(m_vChatMessage);
                        if (obj != null)
                        {
                            var player = "";
                            if (level != null)
                                player += " (" + avatar.GetId() + ", " + avatar.GetAvatarName() + ")";
                            MainWindow.RemoteWindow.WriteConsoleDebug("\t" + obj.GetType().Name + player, (int)MainWindow.level.DEBUGLOG);
                            ((GameOpCommand) obj).Execute(level);
                        }
                    }
                    else
                    {
                        var cm = new ChatStreamEntry();
                        cm.SetId((int) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
                        cm.SetAvatar(avatar);
                        cm.SetMessage(FilterString(m_vChatMessage));

                        var alliance = ObjectManager.GetAlliance(allianceId);
                        if (alliance != null)
                        {
                            alliance.AddChatMessage(cm);

                            foreach (var onlinePlayer in ResourcesManager.GetOnlinePlayers())
                            {
                                if (onlinePlayer.GetPlayerAvatar().GetAllianceId() == allianceId)
                                {
                                    var p = new AllianceStreamEntryMessage(onlinePlayer.GetClient());
                                    var name = cm.GetSenderName();
                                    if (onlinePlayer.isPermittedUser())
                                    {
                                        cm.SetSenderName(name + " #" + cm.GetSenderId());
                                    }

                                    p.SetStreamEntry(cm);
                                    PacketManager.ProcessOutgoingPacket(p);

                                    if (onlinePlayer.isPermittedUser())
                                    {
                                        cm.SetSenderName(name);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}