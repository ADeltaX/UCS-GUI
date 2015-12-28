using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Collections.Generic;
using System.Threading;
using System.Configuration;
using System.Net;
using Ultrapowa_Clash_Server_GUI.Network;
using Ultrapowa_Clash_Server_GUI.Core;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using Ultrapowa_Clash_Server_GUI.Helpers;
using System.IO;
using System.Diagnostics;
using Ultrapowa_Clash_Server_GUI.Sys;
using Ultrapowa_Clash_Server_GUI.PacketProcessing;
using Ultrapowa_Clash_Server_GUI.Logic;
using System.Threading.Tasks;

namespace Ultrapowa_Clash_Server_GUI
{

    public partial class MainWindow : Window
    {
        public static MainWindow RemoteWindow = new MainWindow();
        public static bool IsFocusOk = true;
        bool ChangeUpdatePopup = false;
        static string LogPath = "NONE";
        StreamWriter LogStream = null;
        System.Timers.Timer UpdateInfo = new System.Timers.Timer();
        DispatcherTimer UpdateInfoGUI = new DispatcherTimer();
        Stopwatch HighPrecisionUpdateTimer = new Stopwatch();
        Stopwatch PerformanceCounter = new Stopwatch();
        public static readonly int port = Utils.parseConfigInt("serverPort");
        public static string ElapsedTime = "";


        List<string> CommandList;

        public bool IsServerOnline = false;

        public MainWindow()
        {
            InitializeComponent();
            int port = Utils.parseConfigInt("serverPort");
            RemoteWindow = this;

            if (ConfUCS.IsConsoleMode)
            {
                UpdateInfo.Elapsed += UpdateInfo_Tick;
                UpdateInfo.Interval = 1000;
            }
            else
            {
                UpdateInfoGUI.Tick += UpdateInfo_Tick;
                UpdateInfoGUI.Interval = new TimeSpan(10000);
            }

            if (ConfUCS.IsLogEnabled) PrepareLog();

            CommandList = new List<string>
            {
                "/say", "/ban", "/banip", "/tempban", "/tempbanip", "/unban",
                "/unbanip", "/mute","/unmute","/setlevel", "/sayplayer",
                "/kick", "/help", "/start", "/restart", "/stop", "/uptime",
                "/clear"
            };

            if (ConfUCS.IsConsoleMode==true)
            {
                Console.Clear();
                WindowState = WindowState.Minimized;
                Console.Title = "UCS Server " + ConfUCS.VersionUCS + " | " + "Codename: " + ConfUCS.Codename + " | " + "OFFLINE";
                WriteConsole("Line arg typed: /console", (int)level.LOG);
                WriteConsole("Running in Console mode...", (int)level.LOG);
                WriteConsole("Local IP: " + GetIP(), (int)level.LOG);
                WriteConsole("Ready", (int)level.LOG);
                WriteConsole("Write /start to start the server or write /help to get all commands", (int)level.LOG);
                Console.WriteLine("\n");
                Console.CursorVisible = true;
            }
            else
            {
                Title = "UCS Server " + ConfUCS.VersionUCS + " | " + "Codename: " + ConfUCS.Codename + " | " + "OFFLINE";
                WriteConsole("Loading GUI...", (int)level.LOG);
                CheckThings();
                LBL_IP.Content = "Local IP: " + GetIP();
                LBL_PORT.Content = "PORT: " + port;
                CommandLine.TextChanged += new TextChangedEventHandler(CommandLine_TextChanged);
            }
            

        }


        #region Events

        private void UpdateInfo_Tick(object sender, EventArgs e)
        {
            TimeSpan ts = HighPrecisionUpdateTimer.Elapsed;
            ElapsedTime = string.Format("{0:00}:{1:00}:{2:00}",ts.Hours, ts.Minutes, ts.Seconds + 1);

            string OutTitle = "UCS Server " + ConfUCS.VersionUCS + " | " + "Codename: " + ConfUCS.Codename + " | " + "ONLINE" + " | " + "Up time: " + ElapsedTime;

            if (ConfUCS.IsConsoleMode)
            {
                Console.Title = OutTitle;
            }
            else
            {
                Title = OutTitle;
                LBL_UpTime.Content = "Up time: " + ElapsedTime;
            }

        }

