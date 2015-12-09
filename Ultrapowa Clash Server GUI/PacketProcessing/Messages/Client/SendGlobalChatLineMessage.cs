using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Ultrapowa_Clash_Server_GUI.Helpers;
using Ultrapowa_Clash_Server_GUI.Logic;
using Ultrapowa_Clash_Server_GUI.Network;
using Ultrapowa_Clash_Server_GUI.Core;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    //14715
    class SendGlobalChatLineMessage : Message
    {
        public SendGlobalChatLineMessage(Client client, BinaryReader br) : base(client, br)
        {
        }

        public override void Decode()
        {
            using (var br = new BinaryReader(new MemoryStream(GetData())))
            {
                Message = br.ReadScString();
            }
        }

        public String Message { get; set; }

        public override void Process(Level level)
        {
            if (Message.Length > 0)
            {
                if (Message[0] == '/')
                {
                    object obj = GameOpCommandFactory.Parse(Message);
                    if (obj != null)
                    {
                        string player = "";
                        if (level != null)
                            player += " (" + level.GetPlayerAvatar().GetId() + ", " + level.GetPlayerAvatar().GetAvatarName() + ")";
                        Debugger.WriteLine("\t" + obj.GetType().Name + player);
                        ((GameOpCommand)obj).Execute(level);
                    }
                }
                else
                {
                    long senderId = level.GetPlayerAvatar().GetId();
                    string senderName = level.GetPlayerAvatar().GetAvatarName();
                    foreach (var onlinePlayer in ResourcesManager.GetOnlinePlayers())
                    {
                        var p = new GlobalChatLineMessage(onlinePlayer.GetClient());
                        if (onlinePlayer.GetAccountPrivileges() > 0)
                            p.SetPlayerName(senderName + " #" + senderId);
                        else
                            p.SetPlayerName(senderName);
                        /*{
                            string fileName = @"filter\filter.txt";
                            var lines = File.ReadAllLines(fileName);
                            foreach (var line in lines)
                            {
                                if (Message.Contains(line))
                                {
                                    p.SetChatMessage("******");
                                    p.SetPlayerId(senderId);
                                    PacketManager.ProcessOutgoingPacket(p);
                                }
                                else
                                {*/
                        p.SetChatMessage(this.Message);
                        p.SetPlayerId(senderId);
                        p.SetLeagueId(level.GetPlayerAvatar().GetLeagueId());
                        PacketManager.ProcessOutgoingPacket(p);
                    }
                }
            }
        }
    }
}
