using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
            this.Opacity = 0;
            InitializeComponent();
            OpInW();
        }

        private readonly BackgroundWorker worker = new BackgroundWorker();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeFileList();
            Thread newThread = new Thread(CheckUpdate);
            newThread.Start();

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


        private void InitializeFileList()
        {
            GameListFiles = new List<string>
            {
                @"gamefiles\fingerprint.json",

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

                @"gamefiles\default\home.json",

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
                @"gamefiles\logic\resources.csv",
                @"gamefiles\logic\shields.csv",
                @"gamefiles\logic\spells.csv",
                @"gamefiles\logic\townhall_levels.csv",
                @"gamefiles\logic\traps.csv",
                @"gamefiles\logic\war.csv",

                @"gamefiles\pve\level1.json",
                @"gamefiles\pve\level2.json",
                @"gamefiles\pve\level3.json",
                @"gamefiles\pve\level4.json",
                @"gamefiles\pve\level5.json",
                @"gamefiles\pve\level6.json",
                @"gamefiles\pve\level7.json",
                @"gamefiles\pve\level8.json",
                @"gamefiles\pve\level9.json",
                @"gamefiles\pve\level10.json",
                @"gamefiles\pve\level11.json",
                @"gamefiles\pve\level12.json",
                @"gamefiles\pve\level13.json",
                @"gamefiles\pve\level14.json",
                @"gamefiles\pve\level15.json",
                @"gamefiles\pve\level16.json",
                @"gamefiles\pve\level17.json",
                @"gamefiles\pve\level18.json",
                @"gamefiles\pve\level19.json",
                @"gamefiles\pve\level20.json",
                @"gamefiles\pve\level21.json",
                @"gamefiles\pve\level22.json",
                @"gamefiles\pve\level23.json",
                @"gamefiles\pve\level24.json",
                @"gamefiles\pve\level25.json",
                @"gamefiles\pve\level26.json",
                @"gamefiles\pve\level27.json",
                @"gamefiles\pve\level28.json",
                @"gamefiles\pve\level29.json",
                @"gamefiles\pve\level30.json",
                @"gamefiles\pve\level31.json",
                @"gamefiles\pve\level32.json",
                @"gamefiles\pve\level33.json",
                @"gamefiles\pve\level34.json",
                @"gamefiles\pve\level35.json",
                @"gamefiles\pve\level36.json",
                @"gamefiles\pve\level37.json",
                @"gamefiles\pve\level38.json",
                @"gamefiles\pve\level39.json",
                @"gamefiles\pve\level40.json",
                @"gamefiles\pve\level41.json",
                @"gamefiles\pve\level42.json",
                @"gamefiles\pve\level43.json",
                @"gamefiles\pve\level44.json",
                @"gamefiles\pve\level45.json",
                @"gamefiles\pve\level46.json",
                @"gamefiles\pve\level47.json",
                @"gamefiles\pve\level48.json",
                @"gamefiles\pve\level49.json",
                @"gamefiles\pve\level50.json",
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

        public double localvalue = 0;
        public double Inc = 0;

        private void CheckUpdate()
        {
            
            double minicounter = 0;

            this.Dispatcher.BeginInvoke((Action)delegate () {
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

                this.Dispatcher.BeginInvoke((Action)delegate () {
                    PB_Loader.Value = Inc + (50 * localvalue);
                });

                //Thread.Sleep(25); //For testing only

            }

            minicounter = 0;
            Inc = 50;

            this.Dispatcher.BeginInvoke((Action)delegate () {
                label_txt.Content = "Verifying required data... ";
            });

            if (MissingNotReqFiles.Count != 0)
            {

                foreach (string FilesMiss in MissingNotReqFiles)
                {
                    MonoLine += FilesMiss + Environment.NewLine;
                }
                string IsMoreThanOne = MissingNotReqFiles.Count == 1 ? "There is a missing gamefile: " : "There are a missing gamefiles: ";
                MessageBox.Show(IsMoreThanOne + Environment.NewLine + MonoLine, "Missing Gamefile", MessageBoxButton.OK, MessageBoxImage.Exclamation);
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

                this.Dispatcher.BeginInvoke((Action)delegate () {
                    PB_Loader.Value = Inc + (30 * localvalue);
                });

                //Thread.Sleep(100); for testing only

            }
            this.Dispatcher.BeginInvoke((Action)delegate () {
                label_txt.Content = "Checking update... ";
            });

            this.Dispatcher.BeginInvoke((Action)delegate () {
                PB_Loader.Value = 80;
            });

            var XMLUrl = "http://www.flamewall.net/ucs/system.xml";
            var NamesEL = "";
            XmlTextReader ReadTheXML = null;

            this.Dispatcher.BeginInvoke((Action)delegate () {
                label_txt.Content = "Checking Update";
            });

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

                Dispatcher.BeginInvoke((Action)delegate () {
                    label_txt.Content = "Can't check update. Error: " + ex.Message;
                });

                Thread.Sleep(500);
            }
            finally
            {
                if (ReadTheXML != null) ReadTheXML.Close();
            }

            Version thisAppVer = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

            if (thisAppVer.CompareTo(Sys.ConfUCS.NewVer) < 0)
            {

                Dispatcher.BeginInvoke((Action)delegate () {
                    PB_Loader.Value = 90;
                    label_txt.Content = "New update is available.";
                });

                Sys.ConfUCS.IsUpdateAvailable = true;

            }

            else
            {

                Dispatcher.BeginInvoke((Action)delegate () {
                    label_txt.Content = "No update found";
                });

            }

            Dispatcher.BeginInvoke((Action)delegate ()
            {
                  PB_Loader.Value = 100;
            });

            Thread.Sleep(100);

            Dispatcher.BeginInvoke((Action)delegate () {
                Close();
            });


        }
    }
}

