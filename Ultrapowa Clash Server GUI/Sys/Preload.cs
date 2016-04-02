using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using UCS.UI;
using UCS.Core;

namespace UCS.Sys
{
    class Preload
    {

        List<string> GameListFiles;
        List<string> CoreFiles;
        List<string> MissingNotReqFiles = new List<string>();
        double localvalue = 0;
        double Inc = 0;
        string MonoLine;

        public void PreloadThings()
        {

            if (ConfUCS.IsPreloaded) return;

            InitializeFileList();

            double minicounter = 0;

            if (ConfUCS.IsConsoleMode) Console.Write("Checking gamefiles... ");

            if (!ConfUCS.IsConsoleMode) UI.SplashScreen.SS.Dispatcher.BeginInvoke((Action)delegate ()
            {
               // UI.SplashScreen.SS.label_txt.Content = "Verifying game files... ";
            });

            //Verify GameFiles
            foreach (string DataG in GameListFiles)
            {

                if (!System.IO.File.Exists(System.IO.Directory.GetCurrentDirectory() + @"\" + DataG))
                {
                    MissingNotReqFiles.Add(DataG);
                }

                minicounter++;
                localvalue = (minicounter / GameListFiles.Count);

                if (!ConfUCS.IsConsoleMode) UI.SplashScreen.SS.Dispatcher.BeginInvoke((Action)delegate ()
                {
                    UI.SplashScreen.SS.PB_Loader.Value = Inc + (50 * localvalue);
                });

            }

            minicounter = 0;
            Inc = 50;

            if (!ConfUCS.IsConsoleMode) UI.SplashScreen.SS.Dispatcher.BeginInvoke((Action)delegate ()
            {
                UI.SplashScreen.SS.label_txt.Content = "Verifying required data... ";
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
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("OK!\n");
            Console.ResetColor();

            if (ConfUCS.IsConsoleMode) Console.Write("Checking corefiles... ");
            //Verify CoreFiles (DLL, Database, and so on)
            foreach (string DataG in CoreFiles)
            {
                if (!System.IO.File.Exists(System.IO.Directory.GetCurrentDirectory() + @"\" + DataG))
                {
                    MessageBox.Show(string.Format("The required file {0} in directory {1} is missing. Cannot continue.",
                        DataG, System.IO.Directory.GetCurrentDirectory()), "Error required file", MessageBoxButton.OK, MessageBoxImage.Error);

                    Thread t = new Thread(() =>
                    {
                        UI.SplashScreen.SS.Dispatcher.BeginInvoke((Action)delegate ()
                        {
                            Application.Current.Shutdown();
                        });
                    });
                    t.Start(); //Goodbye application :P
                    Assembly.LoadFrom(System.IO.Directory.GetCurrentDirectory() + @"\" + DataG);
                }

                minicounter++;
                localvalue = (minicounter / CoreFiles.Count);

                if (!ConfUCS.IsConsoleMode) UI.SplashScreen.SS.Dispatcher.BeginInvoke((Action)delegate ()
                {
                    UI.SplashScreen.SS.PB_Loader.Value = Inc + (30 * localvalue);
                });
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("OK!\n");
            Console.ResetColor();

            if (!ConfUCS.IsConsoleMode)
            {
                UI.SplashScreen.SS.Dispatcher.BeginInvoke((Action)delegate ()
                {
                    UI.SplashScreen.SS.label_txt.Content = "Checking update... ";
                    UI.SplashScreen.SS.PB_Loader.Value = 80;
                });

                UpdateChecker.Check();

                UI.SplashScreen.SS.Dispatcher.BeginInvoke((Action)delegate ()
                {
                    UI.SplashScreen.SS.PB_Loader.Value = 100;
                    UI.SplashScreen.SS.Close();
                });

            }
            ConfUCS.IsPreloaded = true;
        }


        //This is a list of required files. Will check if exist, if not, will show a message and stop.
        //This prevent system crash/corruption datas.
        private void InitializeFileList()
        {
            GameListFiles = new List<string>
            {
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
                @"gamefiles\pve\level50.json"
            };

            CoreFiles = new List<string>
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
