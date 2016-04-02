using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using UCS.Core;
using UCS.Core.Threading;
using UCS.Logic;
using UCS.Network;
using UCS.PacketProcessing;
using UCS.Sys;

namespace UCS.Helpers
{
    class CommandParser
    {

        public static void CommandRead(string cmd)
        {
            if (cmd == null) if (ConfUCS.IsConsoleMode) ManageConsole();
            try
            {
                switch (cmd.ToLower())
                {
                    case "/help":

                        Console.WriteLine("/start                             <-- Start the server");
                        Console.WriteLine("/ban <PlayerID>                    <-- Ban a client");
                        // Console.WriteLine("/banip <PlayerID>                  <-- Ban a client by IP");
                        Console.WriteLine("/unban <PlayerID>                  <-- Unban a client");
                        // Console.WriteLine("/unbanip <PlayerID>                <-- Unban a client");
                        // Console.WriteLine("/tempban <PlayerID> <Seconds>      <-- Temporary ban a client");
                        // Console.WriteLine("/tempbanip <PlayerID> <Seconds>    <-- Temporary ban a client by IP");
                        Console.WriteLine("/kick <PlayerID>                   <-- Kick a client from the server");
                        // Console.WriteLine("/mute <PlayerID>                   <-- Mute a client");
                        // Console.WriteLine("/unmute <PlayerID>                 <-- Unmute a client");
                        // Console.WriteLine("/setlevel <PlayerID> <Level>       <-- Set a level for a player");
                        Console.WriteLine("/update                            <-- Check if update is available");
                        // Console.WriteLine("/say <Text>                        <-- Send a text to all");
                        // Console.WriteLine("/sayplayer <PlayerID> <Text>       <-- Send a text to a player");
                        Console.WriteLine("/stop  or   /shutdown              <-- Stop the server and save data");
                        Console.WriteLine("/forcestop                         <-- Force stop the server");
                        Console.WriteLine("/restart                           <-- Save data and then restart");
                        Console.WriteLine("/send sysinfo                      <-- Send server info to all players");
                        Console.WriteLine("/status                            <-- Get server status");
                        Console.WriteLine("/switch                            <-- Switch to GUI/Console mode");
                        break;

                    case "/start":

                        if (!ConfUCS.IsServerOnline)
                        {
                            ConsoleThread CT = new ConsoleThread();
                            CT.Start();
                        }
                        else Console.WriteLine("Server already online!");
                        break;

                    case "/stop":
                    case "/shutdown":

                        Console.WriteLine("Shutting down... Saving all data, wait.");

                        foreach (var onlinePlayer in ResourcesManager.GetOnlinePlayers())
                        {
                            var p = new ShutdownStartedMessage(onlinePlayer.GetClient());
                            p.SetCode(5);
                            PacketManager.ProcessOutgoingPacket(p);
                        }

                        ConsoleManage.FreeConsole();
                        Environment.Exit(0);
                        break;

                    case "/forcestop":

                        Console.WriteLine("Force shutting down... All progress not saved will be lost!");
                        Process.GetCurrentProcess().Kill();
                        break;

                    case "/uptime":

                        Console.WriteLine("Up time: " + ControlTimer.ElapsedTime);
                        break;

                    case "/restart":

                        Console.WriteLine("System Restarting....");

                        var mail = new AllianceMailStreamEntry();
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
                        Console.WriteLine("Saving all data...");
                        foreach (var l in ResourcesManager.GetOnlinePlayers())
                        {
                            //DatabaseManager.Singelton.Save(l);
                        }

                        Console.WriteLine("Restarting now");

                        Process.Start(Application.ResourceAssembly.Location);
                        Process.GetCurrentProcess().Kill();
                        break;

                    case "/clear":

                        Console.WriteLine("Console cleared");
                        if (ConfUCS.IsConsoleMode)
                            Console.Clear();
                        else
                            MainWindow.RemoteWindow.RTB_Console.Clear();
                        break;

                    case "/status":

                        Console.WriteLine("Server IP: " + ConfUCS.GetIP() + " on port 9339");
                        Console.WriteLine("IP Address (public): " + new WebClient().DownloadString("http://bot.whatismyipaddress.com/"));
                        Console.WriteLine("Online Player: " + ResourcesManager.GetOnlinePlayers().Count);
                        Console.WriteLine("Connected Player: " + ResourcesManager.GetConnectedClients().Count);
                        Console.WriteLine("Starting Gold: " + int.Parse(ConfigurationManager.AppSettings["StartingGold"]));
                        Console.WriteLine("Starting Elixir: " +
                                          int.Parse(ConfigurationManager.AppSettings["StartingElixir"]));
                        Console.WriteLine("Starting Dark Elixir: " +
                                          int.Parse(ConfigurationManager.AppSettings["StartingDarkElixir"]));
                        Console.WriteLine("Starting Gems: " + int.Parse(ConfigurationManager.AppSettings["StartingGems"]));
                        Console.WriteLine("CoC Version: " + ConfigurationManager.AppSettings["ClientVersion"]);
                        if (Convert.ToBoolean(ConfigurationManager.AppSettings["useCustomPatch"]))
                        {
                            Console.WriteLine("Patch: Active");
                            Console.WriteLine("Patching Server: " + ConfigurationManager.AppSettings["patchingServer"]);
                        }
                        else
                        {
                            Console.WriteLine("Patch: Disable");
                        }
                        if (Convert.ToBoolean(ConfigurationManager.AppSettings["maintenanceMode"]))
                        {
                            Console.WriteLine("Maintance Mode: Active");
                            Console.WriteLine("Maintance time: " +
                                              Convert.ToInt32(ConfigurationManager.AppSettings["maintenanceTimeleft"]) +
                                              " Seconds");
                        }
                        else
                        {
                            Console.WriteLine("Maintance Mode: Disable");
                        }
                        break;

                    case "/send sysinfo":

                        Console.WriteLine("Server Status is now sent to all online players");

                        var mail1 = new AllianceMailStreamEntry();
                        mail1.SetId((int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
                        mail1.SetSenderId(0);
                        mail1.SetSenderAvatarId(0);
                        mail1.SetSenderName("System Manager");
                        mail1.SetIsNew(0);
                        mail1.SetAllianceId(0);
                        mail1.SetAllianceBadgeData(0);
                        mail1.SetAllianceName("Legendary Administrator");
                        mail1.SetMessage("Latest Server Status:\nConnected Players:" +
                                        ResourcesManager.GetConnectedClients().Count + "\nIn Memory Alliances:" +
                                        ObjectManager.GetInMemoryAlliances().Count + "\nIn Memory Levels:" +
                                        ResourcesManager.GetInMemoryLevels().Count);
                        mail1.SetSenderLeagueId(22);
                        mail1.SetSenderLevel(500);

                        foreach (var onlinePlayer in ResourcesManager.GetOnlinePlayers())
                        {
                            var p = new AvatarStreamEntryMessage(onlinePlayer.GetClient());
                            var pm = new GlobalChatLineMessage(onlinePlayer.GetClient());
                            pm.SetChatMessage("Our current Server Status is now sent at your mailbox!");
                            pm.SetPlayerId(0);
                            pm.SetLeagueId(22);
                            pm.SetPlayerName("System Manager");
                            p.SetAvatarStreamEntry(mail1);
                            PacketManager.ProcessOutgoingPacket(p);
                            PacketManager.ProcessOutgoingPacket(pm);
                        }
                        break;

                    case "/update":
                        UpdateChecker.Check();
                        break;

                    case "/kick":
                        var CommGet = cmd.Split(' ');
                        if (CommGet.Length >= 2)
                        {
                            try
                            {
                                var id = Convert.ToInt64(CommGet[1]);
                                var l = ResourcesManager.GetPlayer(id);
                                if (ResourcesManager.IsPlayerOnline(l))
                                {
                                    ResourcesManager.LogPlayerOut(l);
                                    var p = new OutOfSyncMessage(l.GetClient());
                                    PacketManager.ProcessOutgoingPacket(p);
                                }
                                else
                                {
                                    Console.WriteLine("Kick failed: id " + id + " not found");
                                }
                            }
                            catch (FormatException)
                            {
                                Console.WriteLine("The given id is not a valid number");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Kick failed with error: " + ex);
                            }
                        }
                        else Console.WriteLine("Not enough arguments");
                        break;

                    case "/ban":
                        var CommGet1 = cmd.Split(' ');
                        if (CommGet1.Length >= 2)
                        {
                            try
                            {
                                var id = Convert.ToInt64(CommGet1[1]);
                                var l = ResourcesManager.GetPlayer(id);
                                if (l != null)
                                {
                                    l.SetAccountStatus(99);
                                    l.SetAccountPrivileges(0);
                                    if (ResourcesManager.IsPlayerOnline(l))
                                    {
                                        var p = new OutOfSyncMessage(l.GetClient());
                                        PacketManager.ProcessOutgoingPacket(p);
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Ban failed: id " + id + " not found");
                                }
                            }
                            catch (FormatException)
                            {
                                Console.WriteLine("The given id is not a valid number");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Ban failed with error: " + ex);
                            }
                        }
                        else Console.WriteLine("Not enough arguments");
                        break;

                    case "/unban":
                        var CommGet2 = cmd.Split(' ');
                        if (CommGet2.Length >= 2)
                        {
                            try
                            {
                                var id = Convert.ToInt64(CommGet2[1]);
                                var l = ResourcesManager.GetPlayer(id);
                                if (l != null)
                                {
                                    l.SetAccountStatus(0);
                                }
                                else
                                {
                                    Console.WriteLine("Unban failed: id " + id + " not found");
                                }
                            }
                            catch (FormatException)
                            {
                                Console.WriteLine("The given id is not a valid number");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Unban failed with error: " + ex);
                            }
                        }
                        else Console.WriteLine("Not enough arguments");
                        break;

                    case "/switch":
                        if (ConfUCS.IsConsoleFirst)
                        {
                            Console.WriteLine("Sorry, you need to launch UCS in GUI mode first.");
                        }
                        else
                        {
                            if (ConfUCS.IsConsoleMode)
                            {
                                ConfUCS.IsConsoleMode = false;
                                ConsoleManage.HideConsole();
                                InterfaceThread.Start();
                                Console.WriteLine("Switched to GUI");
                                ControlTimer.SwitchTimer();
                            }
                            else
                            {
                                ConfUCS.IsConsoleMode = true;
                                ConsoleManage.ShowConsole();
                                Console.SetOut(AllocateConsole.StandardConsole);
                                MainWindow.RemoteWindow.Hide();
                                Console.Title = ConfUCS.UnivTitle;
                                Console.WriteLine("Switched to Console");
                                ControlTimer.SwitchTimer();
                                ManageConsole();
                            }
                        }
                        break;

                    default:
                        Console.WriteLine("Unknown command. Type \"/help\" for a list containing all available commands.");
                        break;

                }
            }
            catch (Exception)
            {
                Console.WriteLine("Something wrong happens...");
                //throw;
            }
            

            //else if (cmd.ToLower().StartsWith("/mute"))
            //{
            //    var CommGet = cmd.Split(' ');
            //    if (CommGet.Length >= 2)
            //    {
            //        try
            //        {
            //            var id = Convert.ToInt64(CommGet[1]);
            //            var l = ResourcesManager.GetPlayer(id);
            //            if (ResourcesManager.IsPlayerOnline(l))
            //            {
            //                var p = new BanChatTrigger(l.GetClient());
            //                p.SetCode(999999999);
            //                PacketManager.ProcessOutgoingPacket(p);
            //            }
            //            else
            //            {
            //                Console.WriteLineDebug("Chat Mute failed: id " + id + " not found", CoreWriter.level.DEBUGLOG);
            //            }
            //        }
            //        catch (FormatException)
            //        {
            //            Console.WriteLineDebug("The given id is not a valid number", CoreWriter.level.DEBUGFATAL);
            //        }
            //        catch (Exception ex)
            //        {
            //            Console.WriteLineDebug("Chat Mute failed with error: " + ex, CoreWriter.level.DEBUGFATAL);
            //        }
            //    }
            //    else Console.WriteLineDebug("Not enough arguments", CoreWriter.level.DEBUGFATAL);
            //}


           if (ConfUCS.IsConsoleMode) ManageConsole();
        }

        public static void ManageConsole()
        {
            CommandRead(Console.ReadLine());
        }
    }
}
