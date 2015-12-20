using System;
using Ultrapowa_Clash_Server_GUI.Core;
using Ultrapowa_Clash_Server_GUI.Network; 
using Ultrapowa_Clash_Server_GUI.Logic;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    class PrivateMessageCommand : GameOpCommand
    {
        private string[] m_vArgs;

        public PrivateMessageCommand(string[] args)
        {
            m_vArgs = args;
            SetRequiredAccountPrivileges(0);
        }

        public override void Execute(Level level)
        {
            if (level.GetAccountPrivileges() >= GetRequiredAccountPrivileges())
            {
                try
                {
                    if (m_vArgs.Length >= 2)
                    {
                        var senderId = level.GetPlayerAvatar().GetId();
                        var c = level.GetClient();
                        var argcount = m_vArgs.Length;
                        int currentarg = 2;
                        var msg = "";
                        while (currentarg < m_vArgs.Length)
                        {
                            msg += m_vArgs[currentarg] + " ";
                            currentarg++;
                        }
                        var message = msg;
                        long id = Convert.ToInt64(m_vArgs[1]);

                        bool playerexist;
                        try
                        {
                            var l = ResourcesManager.GetPlayer(id);
                            playerexist = true;
                        }
                        catch
                        {
                            playerexist = false;
                        }
                        if (playerexist.Equals(true))
                        {
                            var l = ResourcesManager.GetPlayer(id);
                            var sendername = level.GetPlayerAvatar().GetAvatarName();
                            var senderalliance = ObjectManager.GetAlliance(level.GetPlayerAvatar().GetAllianceId());
                            if (ResourcesManager.IsPlayerOnline(l))
                            {
                                var utmclient = l.GetClient();
                                var pm = new GlobalChatLineMessage(utmclient);
                                pm.SetChatMessage(Message.FilterString(message));
                                pm.SetPlayerName(sendername + " (Private Message)");
                                pm.SetPlayerId(senderId);
                                pm.SetLeagueId(l.GetPlayerAvatar().GetLeagueId());
                                pm.SetAlliance(senderalliance);
                                PacketManager.ProcessOutgoingPacket(pm);
                                var resp = new GlobalChatLineMessage(c);
                                resp.SetChatMessage("Successfully sent Private Message to: " + l.GetPlayerAvatar().GetAvatarName() + " with the Text: " + Message.FilterString(message));
                                resp.SetPlayerId(senderId);
                                resp.SetAlliance(senderalliance);
                                resp.SetLeagueId(22);
                                PacketManager.ProcessOutgoingPacket(resp);
                            }
                            else
                            {

                                var pm = new GlobalChatLineMessage(c);
                                pm.SetChatMessage("Player is offline or not exist.");
                                pm.SetPlayerId(0);
                                pm.SetPlayerName("System Manager");
                                pm.SetLeagueId(22);
                                PacketManager.ProcessOutgoingPacket(pm);
                            }
                        }
                    }
                    else
                    {
                        var p = new GlobalChatLineMessage(level.GetClient());
                        p.SetChatMessage("Wrong usage,Please use /pm userid text\nTo get your ID use /myid. Save it or tell it to your friends.");
                        p.SetPlayerId(0);
                        p.SetLeagueId(22);
                        p.SetPlayerName("System Manager");
                    }
                }
                catch (Exception e)
                {
                    var p = new GlobalChatLineMessage(level.GetClient());
                    p.SetChatMessage("Wrong usage,Please use /pm userid text\nTo get your ID use /myid. Save it or tell it to your friends.");
                    p.SetPlayerId(0);
                    p.SetLeagueId(22);
                    p.SetPlayerName("System Manager");
                    PacketManager.ProcessOutgoingPacket(p);
                    MainWindow.RemoteWindow.WriteConsoleDebug("Exception on PM caught.: " + e, (int)MainWindow.level.DEBUGFATAL);
                }
            }
            else
            {
                SendCommandFailedMessage(level.GetClient());
            }
        }
    }
}