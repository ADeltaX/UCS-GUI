using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Configuration;
using System.Net;
using Ultrapowa_Clash_Server_GUI.Network;
using Ultrapowa_Clash_Server_GUI.PacketProcessing;
using Ultrapowa_Clash_Server_GUI.Core;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Runtime.InteropServices;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using Ultrapowa_Clash_Server_GUI.Helpers;
using System.IO;

namespace Ultrapowa_Clash_Server_GUI
{

    public partial class MainWindow : Window
    {
        public static MainWindow RemoteWindow = new MainWindow();
        public static bool IsFocusOk = true;
        bool ChangeUpdatePopup = false;
        static string LogPath = "NONE";
        StreamWriter LogStream = null;


        List<string> CommandList;

        bool IsServerOnline = false;

        public MainWindow()
        {
            InitializeComponent();
            RemoteWindow = this;

            if (Sys.ConfUCS.IsLogEnabled) PrepareLog();
            

            CommandList = new List<string>
            {
                "/say", "/ban", "/banip", "/tempban", "/tempbanip", "/unban",
                "/unbanip", "/mute","/unmute","/makeadmin", "/removeadmin",
                "/kick", "/help", "/start", "/restart", "/stop"
            };
            Version thisAppVer = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            GetArgs();

            if (Sys.ConfUCS.IsConsoleMode==true)
            {
                Console.Clear();
                WindowState = WindowState.Minimized;
                Console.Title = "UCS Server " + thisAppVer.Major + "." + thisAppVer.Minor + "." + thisAppVer.Build + "." + thisAppVer.MinorRevision + " OFFLINE";
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
                Title = "UCS Server " + thisAppVer.Major + "." + thisAppVer.Minor + "." + thisAppVer.Build + "." + thisAppVer.MinorRevision + " OFFLINE";
                WriteConsole("Loading GUI...", (int)level.LOG);
                CheckThings();
                LBL_IP.Content = "Local IP: " + GetIP();
                LBL_PORT.Content = "PORT: " + port;
                CommandLine.TextChanged += new TextChangedEventHandler(CommandLine_TextChanged);
            }
            

        }