        private void CommandLine_TextChanged(object sender, TextChangedEventArgs e)
        {
            string TypedCommand = CommandLine.Text;
            List<string> Sug_List = new List<string>();
            Sug_List.Clear();

            foreach (string CM in CommandList)
                if (!string.IsNullOrEmpty(CommandLine.Text))
                    if (CM.StartsWith(TypedCommand))
                        Sug_List.Add(CM);
            
            if (Sug_List.Count > 0)
            {
                LB_CommandTypedList.ItemsSource = Sug_List;
                LB_CommandTypedList.Visibility = Visibility.Visible;
            }
            else if (Sug_List.Count > 0)
            {
                LB_CommandTypedList.ItemsSource = null;
                LB_CommandTypedList.Visibility = Visibility.Collapsed;
            }
            else
            {
                LB_CommandTypedList.ItemsSource = null;
                LB_CommandTypedList.Visibility = Visibility.Collapsed;
            }
        }

        private void LB_CommandTypedList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(LB_CommandTypedList.ItemsSource != null)
            {
                LB_CommandTypedList.Visibility = Visibility.Collapsed;
                CommandLine.TextChanged -= new TextChangedEventHandler(CommandLine_TextChanged);

                if (LB_CommandTypedList.SelectedIndex != -1)
                    CommandLine.Text = LB_CommandTypedList.SelectedItem.ToString();

                CommandLine.TextChanged += new TextChangedEventHandler(CommandLine_TextChanged);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (ConfUCS.IsConsoleMode)
            {
                Hide();
                ManageConsole();
                LaunchServer();
            }
            else
            {
                WriteConsole("GUI loaded", (int)level.LOG);
                DoAnimation();

                AsyncUtils.DelayCall(1500, () => {
                    LaunchServer();
                });

            }
        }

        private void BTN_LaunchServer_Click(object sender, RoutedEventArgs e)
        {
            if (IsServerOnline==false)
            {
                LaunchServer();
            }
            else
            {
                WriteConsole("Server already online!", (int)level.WARNING);
            }
        }

