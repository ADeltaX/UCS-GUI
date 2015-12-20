using System.IO;
using Ultrapowa_Clash_Server_GUI.Core;
using Ultrapowa_Clash_Server_GUI.Helpers;
using Ultrapowa_Clash_Server_GUI.Logic;
using Ultrapowa_Clash_Server_GUI.Network;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    //14715
    internal class SendGlobalChatLineMessage : Message
    {
        public SendGlobalChatLineMessage(Client client, BinaryReader br) : base(client, br)
        {
        }

        public string MessageString { get; set; }

        public override void Decode()
        {
            using (var br = new BinaryReader(new MemoryStream(GetData())))
            {
                MessageString = br.ReadScString();
            }
        }

        public override void Process(Level level)
        {
            if (MessageString.Length > 0)
            {
                if (MessageString[0] == '/')
                {
                    var obj = GameOpCommandFactory.Parse(MessageString);
                    if (obj != null)
                    {
                        var player = "";
                        if (level != null)
                            player += " (" + level.GetPlayerAvatar().GetId() + ", " +
                                      level.GetPlayerAvatar().GetAvatarName() + ")";
                        MainWindow.RemoteWindow.WriteConsoleDebug("\t" + obj.GetType().Name + player, (int)MainWindow.level.DEBUGLOG);
                        ((GameOpCommand) obj).Execute(level);
                    }
                }
                else
                {
                    var senderId = level.GetPlayerAvatar().GetId();
                    var senderName = level.GetPlayerAvatar().GetAvatarName();
                    foreach (var onlinePlayer in ResourcesManager.GetOnlinePlayers())
                    {
                        var p = new GlobalChatLineMessage(onlinePlayer.GetClient());
                        if (onlinePlayer.GetAccountPrivileges() > 0)
                            p.SetPlayerName(senderName + " #" + senderId);
                        else
                            p.SetPlayerName(senderName);

                        p.SetChatMessage(FilterString(MessageString));

                        p.SetPlayerId(senderId);
                        p.SetLeagueId(level.GetPlayerAvatar().GetLeagueId());
                        p.SetAlliance(ObjectManager.GetAlliance(level.GetPlayerAvatar().GetAllianceId()));
                        PacketManager.ProcessOutgoingPacket(p);
                    }
                }
            }
        }
    }
}