using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.Xml;

namespace Ultrapowa_Clash_Server_GUI
{
    /// <summary>
    /// Logica di interazione per SplashScreen.xaml
    /// </summary>
    public partial class SplashScreen : Window
    {
        List<string> GameListFiles;
        List<string> RequiredFiles;
        List<string> MissingNotReqFiles = new List<string>();
        String MonoLine;

        public SplashScreen()
        {
            InitializeComponent();
            if (Sys.ConfUCS.IsConsoleMode == false)
            {
                label_version.Content = "UCS " + Sys.ConfUCS.VersionUCS;
                Opacity = 0;
                OpInW();
            }
            else
            {
                Console.WriteLine(
@"
888     888  .d8888b.   .d8888b.  
888     888 d88P  Y88b d88P  Y88b 
888     888 888    888 Y88b.      
888     888 888         ""Y888b.
888     888 888            ""Y88b. 
888     888 888    888       ""888 
Y88b. .d88P Y88b  d88P Y88b  d88P 
 ""Y88888P""   ""Y8888P""   ""Y8888P""  
        ");
                Console.Write("\n\n\n\n");
                Console.WindowWidth = 100;
                Console.WindowHeight = 25;
                Console.BufferWidth = 100;
            }

        }

        private readonly BackgroundWorker worker = new BackgroundWorker();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeFileList();
            Thread newThread = new Thread(CheckUpdate);
            newThread.Start();
            if (Sys.ConfUCS.IsConsoleMode)
            {
                Hide();
            }

        }



        private void Window_Closing(object sender, CancelEventArgs e)
        {
            OpOutW(sender, e);
        }

        private void OpInW()
        {
            var OpIn = new DoubleAnimation(1, TimeSpan.FromSeconds(0.5));
            this.BeginAnimation(UIElement.OpacityProperty, OpIn);
        }

        private void OpOutW(object sender, CancelEventArgs e)
        {
            Closing -= Window_Closing;
            e.Cancel = true;
            var OpOut = new DoubleAnimation(0, TimeSpan.FromSeconds(0.5));
            OpOut.Completed += (s, _) => { this.Close(); MainWindow.RemoteWindow.Show(); MainWindow.IsFocusOk = true; };
            this.BeginAnimation(OpacityProperty, OpOut);
        }

        public double localvalue = 0;
        public double Inc = 0;