        private void BTN_Enter_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(CommandLine.Text))
                CommandRead(CommandLine.Text);
        }

        private void CommandLine_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                if (!string.IsNullOrWhiteSpace(CommandLine.Text))
                    CommandRead(CommandLine.Text);
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            if (IsFocusOk == false)
            {
                DeBlur(); //Remove the Blur effect
            }
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            if (IsFocusOk == false)
            {
                DoBlur(); //Start doing the Blur effect
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

        #region MenuItems
        private void MI_Restart_Click(object sender, RoutedEventArgs e)
        {
            CommandRead("/restart");
        }
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MI_Ban_Click(object sender, RoutedEventArgs e)
        {
            SendPopup((int)Popup.cause.BAN);
        }

        private void MI_Configuration_Click(object sender, RoutedEventArgs e)
        {
            IsFocusOk = false;
            PopupConfiguration PC = new PopupConfiguration();
            PC.Owner = this;
            PC.ShowDialog();
        }

        private void MI_Ban_IP_Click(object sender, RoutedEventArgs e)
        {
            SendPopup((int)Popup.cause.BANIP);
        }

        private void MI_TEMP_BAN_Click(object sender, RoutedEventArgs e)
        {
            SendPopup((int)Popup.cause.TEMPBAN);
        }

        private void MI_TEMP_BAN_IP_Click(object sender, RoutedEventArgs e)
        {
            SendPopup((int)Popup.cause.TEMPBANIP);
        }

        private void MI_Unban_Click(object sender, RoutedEventArgs e)
        {
            SendPopup((int)Popup.cause.UNBAN);
        }

        private void MI_Unban_IP_Click(object sender, RoutedEventArgs e)
        {
            SendPopup((int)Popup.cause.UNBANIP);
        }

        private void MI_Mute_Click(object sender, RoutedEventArgs e)
        {
            SendPopup((int)Popup.cause.MUTE);
        }

        private void MI_Unmute_Click(object sender, RoutedEventArgs e)
        {
            SendPopup((int)Popup.cause.UNMUTE);
        }

        private void MI_Kick_Click(object sender, RoutedEventArgs e)
        {
            SendPopup((int)Popup.cause.KICK);
        }

        private void MI_CheckUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (ChangeUpdatePopup == true)
            {
                IsFocusOk = false;
                PopupUpdater PopupUpdater = new PopupUpdater();
                PopupUpdater.Owner = this;
                PopupUpdater.ShowDialog();
            }
        }

        #endregion

        private void CB_Debug_Unchecked(object sender, RoutedEventArgs e)
        {
            ConfUCS.DebugMode = false;
        }

        private void CB_Debug_Checked(object sender, RoutedEventArgs e)
        {
            ConfUCS.DebugMode = true;
        }

        #endregion

        #region Do stuff

        public List<ConCatPlayers> Players = new List<ConCatPlayers>();

        public void UpdateTheListPlayers()
        {
            Players.Clear();
            Dispatcher.BeginInvoke((Action)delegate ()
            {
                listBox.ItemsSource = null;
            });
            foreach (var x in ResourcesManager.GetOnlinePlayers())
            {
                Players.Add(new ConCatPlayers { PlayerIDs = x.GetPlayerAvatar().GetId().ToString(), PlayerNames = x.GetPlayerAvatar().GetAvatarName().ToString() });
            }
            if (!ConfUCS.IsConsoleMode)
            Dispatcher.BeginInvoke((Action)delegate ()
            {
                listBox.ItemsSource = Players;
            });
        }

        private void SendPopup(int why)
        {
            if (!IsServerOnline)
            {
                WriteConsole("The server is not running", (int)level.WARNING);
            }
            else
            {
            IsFocusOk = false;
            Popup Popup = new Popup(why);
            Popup.Owner = this;
            Popup.ShowDialog();
            }
        }

        private void PrepareLog()
        {
            if (!Directory.Exists(ConfUCS.LogDirectory)) Directory.CreateDirectory(ConfUCS.LogDirectory);

            string DTT = DateTime.Now.ToString("HH:mm:ss");
            string DTD = DateTime.Now.ToString("dd/MM/yyyy");

            LogPath = ConfUCS.LogDirectory + string.Format("LOG_{0}_{1}.txt", DTD.Replace("/", "-"), DTT.Replace(":", "-"));

            try
            {
                LogStream = File.CreateText(LogPath);
            }
            catch (Exception)
            {
                WriteConsole("Cannot create log. Disabling log mode.", (int)level.FATAL);
                ConfUCS.IsLogEnabled = false;
            }
        }

        public string GetIP()
        {
            string HostName = Dns.GetHostName();
            return Dns.GetHostByName(HostName).AddressList[0].ToString();
        }

        private void DoAnimation()
        {
            //AYY LMAO

            int DeltaVariation = -100;
            AnimationLib.MoveToTargetY(CB_Debug, DeltaVariation, 0.25,50);
            AnimationLib.MoveToTargetY(LBL_UpTime, DeltaVariation, 0.25,100);
            AnimationLib.MoveToTargetY(LBL_PORT, DeltaVariation, 0.25, 150);
            AnimationLib.MoveToTargetY(LBL_IP, DeltaVariation, 0.25, 200);
            AnimationLib.MoveToTargetX(BTN_LaunchServer, DeltaVariation - 100, 0.25, 100);
            AnimationLib.MoveToTargetX(listBox, DeltaVariation - 100, 0.3, 200);
            AnimationLib.MoveToTargetX(label_player, DeltaVariation - 100, 0.35, 200);
            AnimationLib.MoveToTargetX(BTN_Enter, -DeltaVariation * 7, 0.4, 150);
            AnimationLib.MoveToTargetX(CommandLine, -DeltaVariation * 7, 0.4, 250);
            AnimationLib.MoveToTargetX(RTB_Console, -DeltaVariation * 7, 0.3, 300);
            AnimationLib.MoveToTargetX(label_console, -DeltaVariation * 7, 0.35, 300);

            //AYY LMAO 2

            AnimationLib.MoveToTargetX(MI_Menu, DeltaVariation - 600, 0.5, 0);
            AnimationLib.MoveToTargetX(SEP_1, DeltaVariation - 600, 0.5, 0);
            AnimationLib.MoveToTargetX(MI_Comands, DeltaVariation - 600, 0.5, 50);
            AnimationLib.MoveToTargetX(SEP_2, DeltaVariation - 600, 0.5, 50);
            AnimationLib.MoveToTargetX(MI_Utility, DeltaVariation - 600, 0.5, 100);
            AnimationLib.MoveToTargetX(SEP_3, DeltaVariation - 600, 0.5, 100);
            AnimationLib.MoveToTargetX(MI_Options, DeltaVariation - 600, 0.5, 150);
            AnimationLib.MoveToTargetX(SEP_4, DeltaVariation - 600, 0.5, 150);
            AnimationLib.MoveToTargetX(MI_About, DeltaVariation - 600, 0.5, 200);
            AnimationLib.MoveToTargetX(SEP_5, DeltaVariation - 600, 0.5, 200);
            AnimationLib.MoveToTargetX(MI_Feedback, DeltaVariation - 600, 0.5, 250);
            AnimationLib.MoveToTargetX(SEP_6, DeltaVariation - 600, 0.5, 250);
            AnimationLib.MoveToTargetX(MI_CheckUpdate, DeltaVariation - 600, 0.5, 350);

        }

        private void CheckThings()
        {
            if (ConfUCS.IsUpdateAvailable== true)
            {
                MI_CheckUpdate.Header = "_UPDATE AVAILABLE";
                MI_CheckUpdate.Foreground = Brushes.Yellow;
                ChangeUpdatePopup = true;
            }
        }               

        private void LaunchServer()
        {
            if (!ConfUCS.IsConsoleMode)
            {
                LoadingServerScreen.LSS.Owner = this;
                LoadingServerScreen.LSS.Show();
            }
            if (Stopwatch.IsHighResolution) WriteConsole("Using high performance timer", (int)level.WARNING);
            else WriteConsole("Using basic performance timer", (int)level.WARNING);
            WriteConsole("Measuring loading time.", (int)level.WARNING);
            PerformanceCounter.Start();
            WriteConsole("Starting server...", (int)level.WARNING);
            WriteConsole("Loading symbols/library...", (int)level.WARNING);
            if (ConfUCS.IsConsoleMode) Console.CursorVisible = false;

            new ResourcesManager();
            new ObjectManager();
            new Gateway().Start();
            new ApiManager();

            if (!Directory.Exists("logs"))
            {
                WriteConsole("Folder \"logs/\" does not exist. Let me create one..",(int)level.WARNING);
                Directory.CreateDirectory("logs");
            }

            if (Convert.ToBoolean(Utils.parseConfigString("apiManagerPro")))
            {
                if (ConfigurationManager.AppSettings["ApiKey"] == null)
                {
                    var random = new Random();
                    var chars = "A1b5B6b7C1c5D3";
                    var ch = Convert.ToString(chars[random.Next(chars.Length)]);
                    ConfigurationManager.AppSettings.Set("ApiKey", ch);
                }
                var ws = new ApiManagerPro(ApiManagerPro.SendResponse,
                    "http://+:" + Utils.parseConfigInt("proDebugPort") + "/" + Utils.parseConfigString("ApiKey") + "/");
                ws.Run();
            }

            Logger.SetLogLevel(Utils.parseConfigInt("loggingLevel"));

            InitProgramThreads(); //With this, InitializeComponent will do a lot of time to finish the work.

            if (Utils.parseConfigInt("loggingLevel") >= 5)
            {
                WriteConsoleDebug("\t",(int)level.DEBUGLOG);
                WriteConsoleDebug("Played ID's:", (int)level.DEBUGLOG);
                foreach (var id in ResourcesManager.GetAllPlayerIds())
                {
                    WriteConsoleDebug("Played ID's:", (int)level.DEBUGLOG);
                    WriteConsoleDebug("\t" + id, (int)level.DEBUGLOG);
                }
                WriteConsoleDebug("\t", (int)level.DEBUGLOG);
            }
            try
            {
                if (!ConfUCS.IsConsoleMode) LoadingServerScreen.LSS.Close();
            }
            catch (Exception)
            {

                throw;
            }

            PerformanceCounter.Stop();

            TimeSpan PCTS = PerformanceCounter.Elapsed;
            string MeasuredPerformanceTime = string.Format("{0}", PCTS.TotalMilliseconds);
            WriteConsole(string.Format("Operation completed in {0} ms", MeasuredPerformanceTime),(int)level.WARNING);
            WriteConsole("Server started on port " + port + ". Let's play Clash of Clans!", (int)level.LOG);
            HighPrecisionUpdateTimer.Start();
            if (ConfUCS.IsConsoleMode) UpdateInfo.Start();
            else UpdateInfoGUI.Start();

            IsServerOnline = true;

            if (ConfUCS.IsConsoleMode)
            {
                Console.CursorVisible = true;
                ManageConsole();
            }
        }

        private void InitProgramThreads1()
        {
            WriteConsoleDebug("Server Thread's:", (int)level.DEBUGLOG);
            var programThreads = new List<Thread>();
            for (var i = 0; i < int.Parse(ConfigurationManager.AppSettings["programThreadCount"]); i++)
            {
                var pt = new ProgramThread();
                programThreads.Add(new Thread(pt.Start));
                programThreads[i].Start();
                WriteConsoleDebug("\tServer Running On Thread " + i, (int)level.DEBUGLOG);
            }

        }

        private void InitProgramThreads()
        {
            WriteConsoleDebug("Server Thread's:", (int)level.DEBUGLOG);
            var pt = new ProgramThread();
            pt.Start();
            WriteConsoleDebug("\tServer Running On Thread 1", (int)level.DEBUGLOG);

        }

        #region Blur Settings

        Storyboard myStoryboard = new Storyboard();
        DoubleAnimation myDoubleAnimation = new DoubleAnimation();
        BlurEffect blurEffect = new BlurEffect();
        private void DoBlur()
        {
            RegisterName("blurEffect", blurEffect);
            blurEffect.Radius = 0;
            Effect = blurEffect;

            myDoubleAnimation.From = 0;
            myDoubleAnimation.To = 15;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.125));
            myDoubleAnimation.AutoReverse = false;

            Storyboard.SetTargetName(myDoubleAnimation, "blurEffect");
            Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath(BlurEffect.RadiusProperty));
            myStoryboard.Children.Add(myDoubleAnimation);
            myStoryboard.Begin(this);
        }

        private void DeBlur()
        {
            RegisterName("blurEffect", blurEffect);
            blurEffect.Radius = 0;
            Effect = blurEffect;

            myDoubleAnimation.From = 15;
            myDoubleAnimation.To = 0;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.125));
            myDoubleAnimation.AutoReverse = false;

            Storyboard.SetTargetName(myDoubleAnimation, "blurEffect");
            Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath(BlurEffect.RadiusProperty));
            myStoryboard.Children.Add(myDoubleAnimation);
            myStoryboard.Begin(this);
        }

        #endregion

        #endregion

        #region Console RTB Setup


        public void SetupRTB(SolidColorBrush color, string text, string pretext, bool IsDebugMode=false)
        {
            
            Dispatcher.BeginInvoke((Action) delegate {

                    TextRange TXT = new TextRange(RTB_Console.Document.ContentEnd, RTB_Console.Document.ContentEnd);
                    TXT.Text = pretext + text + "\u2028";
                    try
                    {
                        TXT.ApplyPropertyValue(TextElement.ForegroundProperty, color);
                    }
                    catch (InvalidOperationException)
                    {

                    }
                    
                    RTB_Console.ScrollToEnd();

                });

        }


        public void WriteOnLog(string text,string pretext)
        {
            if (ConfUCS.IsLogEnabled)
            {
                try
                {
                    LogStream.AutoFlush = true;
                    LogStream.WriteLine(pretext + text);
                }
                catch (Exception ex)
                {
                    ConfUCS.IsLogEnabled = false;
                    WriteConsole("Error during saving log to file: " + ex.Message, (int)level.FATAL);
                    WriteConsole("Log mode disabled", (int)level.FATAL);
                }
            }
        }


       public enum level
        {
            LOG = 1,
            WARNING = 2,
            FATAL = 3,
            PLAYERMSG = 4,
            SERVERMSG = 5,
            DEBUGLOG = 6,
            DEBUGFATAL = 7
        }
        public void WriteConsole(string text, int level)
        {
            if (ConfUCS.IsConsoleMode == false)
            {
                switch (level)
                {
                    case 1:
                        SetupRTB(new SolidColorBrush(Color.FromRgb(22, 160, 133)), text, "[LOG]" + Type());
                        WriteOnLog(text, "[LOG]" + Type());  break;
                    case 2:
                        SetupRTB(Brushes.Orange, text, "[WARNING]" + Type());
                        WriteOnLog(text, "[WARNING]" + Type()); break;
                    case 3:
                        SetupRTB(Brushes.Red, text, "[FATAL]" + Type());
                        WriteOnLog(text, "[FATAL]" + Type()); break;
                }
            }
            else
            {
                switch (level)
                {
                    case 1:
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("[LOG]" + Type() + text); 
                        Console.ResetColor();
                        WriteOnLog(text, "[LOG]" + Type());
                        break;
                    case 2:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("[WARNING]"  + Type() + text);
                        Console.ResetColor();
                        WriteOnLog(text, "[WARNING]" + Type());
                        break;
                    case 3:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("[FATAL]" + Type() + text);
                        Console.ResetColor();
                        WriteOnLog(text, "[FATAL]" + Type());
                        break;
                }
            }
        }

        public void WriteConsoleDebug(string text, int level)
        {
            if (ConfUCS.DebugMode == true)
            {
                if (ConfUCS.IsConsoleMode == false)
                {
                    switch (level)
                    {
                        case 6:
                            SetupRTB(Brushes.Yellow, text, "[DEBUG-LOG]" + Type(), true); 
                            WriteOnLog(text, "[DEBUG-LOG]" + Type()); break;
                        case 7:
                            SetupRTB(Brushes.LightYellow, text, "[DEBUG-FATAL]" + Type(), true); 
                            WriteOnLog(text, "[DEBUG-FATAL]" + Type()); break;
                    }
                }
                else
                {
                    switch (level)
                    {
                        case 6:
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("[DEBUG-LOG]" + Type() + text);
                            Console.ResetColor();
                            WriteOnLog(text, "[DEBUG-LOG]" + Type());
                            break;
                        case 7:
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            Console.WriteLine("[DEBUG-FATAL]" + Type() + text);
                            Console.ResetColor();
                            WriteOnLog(text, "[DEBUG-FATAL]" + Type());
                            break;

                    }
                }
            }

        }

        public void WriteMessageConsole(string text, int level, string sender = "")
        {
            if (ConfUCS.IsConsoleMode == false)
            {
                switch (level)
                {
                    case 4:
                        SetupRTB(Brushes.White, text,  "<" + sender + "> " + TypeMSG() + " "); 
                        WriteOnLog(text, "<" + sender + "> " + TypeMSG() + " "); break;
                    case 5:
                        SetupRTB(Brushes.Violet, text, "[SERVER] " + TypeMSG() + " "); 
                        WriteOnLog(text, "[SERVER] " + TypeMSG() + " "); break;
                }
            }
            else
            {
                switch (level)
                {
                    case 4:
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("<" + sender + "> " + TypeMSG()+ " " + text);
                        Console.ResetColor();
                        WriteOnLog(text, "<" + sender + "> " + TypeMSG() + " ");
                        break;
                    case 5:
                        Console.ForegroundColor = ConsoleColor.DarkMagenta;
                        Console.WriteLine("[SERVER] " + TypeMSG() + " " + text);
                        Console.ResetColor();
                        WriteOnLog(text, "[SERVER] " + TypeMSG() + " ");
                        break;
                }

            }
        }

        string Type()
        {
            string x = "<" + DateTime.Now.ToString("HH:mm:ss") + ">" + ": ";
            return x;
        }
        string TypeMSG()
        {
            string x =DateTime.Now.ToString("HH:mm:ss");
            return x + ">>";
        }

        #endregion

        #region Console Helper Commands & Console

        public void CommandRead(string cmd)
        {
            if (!ConfUCS.IsConsoleMode) SetupRTB(Brushes.White, cmd, "> ");
            if (cmd.ToLower() == "/help")
            {
                WriteMessageConsole("/start                             <-- Start the server", (int)level.SERVERMSG);
                WriteMessageConsole("/ban <PlayerID>                    <-- Ban a client", (int)level.SERVERMSG);
               // WriteMessageConsole("/banip <PlayerID>                  <-- Ban a client by IP", (int)level.SERVERMSG);
                WriteMessageConsole("/unban <PlayerID>                  <-- Unban a client", (int)level.SERVERMSG);
               // WriteMessageConsole("/unbanip <PlayerID>                <-- Unban a client", (int)level.SERVERMSG);
               // WriteMessageConsole("/tempban <PlayerID> <Seconds>      <-- Temporary ban a client", (int)level.SERVERMSG);
               // WriteMessageConsole("/tempbanip <PlayerID> <Seconds>    <-- Temporary ban a client by IP", (int)level.SERVERMSG);
                WriteMessageConsole("/kick <PlayerID>                   <-- Kick a client from the server", (int)level.SERVERMSG);
                WriteMessageConsole("/mute <PlayerID>                   <-- Mute a client", (int)level.SERVERMSG);
               // WriteMessageConsole("/unmute <PlayerID>                 <-- Unmute a client", (int)level.SERVERMSG);
                WriteMessageConsole("/setlevel <PlayerID> <Level>       <-- Set a level for a player", (int)level.SERVERMSG);
                WriteMessageConsole("/update                            <-- Check if update is available", (int)level.SERVERMSG);
               // WriteMessageConsole("/say <Text>                        <-- Send a text to all", (int)level.SERVERMSG);
               // WriteMessageConsole("/sayplayer <PlayerID> <Text>       <-- Send a text to a player", (int)level.SERVERMSG);
                WriteMessageConsole("/stop  or   /shutdown              <-- Stop the server and save data", (int)level.SERVERMSG);
                WriteMessageConsole("/forcestop                         <-- Force stop the server", (int)level.SERVERMSG);
                WriteMessageConsole("/restart                           <-- Save data and then restart", (int)level.SERVERMSG);
                WriteMessageConsole("/sysinfo                           <-- Send server info to all players", (int)level.SERVERMSG);
                WriteMessageConsole("/status                            <-- Get server status", (int)level.SERVERMSG);
                WriteMessageConsole("/startx                            <-- Start legacy UCS manager", (int)level.SERVERMSG);

            }
            else if (cmd.ToLower() == "/start")
            {
                if (!IsServerOnline) LaunchServer();
                else WriteConsole("Server already online!", (int)level.WARNING);
            }
            else if (cmd.ToLower() == "/stop" || cmd.ToLower() == "/shutdown")
            {
                WriteConsole("Shutting down... Saving all data, wait.", (int)level.WARNING);

                foreach (var onlinePlayer in ResourcesManager.GetOnlinePlayers())
                {
                    var p = new ShutdownStartedMessage(onlinePlayer.GetClient());
                    p.SetCode(5);
                    PacketManager.ProcessOutgoingPacket(p);
                }

                ConsoleManage.FreeConsole();
                Environment.Exit(0);
            }
            else if (cmd.ToLower() == "/forcestop")
            {
                WriteConsole("Force shutting down... All progress not saved will be lost!", (int)level.WARNING);
                Process.GetCurrentProcess().Kill();
            }
            else if (cmd.ToLower() == "/uptime")
            {
                WriteConsole("Up time: " + ElapsedTime, (int)level.LOG);
            }
            else if (cmd.ToLower() == "/restart")
            {

                WriteConsole("System Restarting....", (int)level.WARNING);

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
                WriteConsole("Saving all data...", (int)level.WARNING);
                foreach (var l in ResourcesManager.GetOnlinePlayers())
                {
                    DatabaseManager.Singelton.Save(l);
                }

                WriteConsole("Restarting now", (int)level.WARNING);

                Process.Start(Application.ResourceAssembly.Location);
                Process.GetCurrentProcess().Kill();
            }
            else if (cmd.ToLower() == "/clear")
            {
                WriteMessageConsole("Console cleared", (int)level.SERVERMSG);
                if (ConfUCS.IsConsoleMode) Console.Clear();
                else
                {
                    TextRange txt = new TextRange(RTB_Console.Document.ContentStart, RTB_Console.Document.ContentEnd);
                    txt.Text = "";
                }
            }
            else if (cmd.ToLower() == "/reloadfilter")
            {
                WriteMessageConsole("Filter has been reloaded", (int)level.SERVERMSG);
                Message.ReloadChatFilterList();
            }

            else if (cmd.ToLower() == "/status")
            {
                var IPM = GetIP();
                WriteMessageConsole("Server IP: " + IPM + " on port 9339", (int)level.SERVERMSG);
                WriteMessageConsole("Online Player: " + ResourcesManager.GetOnlinePlayers().Count, (int)level.SERVERMSG);
                WriteMessageConsole("Connected Player: " + ResourcesManager.GetConnectedClients().Count, (int)level.SERVERMSG);
                WriteMessageConsole("Starting Gold: " + int.Parse(ConfigurationManager.AppSettings["StartingGold"]), (int)level.SERVERMSG);
                WriteMessageConsole("Starting Elixir: " +
                                  int.Parse(ConfigurationManager.AppSettings["StartingElixir"]), (int)level.SERVERMSG);
                WriteMessageConsole("Starting Dark Elixir: " +
                                  int.Parse(ConfigurationManager.AppSettings["StartingDarkElixir"]), (int)level.SERVERMSG);
                WriteMessageConsole("Starting Gems: " + int.Parse(ConfigurationManager.AppSettings["StartingGems"]), (int)level.SERVERMSG);
                WriteMessageConsole("CoC Version: " + ConfigurationManager.AppSettings["ClientVersion"], (int)level.SERVERMSG);
                if (Convert.ToBoolean(ConfigurationManager.AppSettings["useCustomPatch"]))
                {
                    WriteMessageConsole("Patch: Active", (int)level.SERVERMSG);
                    WriteMessageConsole("Patching Server: " + ConfigurationManager.AppSettings["patchingServer"], (int)level.SERVERMSG);
                }
                else
                {
                    WriteMessageConsole("Patch: Disable", (int)level.SERVERMSG);
                }
                if (Convert.ToBoolean(ConfigurationManager.AppSettings["maintenanceMode"]))
                {
                    WriteMessageConsole("Maintance Mode: Active", (int)level.SERVERMSG);
                    WriteMessageConsole("Maintance time: " +
                                      Convert.ToInt32(ConfigurationManager.AppSettings["maintenanceTimeleft"]) +
                                      " Seconds", (int)level.SERVERMSG);
                }
                else
                {
                    WriteMessageConsole("Maintance Mode: Disable", (int)level.SERVERMSG);
                }
            }
            else if (cmd.ToLower() == "/sysinfo")
            {
                WriteMessageConsole("Server Status is now sent to all online players", (int)level.SERVERMSG);

                var mail = new AllianceMailStreamEntry();
                mail.SetId((int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
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
            }

            else if (cmd.ToLower() == "/update")
            {
                WriteConsole("No update found", (int)level.WARNING); //Until Aidid's server is ready
            }

            else if (cmd.ToLower().StartsWith("/kick"))
            {
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
                            WriteConsoleDebug("Kick failed: id " + id + " not found", (int)level.DEBUGLOG);
                        }
                    }
                    catch (FormatException)
                    {
                        WriteConsoleDebug("The given id is not a valid number", (int)level.DEBUGFATAL);
                    }
                    catch (Exception ex)
                    {
                        WriteConsoleDebug("Kick failed with error: " + ex, (int)level.DEBUGFATAL);
                    }
                }
                else WriteConsoleDebug("Not enough arguments", (int)level.DEBUGFATAL);
            }

            else if (cmd.ToLower().StartsWith("/ban"))
            {
                var CommGet = cmd.Split(' ');
                if (CommGet.Length >= 2)
                {
                    try
                    {
                         var id = Convert.ToInt64(CommGet[1]);
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
                              WriteConsoleDebug("Ban failed: id " + id + " not found", (int)level.DEBUGLOG);
                         }
                    }
                    catch (FormatException)
                    {
                         WriteConsoleDebug("The given id is not a valid number", (int)level.DEBUGFATAL);
                    }
                    catch (Exception ex)
                    {
                        WriteConsoleDebug("Ban failed with error: " + ex, (int)level.DEBUGFATAL);
                    }
                }
                else WriteConsoleDebug("Not enough arguments", (int)level.DEBUGFATAL);
            }

            else if (cmd.ToLower().StartsWith("/unban"))
            {
                var CommGet = cmd.Split(' ');
                if (CommGet.Length >= 2)
                {
                    try
                    {
                        var id = Convert.ToInt64(CommGet[1]);
                        var l = ResourcesManager.GetPlayer(id);
                        if (l != null)
                        {
                            l.SetAccountStatus(0);
                        }
                        else
                        {
                            WriteConsoleDebug("Unban failed: id " + id + " not found", (int)level.DEBUGLOG);
                        }
                    }
                    catch (FormatException)
                    {
                        WriteConsoleDebug("The given id is not a valid number", (int)level.DEBUGFATAL);
                    }
                    catch (Exception ex)
                    {
                        WriteConsoleDebug("Unban failed with error: " + ex, (int)level.DEBUGFATAL);
                    }
                }
                else WriteConsoleDebug("Not enough arguments", (int)level.DEBUGFATAL);

            }

            else if (cmd.ToLower().StartsWith("/mute"))
            {
                var CommGet = cmd.Split(' ');
                if (CommGet.Length >= 2)
                {
                    try
                    {
                        var id = Convert.ToInt64(CommGet[1]);
                        var l = ResourcesManager.GetPlayer(id);
                        if (ResourcesManager.IsPlayerOnline(l))
                        {
                            var p = new BanChatTrigger(l.GetClient());
                            p.SetCode(999999999);
                            PacketManager.ProcessOutgoingPacket(p);
                        }
                        else
                        {
                            WriteConsoleDebug("Chat Mute failed: id " + id + " not found", (int)level.DEBUGLOG);
                        }
                    }
                    catch (FormatException)
                    {
                        WriteConsoleDebug("The given id is not a valid number", (int)level.DEBUGFATAL);
                    }
                    catch (Exception ex)
                    {
                        WriteConsoleDebug("Chat Mute failed with error: " + ex, (int)level.DEBUGFATAL);
                    }
                }
                else WriteConsoleDebug("Not enough arguments", (int)level.DEBUGFATAL);
            }

            else
            {
                WriteConsole("Command not found, try typing /help", (int)level.WARNING);
            }

            //Verify and execute

            CommandLine.Clear();
            //Clear

            if (ConfUCS.IsConsoleMode) ManageConsole();

        }

        

        private void ManageConsole()
        {

            CommandRead(Console.ReadLine());

        }


        #endregion
    }

    public class ConCatPlayers
    {
        public string PlayerIDs { get; set; }
        public string PlayerNames { get; set; }

        public override string ToString()
        {
            return string.Format("{0} : {1}", PlayerNames, PlayerIDs);
        }
    }

    static class AsyncUtils
    {
        static public void DelayCall(int msec, Action fn)
        {
            Dispatcher d = Dispatcher.CurrentDispatcher;
            new Task(() => {
                Thread.Sleep(msec);
                d.BeginInvoke(fn);
            }).Start();
        }
    }
}
