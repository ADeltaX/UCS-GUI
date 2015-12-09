using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Net;
using System.Threading.Tasks;
using Ultrapowa_Clash_Server_GUI.PacketProcessing;
using Ultrapowa_Clash_Server_GUI.Logic;
using Ultrapowa_Clash_Server_GUI.Helpers;
using Ultrapowa_Clash_Server_GUI.GameFiles;
using Ultrapowa_Clash_Server_GUI.Network;

namespace Ultrapowa_Clash_Server_GUI.Core
{
    class Menu
    {
        private string[] m_vArgs;
        public Menu()
        {
            while (true)
            {
                Console.WriteLine("");
                string line = Console.ReadLine();
                if (line == "/shutdown")
                {
                    foreach (var onlinePlayer in ResourcesManager.GetOnlinePlayers())
                    {
                        var p = new ShutdownStartedMessage(onlinePlayer.GetClient());
                        p.SetCode(5);
                        PacketManager.ProcessOutgoingPacket(p);
                    }
                    Console.WriteLine("Message has been send to the user");
                }
                else if (line == "/clear")
                {
                    Console.Clear();
                }
                else if (line == "/restart")
                {
                    AllianceMailStreamEntry mail = new AllianceMailStreamEntry();
                    mail.SetId((int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
                    mail.SetSenderId(0);
                    mail.SetSenderAvatarId(0);
                    mail.SetSenderName("System Manager");
                    mail.SetIsNew(0);
                    mail.SetAllianceId(0);
                    mail.SetAllianceBadgeData(0);
                    mail.SetAllianceName("Legendary Administrator");
                    mail.SetMessage("System is about to restart in a few moments.");
                    mail.SetSenderLevel(500);
                    mail.SetSenderLeagueId(22);

                    foreach (var onlinePlayer in ResourcesManager.GetOnlinePlayers())
                    {
                        var pm = new GlobalChatLineMessage(onlinePlayer.GetClient());
                        var ps = new ShutdownStartedMessage(onlinePlayer.GetClient());
                        var p = new AvatarStreamEntryMessage(onlinePlayer.GetClient());
                        ps.SetCode(5);
                        p.SetAvatarStreamEntry(mail);
                        pm.SetChatMessage("System is about to restart in a few moments.");
                        pm.SetPlayerId(0);
                        pm.SetLeagueId(22);
                        pm.SetPlayerName("System Manager");
                        PacketManager.ProcessOutgoingPacket(p);
                        PacketManager.ProcessOutgoingPacket(ps);
                        PacketManager.ProcessOutgoingPacket(pm);
                    }
                    Console.WriteLine("System Restarting....");
                    System.Diagnostics.Process.Start(@"tools\Ultrapowa_Clash_Server_GUI-restart.bat");
                }
                else if (line == "/status")
                {
                    string hostName = Dns.GetHostName();
                    string IPM = Dns.GetHostByName(hostName).AddressList[0].ToString();
                    Console.WriteLine("Server IP : " + IPM + " on port 9339");
                    Console.WriteLine("Players Online : " + ResourcesManager.GetOnlinePlayers().Count);
                    Console.WriteLine("Starting Gold : " + Int32.Parse(ConfigurationManager.AppSettings["StartingGold"]));
                    Console.WriteLine("Starting Elixir : " + Int32.Parse(ConfigurationManager.AppSettings["StartingElixir"]));
                    Console.WriteLine("Starting Dark Elixir : " + Int32.Parse(ConfigurationManager.AppSettings["StartingDarkElixir"]));
                    Console.WriteLine("Starting Gems : " + Int32.Parse(ConfigurationManager.AppSettings["StartingGems"]));
                    Console.WriteLine("CoC Version : " + ConfigurationManager.AppSettings["ClientVersion"]);
                    if (Convert.ToBoolean(ConfigurationManager.AppSettings["useCustomPatch"]))
                    {
                        Console.WriteLine("Patch : Active");
                        Console.WriteLine("Patching Server : " + ConfigurationManager.AppSettings["patchingServer"]);
                    }
                    else
                    {
                        Console.WriteLine("Patch : Disable");
                    }
                    if (Convert.ToBoolean(ConfigurationManager.AppSettings["maintenanceMode"]))
                    {
                        Console.WriteLine("Maintance Mode : Active");
                        Console.WriteLine("Maintance time : " + (Convert.ToInt32(ConfigurationManager.AppSettings["maintenanceTimeleft"])) + " Seconds");
                    }
                    else
                    {
                        Console.WriteLine("Maintance Mode : Disable");
                    }
                }
                else if (line == "/sysinfo")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Server Status is now sent to all online players");
                    AllianceMailStreamEntry mail = new AllianceMailStreamEntry();
                    mail.SetId((int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
                    mail.SetSenderId(0);
                    mail.SetSenderAvatarId(0);
                    mail.SetSenderName("System Manager");
                    mail.SetIsNew(0);
                    mail.SetAllianceId(0);
                    mail.SetAllianceBadgeData(0);
                    mail.SetAllianceName("Legendary Administrator");
                    mail.SetMessage("Latest Server Status:\nConnected Players:" + ResourcesManager.GetConnectedClients().Count + "\nIn Memory Alliances:" + ObjectManager.GetInMemoryAlliances().Count + "\nIn Memory Levels:" + ResourcesManager.GetInMemoryLevels().Count);
                    mail.SetSenderLeagueId(22);
                    mail.SetSenderLevel(500);

                    foreach (var onlinePlayer in ResourcesManager.GetOnlinePlayers())
                    {
                        var p = new AvatarStreamEntryMessage(onlinePlayer.GetClient());
                        var pm = new GlobalChatLineMessage(onlinePlayer.GetClient());
                        pm.SetChatMessage("Our current Server Status is now sent at your mailbox!");
                        pm.SetPlayerId(0);
                        pm.SetLeagueId(22);
                        pm.SetPlayerName("System Manager");
                        p.SetAvatarStreamEntry(mail);
                        PacketManager.ProcessOutgoingPacket(p);
                        PacketManager.ProcessOutgoingPacket(pm);
                    }
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else if (line == "/help")
                {
                    Console.WriteLine("");
                    Console.WriteLine("Available commands :");
                    Console.WriteLine("");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("/restart - This commands restart server and sending online player info about it.");
                    Console.WriteLine("");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("/shutdown - This commands fully close the server with message after five minutes.");
                    Console.WriteLine("");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("/status - This commands show informations about the server.");
                    Console.WriteLine("");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("/clear - Clean the emulator screen");
                    Console.WriteLine("");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("/help - This commands show a list of available commands.");
                    Console.WriteLine("");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("/sysinfo - This command will send the current Server Status to all online players.");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.WriteLine("");
                    Console.WriteLine("Available commands :");
                    Console.WriteLine("");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("/restart - This commands restart server and sending online player info about it.");
                    Console.WriteLine("");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("/shutdown - This commands fully close the server with message after five minutes.");
                    Console.WriteLine("");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("/status - This commands show informations about the server.");
                    Console.WriteLine("");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("/clear - Clean the emulator screen");
                    Console.WriteLine("");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("/help - This commands show a list of available commands.");
                    Console.WriteLine("");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("/sysinfo - This command will send the current Server Status to all online players.");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
        }
    }
}
        