        private void CheckUpdate()
        {

            double minicounter = 0;

            this.Dispatcher.BeginInvoke((Action)delegate ()
            {
                label_txt.Content = "Verifying game files... ";
            });

            foreach (string DataG in GameListFiles)
            {

                if (!System.IO.File.Exists(System.IO.Directory.GetCurrentDirectory() + @"\" + DataG))
                {
                    MissingNotReqFiles.Add(DataG);
                }

                minicounter++;
                localvalue = (minicounter / GameListFiles.Count);

                this.Dispatcher.BeginInvoke((Action)delegate ()
                {
                    PB_Loader.Value = Inc + (50 * localvalue);
                });

                if (Sys.ConfUCS.IsConsoleMode)
                {
                    DrawProgressBar((int)(Inc + (50 * localvalue)), 100, 40, "█", "Verifying game files...       ");
                }

                //Thread.Sleep(50); //For testing only

            }

            minicounter = 0;
            Inc = 50;

            this.Dispatcher.BeginInvoke((Action)delegate ()
            {
                label_txt.Content = "Verifying required data... ";
            });

            if (MissingNotReqFiles.Count != 0)
            {

                foreach (string FilesMiss in MissingNotReqFiles)
                {
                    MonoLine += FilesMiss + " || ";
                }
                string IsMoreThanOne = MissingNotReqFiles.Count == 1 ? "There is a missing gamefile: " : "There are a missing gamefiles from the directory " + @"""" + System.IO.Directory.GetCurrentDirectory() + @"\ ";
                MessageBox.Show(IsMoreThanOne + MonoLine, "Missing Gamefile", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }

            foreach (string DataG in RequiredFiles)
            {
                if (!System.IO.File.Exists(System.IO.Directory.GetCurrentDirectory() + @"\" + DataG))
                {

                    MessageBox.Show(string.Format("The required file {0} in directory {1} is missing. Cannot continue.",
                        DataG, System.IO.Directory.GetCurrentDirectory()), "Error required file", MessageBoxButton.OK, MessageBoxImage.Error);

                    ThreadStart ts = delegate ()
                    {
                        Dispatcher.BeginInvoke((Action)delegate ()
                        {
                            Application.Current.Shutdown();
                        });
                    };
                    Thread t = new Thread(ts);
                    t.Start();
                    worker.CancelAsync();

                }
                minicounter++;
                localvalue = (minicounter / RequiredFiles.Count);

                this.Dispatcher.BeginInvoke((Action)delegate ()
                {
                    PB_Loader.Value = Inc + (30 * localvalue);
                });

                if (Sys.ConfUCS.IsConsoleMode)
                {
                    DrawProgressBar((int)(Inc + (30 * localvalue)), 100, 40, "█", "Verifying required data...    ");
                }

                //Thread.Sleep(50); //for testing only

            }
            this.Dispatcher.BeginInvoke((Action)delegate ()
            {
                label_txt.Content = "Checking update... ";
            });

            if (Sys.ConfUCS.IsConsoleMode)
            {
                DrawProgressBar((int)(Inc + (50 * localvalue)), 100, 40, "█", "Checking update...            ");
            }

            this.Dispatcher.BeginInvoke((Action)delegate ()
            {
                PB_Loader.Value = 80;
            });

            if (Sys.ConfUCS.IsConsoleMode)
            {
                DrawProgressBar(80, 100, 40, "█", "Checking update...            ");
            }

            var XMLUrl = "http://www.flamewall.net/ucs/system.xml";
            var NamesEL = "";
            XmlTextReader ReadTheXML = null;

            this.Dispatcher.BeginInvoke((Action)delegate ()
            {
                label_txt.Content = "Checking Update";
            });

            if (Sys.ConfUCS.IsConsoleMode)
            {
                DrawProgressBar(80, 100, 40, "█", "Checking update               ");
            }

            try
            {
                ReadTheXML = new XmlTextReader(XMLUrl);
                ReadTheXML.MoveToContent();
                if ((ReadTheXML.NodeType == XmlNodeType.Element) && (ReadTheXML.Name == "appinfo"))
                    while (ReadTheXML.Read())
                        if (ReadTheXML.NodeType == XmlNodeType.Element) NamesEL = ReadTheXML.Name;
                        else
                        {
                            if ((ReadTheXML.NodeType == XmlNodeType.Text) && (ReadTheXML.HasValue))
                            {
                                switch (NamesEL)
                                {
                                    case "version": Sys.ConfUCS.NewVer = new Version(ReadTheXML.Value); break;
                                    case "url": Sys.ConfUCS.UrlPage = ReadTheXML.Value; break;
                                    case "about": Sys.ConfUCS.Changelog = ReadTheXML.Value; break;

                                }
                            }
                        }

            }
            catch (Exception ex)
            {

                Dispatcher.BeginInvoke((Action)delegate ()
                {
                    label_txt.Content = "Can't check update. Error: " + ex.Message;
                });

                if (Sys.ConfUCS.IsConsoleMode)
                {
                    DrawProgressBar(80, 100, 40, "█", "Can't check update. Error     ");
                }

                Thread.Sleep(500);
            }
            finally
            {
                if (ReadTheXML != null) ReadTheXML.Close();
            }

            Version thisAppVer = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

            if (thisAppVer.CompareTo(Sys.ConfUCS.NewVer) < 0)
            {

                Dispatcher.BeginInvoke((Action)delegate ()
                {
                    PB_Loader.Value = 90;
                    label_txt.Content = "New update is available.";
                });

                if (Sys.ConfUCS.IsConsoleMode)
                {
                    DrawProgressBar(90, 100, 40, "█", "New update is available.      ");
                }

                Sys.ConfUCS.IsUpdateAvailable = true;

            }

            else
            {

                Dispatcher.BeginInvoke((Action)delegate ()
                {
                    label_txt.Content = "No update found";
                });

                if (Sys.ConfUCS.IsConsoleMode)
                {
                    DrawProgressBar(90, 100, 40, "█", "No update found               ");
                }

            }

            Dispatcher.BeginInvoke((Action)delegate ()
            {
                PB_Loader.Value = 100;
            });

            if (Sys.ConfUCS.IsConsoleMode)
            {
                DrawProgressBar(100, 100, 40, "█", "No update found               ");
            }

            Thread.Sleep(100);

            Dispatcher.BeginInvoke((Action)delegate ()
            {
                Close();
            });


        }

        static int RNDNum = 0;

        private static void DrawProgressBar(int complete, int maxVal, int barSize, string TypeText, string messg)
        {
            Console.CursorVisible = false;
            int left = Console.CursorLeft;
            decimal perc = (decimal)complete / (decimal)maxVal;
            int chars = (int)Math.Floor(perc / ((decimal)1 / (decimal)barSize));
            string p1 = string.Empty, p2 = string.Empty;
            string ch = "";
            for (int i = 0; i < chars; i++) p1 += TypeText;
            for (int i = 0; i < barSize - chars; i++) p2 += TypeText;

            Console.Write(messg + " ");

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(p1);
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(p2);

            Console.Title = "UCS is loading: " + (perc * 100).ToString("N2") + "%";

            Console.ResetColor();
            switch (RNDNum)
            {
                case 0:
                    ch = "-";
                    break;
                case 5:
                    ch = @"\";
                    break;
                case 10:
                    ch = "|";
                    break;
                case 15:
                    ch = "/";
                    RNDNum = -6;
                    break;
            }
            Console.Write(" [{0}", (perc * 100).ToString("N2") + "%] " + ch);
            Console.CursorLeft = left;
            RNDNum++;
        }

        private void InitializeFileList()
        {
            GameListFiles = new List<string>
            {
                @"gamefiles\fingerprint.json",
                @"gamefiles\starting_home.json",

                @"gamefiles\csv\animations.csv",
                @"gamefiles\csv\billing_packages.csv",
                @"gamefiles\csv\client_globals.csv",
                @"gamefiles\csv\credits.csv",
                @"gamefiles\csv\faq.csv",
                @"gamefiles\csv\hints.csv",
                @"gamefiles\csv\news.csv",
                @"gamefiles\csv\particle_emitters.csv",
                @"gamefiles\csv\resource_packs.csv",
                @"gamefiles\csv\texts.csv",
                @"gamefiles\csv\texts_patch.csv",

                @"gamefiles\logic\achievements.csv",
                @"gamefiles\logic\alliance_badge_layers.csv",
                @"gamefiles\logic\alliance_badges.csv",
                @"gamefiles\logic\alliance_levels.csv",
                @"gamefiles\logic\alliance_portal.csv",
                @"gamefiles\logic\building_classes.csv",
                @"gamefiles\logic\buildings.csv",
                @"gamefiles\logic\characters.csv",
                @"gamefiles\logic\decos.csv",
                @"gamefiles\logic\effects.csv",
                @"gamefiles\logic\experience_levels.csv",
                @"gamefiles\logic\globals.csv",
                @"gamefiles\logic\heroes.csv",
                @"gamefiles\logic\leagues.csv",
                @"gamefiles\logic\locales.csv",
                @"gamefiles\logic\missions.csv",
                @"gamefiles\logic\npcs.csv",
                @"gamefiles\logic\obstacles.csv",
                @"gamefiles\logic\projectiles.csv",
                @"gamefiles\logic\regions.csv",
                @"gamefiles\logic\resources.csv",
                @"gamefiles\logic\shields.csv",
                @"gamefiles\logic\spells.csv",
                @"gamefiles\logic\townhall_levels.csv",
                @"gamefiles\logic\traps.csv",
                @"gamefiles\logic\war.csv",

                @"gamefiles\pve\expertPve\level1.json",
                @"gamefiles\pve\expertPve\level2.json",
                @"gamefiles\pve\expertPve\level3.json",
                @"gamefiles\pve\expertPve\level4.json",
                @"gamefiles\pve\expertPve\level5.json",
                @"gamefiles\pve\expertPve\level6.json",
                @"gamefiles\pve\expertPve\level7.json",
                @"gamefiles\pve\expertPve\level8.json",
                @"gamefiles\pve\expertPve\level9.json",
                @"gamefiles\pve\expertPve\level10.json",
                @"gamefiles\pve\expertPve\level11.json",
                @"gamefiles\pve\expertPve\level12.json",
                @"gamefiles\pve\expertPve\level13.json",
                @"gamefiles\pve\expertPve\level14.json",
                @"gamefiles\pve\expertPve\level15.json",
                @"gamefiles\pve\expertPve\level16.json",
                @"gamefiles\pve\expertPve\level17.json",
                @"gamefiles\pve\expertPve\level18.json",
                @"gamefiles\pve\expertPve\level19.json",
                @"gamefiles\pve\expertPve\level20.json",
                @"gamefiles\pve\expertPve\level21.json",
                @"gamefiles\pve\expertPve\level22.json",
                @"gamefiles\pve\expertPve\level23.json",
                @"gamefiles\pve\expertPve\level24.json",
                @"gamefiles\pve\expertPve\level25.json",
                @"gamefiles\pve\expertPve\level26.json",
                @"gamefiles\pve\expertPve\level27.json",
                @"gamefiles\pve\expertPve\level28.json",
                @"gamefiles\pve\expertPve\level29.json",
                @"gamefiles\pve\expertPve\level30.json",
                @"gamefiles\pve\expertPve\level31.json",
                @"gamefiles\pve\expertPve\level32.json",
                @"gamefiles\pve\expertPve\level33.json",
                @"gamefiles\pve\expertPve\level34.json",
                @"gamefiles\pve\expertPve\level35.json",
                @"gamefiles\pve\expertPve\level36.json",
                @"gamefiles\pve\expertPve\level37.json",
                @"gamefiles\pve\expertPve\level38.json",
                @"gamefiles\pve\expertPve\level39.json",
                @"gamefiles\pve\expertPve\level40.json",
                @"gamefiles\pve\expertPve\level41.json",
                @"gamefiles\pve\expertPve\level42.json",
                @"gamefiles\pve\expertPve\level43.json",
                @"gamefiles\pve\expertPve\level44.json",
                @"gamefiles\pve\expertPve\level45.json",
                @"gamefiles\pve\expertPve\level46.json",
                @"gamefiles\pve\expertPve\level47.json",
                @"gamefiles\pve\expertPve\level48.json",
                @"gamefiles\pve\expertPve\level49.json",
                @"gamefiles\pve\expertPve\level50.json",

                @"gamefiles\pve\normalPve\level1.json",
                @"gamefiles\pve\normalPve\level2.json",
                @"gamefiles\pve\normalPve\level3.json",
                @"gamefiles\pve\normalPve\level4.json",
                @"gamefiles\pve\normalPve\level5.json",
                @"gamefiles\pve\normalPve\level6.json",
                @"gamefiles\pve\normalPve\level7.json",
                @"gamefiles\pve\normalPve\level8.json",
                @"gamefiles\pve\normalPve\level9.json",
                @"gamefiles\pve\normalPve\level10.json",
                @"gamefiles\pve\normalPve\level11.json",
                @"gamefiles\pve\normalPve\level12.json",
                @"gamefiles\pve\normalPve\level13.json",
                @"gamefiles\pve\normalPve\level14.json",
                @"gamefiles\pve\normalPve\level15.json",
                @"gamefiles\pve\normalPve\level16.json",
                @"gamefiles\pve\normalPve\level17.json",
                @"gamefiles\pve\normalPve\level18.json",
                @"gamefiles\pve\normalPve\level19.json",
                @"gamefiles\pve\normalPve\level20.json",
                @"gamefiles\pve\normalPve\level21.json",
                @"gamefiles\pve\normalPve\level22.json",
                @"gamefiles\pve\normalPve\level23.json",
                @"gamefiles\pve\normalPve\level24.json",
                @"gamefiles\pve\normalPve\level25.json",
                @"gamefiles\pve\normalPve\level26.json",
                @"gamefiles\pve\normalPve\level27.json",
                @"gamefiles\pve\normalPve\level28.json",
                @"gamefiles\pve\normalPve\level29.json",
                @"gamefiles\pve\normalPve\level30.json",
                @"gamefiles\pve\normalPve\level31.json",
                @"gamefiles\pve\normalPve\level32.json",
                @"gamefiles\pve\normalPve\level33.json",
                @"gamefiles\pve\normalPve\level34.json",
                @"gamefiles\pve\normalPve\level35.json",
                @"gamefiles\pve\normalPve\level36.json",
                @"gamefiles\pve\normalPve\level37.json",
                @"gamefiles\pve\normalPve\level38.json",
                @"gamefiles\pve\normalPve\level39.json",
                @"gamefiles\pve\normalPve\level40.json",
                @"gamefiles\pve\normalPve\level41.json",
                @"gamefiles\pve\normalPve\level42.json",
                @"gamefiles\pve\normalPve\level43.json",
                @"gamefiles\pve\normalPve\level44.json",
                @"gamefiles\pve\normalPve\level45.json",
                @"gamefiles\pve\normalPve\level46.json",
                @"gamefiles\pve\normalPve\level47.json",
                @"gamefiles\pve\normalPve\level48.json",
                @"gamefiles\pve\normalPve\level49.json",
                @"gamefiles\pve\normalPve\level50.json",
            };

            RequiredFiles = new List<string>
            {
                @"EntityFramework.dll",
                @"EntityFramework.SqlServer.dll",
                @"EntityFramework.SqlServer.dll",
                @"Ionic.Zlib.dll",
                @"MySql.Data.dll",
                @"MySql.Data.Entity.EF6.dll",
                @"Newtonsoft.Json.dll",
                @"Newtonsoft.Json.xml",
                @"System.Data.SQLite.dll",
                @"System.Data.SQLite.EF6.dll",
                @"System.Data.SQLite.Linq.dll",
                @"System.Data.SQLite.xml",
                @"ucsconf.config",
                @"ucsdb"
            };

        }

    }
}