        #region Events

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
            if (Sys.ConfUCS.IsConsoleMode)
            {
                Hide();
                ManageConsole();
            }
            else
            {
                WriteConsole("GUI loaded", (int)level.LOG);
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
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

        private void CommandLine_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
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

        private void MI_Ban_Click(object sender, RoutedEventArgs e)
        {
            IsFocusOk = false;
            Popup Popup = new Popup((int)Popup.cause.BAN);
            Popup.Owner = this;
            Popup.ShowDialog();

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

        private void CB_Debug_Unchecked(object sender, RoutedEventArgs e)
        {
            Sys.ConfUCS.DebugMode = false;
        }

        private void CB_Debug_Checked(object sender, RoutedEventArgs e)
        {
            Sys.ConfUCS.DebugMode = true;
        }

        #endregion

        #region Do stuff

        private void PrepareLog()
        {
            if (!Directory.Exists(Sys.ConfUCS.LogDirectory)) Directory.CreateDirectory(Sys.ConfUCS.LogDirectory);

            string DTT = DateTime.Now.ToString("HH:mm:ss");
            string DTD = DateTime.Now.ToString("dd/MM/yyyy");

            LogPath = Sys.ConfUCS.LogDirectory + string.Format("LOG_{0}_{1}.txt", DTD.Replace("/", "-"), DTT.Replace(":", "-"));

            try
            {
                LogStream = File.CreateText(LogPath);
            }
            catch (Exception)
            {
                WriteConsole("Cannot create log. Disabling log mode.", (int)level.FATAL);
                Sys.ConfUCS.IsLogEnabled = false;
            }
        }

        private string GetIP()
        {
            string HostName = Dns.GetHostName();
            return Dns.GetHostByName(HostName).AddressList[0].ToString();
        }

        private void CheckThings()
        {
            if (Sys.ConfUCS.IsUpdateAvailable== true)
            {
                MI_CheckUpdate.Header = "_UPDATE AVAILABLE";
                MI_CheckUpdate.Foreground = Brushes.Yellow;
                ChangeUpdatePopup = true;
            }
        }

        private void GetArgs()
        {
            Dictionary<string, string> CMline = new Dictionary<string, string>();
            string[] args = Environment.GetCommandLineArgs();
            for (int index = 1; index < args.Length; index += 2)
            {
                string arg = args[index].Replace("/", "");
                CMline.Add(arg, args[index]);
            }
            if (CMline.ContainsKey("default"))
            {
                WriteConsole("Line arg typed: /default", (int)level.LOG);
                WriteConsole("Loading default configuration", (int)level.LOG);
                LoadDefaultConfig();
            }
            if (CMline.ContainsKey("console"))
            {
                Sys.ConfUCS.IsConsoleMode = true;
            }
        }

        public static readonly int port = Utils.parseConfigInt("serverPort");
        //public static readonly int port = 9339;



        private void LaunchServer()
        {
            WriteConsole("Starting server...", (int)level.WARNING);

            if (Sys.ConfUCS.IsConsoleMode) Console.CursorVisible = false;

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
            WriteConsole("Server started on port " + port + ". Let's play Clash of Clans!", (int)level.LOG);
            IsServerOnline = true;

            if (Sys.ConfUCS.IsConsoleMode)
            {
                Console.CursorVisible = true;
                ManageConsole();
            }

            if (Convert.ToBoolean(Utils.parseConfigString("consoleCommand")))
            {
                //new Core.Menu();            //PLACEHOLDER DEBUG 
            }
            else
            {
                //Application.Run(new UCSManager());
            }
        }

        private static void InitProgramThreads()
        {
            RemoteWindow.WriteConsoleDebug("\t", (int)level.DEBUGLOG);
            var programThreads = new List<Thread>();
            var pt = new ProgramThread();
            pt.Start();
            RemoteWindow.WriteConsoleDebug("\tServer Running On Thread 1", (int)level.DEBUGLOG);

        }

        private void LoadDefaultConfig()
        {
            //Reset the config file
        }

        #region Blur Settings

        Storyboard myStoryboard = new Storyboard();
        DoubleAnimation myDoubleAnimation = new DoubleAnimation();
        BlurEffect blurEffect = new BlurEffect();
        private void DoBlur()
        {
            this.RegisterName("blurEffect", blurEffect);
            blurEffect.Radius = 0;
            this.Effect = blurEffect;

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
            this.RegisterName("blurEffect", blurEffect);
            blurEffect.Radius = 0;
            this.Effect = blurEffect;

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
            Dispatcher.BeginInvoke((Action)delegate ()
            {     
                
                    Dispatcher.Invoke(() => {
                        TextRange Sec_Text = new TextRange(RTB_Console.Document.ContentEnd, RTB_Console.Document.ContentEnd);
                        Sec_Text.Text = pretext + text + "\u2028";
                        Sec_Text.ApplyPropertyValue(TextElement.ForegroundProperty, color);
                    },DispatcherPriority.Send);
                
                
                //if (IsDebugMode == true) { Sec_Text.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.DarkMagenta); Sec_Text.ApplyPropertyValue(Inline.TextDecorationsProperty, TextDecorations.Underline); }
                RTB_Console.ScrollToEnd();
            });
        }

        public void WriteOnLog(string text,string pretext)
        {
            if (Sys.ConfUCS.IsLogEnabled)
            {
                try
                {
                    LogStream.AutoFlush = true;
                    LogStream.WriteLine(pretext + text);
                }
                catch (Exception ex)
                {
                    Sys.ConfUCS.IsLogEnabled = false;
                    WriteConsole("Error during saving log to file. ::" + ex.Message, (int)level.FATAL);
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
            if (Sys.ConfUCS.IsConsoleMode == false)
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
            if (Sys.ConfUCS.DebugMode == true)
            {
                if (Sys.ConfUCS.IsConsoleMode == false)
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
            if (Sys.ConfUCS.IsConsoleMode == false)
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

        private void CommandRead(string cmd)
        {
            if (!Sys.ConfUCS.IsConsoleMode) SetupRTB(Brushes.White, cmd, ">");
            if (cmd == "/help")
            {

                WriteMessageConsole("/ban <PlayerID>                    <-- Ban a client", 5);
                WriteMessageConsole("/banip <PlayerID>                  <-- Ban a client by IP", 5);
                WriteMessageConsole("/kick <PlayerID>                   <-- Kick a client from the server", 5);
                WriteMessageConsole("/unban <PlayerID>                  <-- Unban a client", 5);
                WriteMessageConsole("/unbanip <PlayerID>                <-- Unban a client", 5);
                WriteMessageConsole("/tempban <PlayerID> <Seconds>      <-- Temporary ban a client", 5);
                WriteMessageConsole("/tempbanip <PlayerID> <Seconds>    <-- Temporary ban a client by IP", 5);
                WriteMessageConsole("/mute <PlayerID>                   <-- Mute a client", 5);
                WriteMessageConsole("/unmute <PlayerID>                 <-- Unmute a client", 5);

                WriteMessageConsole("/update                            <-- Check if update is available", 5);
                WriteMessageConsole("/tempbanip <PlayerID> <Seconds>    <-- Temporary ban a client by IP", 5);
                WriteMessageConsole("/say <Text>                        <-- Send a text to all", 5);
                
                WriteMessageConsole("...", 5);
                WriteMessageConsole("I'll build the list of command lol", 5);

            }
            else if (cmd == "/start")
            {
                if (!IsServerOnline) LaunchServer();
                else WriteConsole("Server already online!", 2);
            }
            else if (cmd == "/stop" || cmd == "/shutdown")
            {
                WriteConsole("Shutting down... Saving all data, wait.", 2);
                //EXECUTE
                Environment.Exit(0);
            }
            else if (cmd == "/forcestop")
            {
                WriteConsole("Force shutting down... All progress not saved will be lost!", 2);
                Application.Current.Shutdown();
                Environment.Exit(0);
            }
            else
            {
                WriteConsole("Command not found, try typing /help", 2);
            }

            //Verify and execute

            CommandLine.Clear();
            //Clear

            if (Sys.ConfUCS.IsConsoleMode) ManageConsole();

        }

        

        private void ManageConsole()
        {

            CommandRead(Console.ReadLine());

        }


        #endregion
    }
}
