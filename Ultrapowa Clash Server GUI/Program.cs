using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Ultrapowa_Clash_Server_GUI.Core;
using Ultrapowa_Clash_Server_GUI.Helpers;
using Ultrapowa_Clash_Server_GUI.Network;

using Debugger = Ultrapowa_Clash_Server_GUI.Core.Debugger;
using Menu = Ultrapowa_Clash_Server_GUI.Core.Menu;

namespace Ultrapowa_Clash_Server_GUI
{
    internal class Program
    {
        private static bool isclosing = false;
        public static readonly int port = Utils.parseConfigInt("serverPort");

        [DllImport("kernel32.dll")]
        public static extern int GetConsoleWindow();

        private static void InitConsoleStuff()
        {
            Console.WriteLine("www.ultrapowa.com");
            Console.WriteLine("");
            Console.WriteLine("Server starting...");
            Console.ResetColor();
        }

        private static void InitProgramThreads()
        {
            Debugger.WriteLine("\t", null, 5);
            Debugger.WriteLine("Server Thread's:", null, 5, ConsoleColor.Blue);
            var programThreads = new List<Thread>();
            for (var i = 0; i < int.Parse(ConfigurationManager.AppSettings["programThreadCount"]); i++)
            {
                var pt = new ProgramThread();
                programThreads.Add(new Thread(pt.Start));
                programThreads[i].Start();
                Debugger.WriteLine("\tServer Running On Thread " + i, null, 5, ConsoleColor.Blue);
            }
            Console.ResetColor();
        }

        public static void ExitProgram()
        {
            Console.WriteLine("Starting saving all player to database");
            foreach (var l in ResourcesManager.GetOnlinePlayers())
            {
                DatabaseManager.Singelton.Save(l);
            }
            Environment.Exit(1);
        }

        public static void RestartProgram()
        {
            foreach (var l in ResourcesManager.GetOnlinePlayers())
            {
                DatabaseManager.Singelton.Save(l);
            }
            Process.Start(@"tools\ucs-restart.bat");
        }

        private static void InitUCS()
        {
            new ResourcesManager();
            new ObjectManager();
            new Gateway().Start();
            new ApiManager();
            if (!Directory.Exists("logs"))
            {
                Console.WriteLine("Folder \"logs/\" does not exist. Let me create one..");
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

            Debugger.SetLogLevel(Utils.parseConfigInt("loggingLevel"));
            Logger.SetLogLevel(Utils.parseConfigInt("loggingLevel"));

            InitProgramThreads();

            if (Utils.parseConfigInt("loggingLevel") >= 5)
            {
                Debugger.WriteLine("\t", null, 5);
                Debugger.WriteLine("Played ID's:", null, 5, ConsoleColor.Blue);
                foreach (var id in ResourcesManager.GetAllPlayerIds())
                {
                    Debugger.WriteLine("\t" + id, null, 5, ConsoleColor.Blue);
                }
                Debugger.WriteLine("\t", null, 5);
            }
            Console.WriteLine("Server started on port " + port + ". Let's play Clash of Clans!");     

            if (Convert.ToBoolean(Utils.parseConfigString("consoleCommand")))
            {
                new Menu();
            }
            else
            {
                //Application.Run(new UCSManager());
            }
        }

        private static void Main(string[] args)
        {
            var win = GetConsoleWindow();
            if (Convert.ToBoolean(Utils.parseConfigString("guiMode")))
            {
                ShowWindow(win, 0);
                //Application.Run(new UCSGui());
            }
            else
            {
                ShowWindow(win, 5);
                ExitHandler.SetConsoleCtrlHandler(new ExitHandler.HandlerRoutine(ExitHandler.ConsoleCtrlCheck), true);
                InitConsoleStuff();
                InitUCS();

            }
        }

        [DllImport("user32.dll")]
        private static extern int ShowWindow(int Handle, int showState);
    }
}