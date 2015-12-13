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

namespace Ultrapowa_Clash_Server_GUI
{

    public partial class MainWindow : Window
    {
        public static MainWindow RemoteWindow = new MainWindow();
        public static bool IsFocusOk = true;
        bool ChangeUpdatePopup = false;

        List<string> CommandList;

        bool IsServerOnline = false;

       [DllImport("kernel32.dll")]
        static extern bool AttachConsole(int dwProcessId);
        private const int ATTACH_PARENT_PROCESS = -1;
        public MainWindow()
        {
            InitializeComponent();
            RemoteWindow = this;
            CommandList = new List<string>
            {
                "/say", "/ban", "/banip", "/tempban", "/tempbanip", "/unban",
                "/unbanip", "/mute","/unmute","/makeadmin", "/removeadmin",
                "/kick", "/help", "/start", "/restart", "/stop"
            };

            GetArgs();

            if (Sys.ConfUCS.IsConsoleMode==true)
            {
                this.Hide();
                AttachConsole(ATTACH_PARENT_PROCESS);
                Console.Clear();
                Console.Title = "UCS Server";
                WriteConsole("Line arg typed: /console", (int)level.LOG);
                WriteConsole("Running in Console mode...", (int)level.LOG);
                WriteConsole("Local IP: " + GetIP(), (int)level.LOG);
                WriteConsole("Ready", (int)level.LOG);
                WriteConsole("Write /start to start the server or write /help to get all commands", (int)level.LOG);
            }
            else
            {
                WriteConsole("Loading GUI...", (int)level.LOG);
                CheckThings();
                LBL_IP.Content = "Local IP: " + GetIP();
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
            WriteConsole("GUI loaded", (int)level.LOG);
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

        #endregion

        #region Do stuff

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

        private void LaunchServer()
        {
            WriteConsole("Starting server...", (int)level.WARNING);
            Gateway g = new Gateway();
            PacketManager ph = new PacketManager();
            MessageManager dp = new MessageManager();
            ResourcesManager rm = new ResourcesManager();
            ObjectManager pm = new ObjectManager();
            dp.Start();
            ph.Start();
            g.Start();
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
            TextRange Sec_Text = new TextRange(RTB_Console.Document.ContentEnd, RTB_Console.Document.ContentEnd);
            Sec_Text.Text = pretext + text + "\u2028";
            Sec_Text.ApplyPropertyValue(TextElement.ForegroundProperty, color);
            if (IsDebugMode == true) { Sec_Text.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.DarkMagenta); Sec_Text.ApplyPropertyValue(Inline.TextDecorationsProperty, TextDecorations.Underline); }
            RTB_Console.ScrollToEnd();
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
                        SetupRTB(new SolidColorBrush(Color.FromRgb(22, 160, 133)), text, "[LOG]" + Type()); break;
                    case 2:
                        SetupRTB(Brushes.Orange, text, "[WARNING]" + Type()); break;
                    case 3:
                        SetupRTB(Brushes.Red, text, "[FATAL]" + Type()); break;
                }
            }
            else
            {
                switch (level)
                {
                    case 1:
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("[LOG]" + Type() + text); 
                        Console.ResetColor(); break;
                    case 2:
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.WriteLine("[WARNING]"  + Type() + text);
                        Console.ResetColor(); break;
                    case 3:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("[FATAL]" + Type() + text);
                        Console.ResetColor(); break;
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
                            SetupRTB(Brushes.Yellow, text, "[DEBUG-LOG]" + Type(), true); break;
                        case 7:
                            SetupRTB(Brushes.LightYellow, text, "[DEBUG-FATAL]" + Type(), true); break;
                    }
                }
                else
                {
                    switch (level)
                    {
                        case 6:
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("[DEBUG-LOG]" + Type() + text);
                            Console.ResetColor(); break;
                        case 7:
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            Console.WriteLine("[DEBUG-WARNING]" + Type() + text);
                            Console.ResetColor(); break;
                    }
                }
            }

        }

        public void WriteMessageConsole(string text, int level, string sender)
        {
            if (Sys.ConfUCS.IsConsoleMode == false)
            {
                switch (level)
                {
                    case 4:
                        SetupRTB(Brushes.White, text, TypeMSG() + " <" + sender + "> "); break;
                    case 5:
                        SetupRTB(Brushes.Violet, text, TypeMSG() + " [SERVER] "); break;
                }
            }
            else
            {
                switch (level)
                {
                    case 4:
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine(" <" + sender + "> " + TypeMSG() + text);
                        Console.ResetColor(); break;
                    case 5:
                        Console.ForegroundColor = ConsoleColor.DarkMagenta;
                        Console.WriteLine(" [SERVER] " + TypeMSG() + text);
                        Console.ResetColor(); break;
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
            return x;
        }

        #endregion

        #region Console Helper Commands

        private void CommandRead(string cmd)
        {

            //Verify and execute

            CommandLine.Clear();
            //Clear
        }


        #endregion
    }
}
