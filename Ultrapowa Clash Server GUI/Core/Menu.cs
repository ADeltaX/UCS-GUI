using System;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using Ultrapowa_Clash_Server_GUI.Logic;
using Ultrapowa_Clash_Server_GUI.Network;
using Ultrapowa_Clash_Server_GUI.PacketProcessing;
using Message = Ultrapowa_Clash_Server_GUI.PacketProcessing.Message;

namespace Ultrapowa_Clash_Server_GUI.Core
{
    internal class Menu

    {

        public Menu()
        {
            while (true)
            {
                Console.WriteLine("");
                var line = Console.ReadLine();
                if (line == "/startx")
                {
                    //Application.Run(new UCSManager());
                }
                else if (line == "/reloadfilter")
                {
                    Console.WriteLine("Filter Has Been Reload");
                    Message.ReloadChatFilterList();
                }
                else if (line == "/shutdown")
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
                    var mail = new AllianceMailStreamEntry();
                    mail.SetId((int) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
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
                    Program.RestartProgram();
                }
                else if (line == "/update")
                {
                    var downloadUrl = "";
                    Version newVersion = null;
                    var aboutUpdate = "";
                    var xmlUrl = "https://www.flamewall.net/ucs/system.xml";
                    XmlTextReader reader = null;
                    try
                    {
                        reader = new XmlTextReader(xmlUrl);
                        reader.MoveToContent();
                        var elementName = "";
                        if ((reader.NodeType == XmlNodeType.Element) && (reader.Name == "appinfo"))
                        {
                            while (reader.Read())
                            {
                                if (reader.NodeType == XmlNodeType.Element)
                                {
                                    elementName = reader.Name;
                                }
                                else
                                {
                                    if ((reader.NodeType == XmlNodeType.Text) && reader.HasValue)
                                        switch (elementName)
                                        {
                                            case "version":
                                                newVersion = new Version(reader.Value);
                                                break;

                                            case "url":
                                                downloadUrl = reader.Value;
                                                break;

                                            case "about":
                                                aboutUpdate = reader.Value;
                                                break;
                                        }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    finally
                    {
                        if (reader != null)
                            reader.Close();
                    }
                    var applicationVersion = Assembly.GetExecutingAssembly().GetName().Version;
                    if (applicationVersion.CompareTo(newVersion) < 0)
                    {
                        var str =
                            string.Format(
                                "New version found!\nYour version: {0}.\nNewest version: {1}. \nAdded in this version: {2}. ",
                                applicationVersion, newVersion, aboutUpdate);
                        if (DialogResult.No !=
                            MessageBox.Show(str + "\nWould you like to download this update?", "Check for updates",
                                MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                        {
                            try
                            {
                                Process.Start(downloadUrl);
                            }
                            catch
                            {
                            }
                            return;
                        }
                        ;
                    }
                    else
                    {
                        MessageBox.Show("Your version: " + applicationVersion + "  is up to date.", "Check for Updates",
                            MessageBoxButtons.OK, MessageBoxIcon.None);
                    }
                }
                else if (line == "/status")
                {
                    var IPM = MainWindow.RemoteWindow.GetIP();
                    Console.WriteLine("Server IP : " + IPM + " on port 9339");
                    Console.WriteLine("Online Player : " + ResourcesManager.GetOnlinePlayers().Count);
                    Console.WriteLine("Connected Player : " + ResourcesManager.GetConnectedClients().Count);
                    Console.WriteLine("Starting Gold : " + int.Parse(ConfigurationManager.AppSettings["StartingGold"]));
                    Console.WriteLine("Starting Elixir : " +
                                      int.Parse(ConfigurationManager.AppSettings["StartingElixir"]));
                    Console.WriteLine("Starting Dark Elixir : " +
                                      int.Parse(ConfigurationManager.AppSettings["StartingDarkElixir"]));
                    Console.WriteLine("Starting Gems : " + int.Parse(ConfigurationManager.AppSettings["StartingGems"]));
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
                        Console.WriteLine("Maintance time : " +
                                          Convert.ToInt32(ConfigurationManager.AppSettings["maintenanceTimeleft"]) +
                                          " Seconds");
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
                    var mail = new AllianceMailStreamEntry();
                    mail.SetId((int) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
                    mail.SetSenderId(0);
                    mail.SetSenderAvatarId(0);
                    mail.SetSenderName("System Manager");
                    mail.SetIsNew(0);
                    mail.SetAllianceId(0);
                    mail.SetAllianceBadgeData(0);
                    mail.SetAllianceName("Legendary Administrator");
                    mail.SetMessage("Latest Server Status:\nConnected Players:" +
                                    ResourcesManager.GetConnectedClients().Count + "\nIn Memory Alliances:" +
                                    ObjectManager.GetInMemoryAlliances().Count + "\nIn Memory Levels:" +
                                    ResourcesManager.GetInMemoryLevels().Count);
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
                    Console.WriteLine("/startx - This commands start the server Gui");
                    Console.WriteLine("");
                    Console.WriteLine("/restart - This commands restart server and sending online player info about it.");
                    Console.WriteLine("");
                    Console.WriteLine("/update - This commands check for new UCS update");
                    Console.WriteLine("");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine(
                        "/shutdown - This commands fully close the server with message after five minutes.");
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
                    Console.WriteLine(
                        "/sysinfo - This command will send the current Server Status to all online players.");
                    Console.WriteLine("");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("/reloadfilter - This commands reload in memory filtered texts");
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine("");
                    Console.WriteLine("Available commands :");
                    Console.WriteLine("");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("/startx - This commands start the server Gui");
                    Console.WriteLine("");
                    Console.WriteLine("/restart - This commands restart server and sending online player info about it.");
                    Console.WriteLine("");
                    Console.WriteLine("/update - This commands check for new UCS update");
                    Console.WriteLine("");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine(
                        "/shutdown - This commands fully close the server with message after five minutes.");
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
                    Console.WriteLine(
                        "/sysinfo - This command will send the current Server Status to all online players.");
                    Console.WriteLine("");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("/reloadfilter - This commands reload in memory filtered texts");
                    Console.ResetColor();
                }
            }
        }
    }
}