using System;
using System.Diagnostics;
using System.Windows;
using Ultrapowa_Clash_Server_GUI.Sys;
namespace Ultrapowa_Clash_Server_GUI
{
    /// <summary>
    /// Logica di interazione per App.xaml
    /// </summary>
    public partial class App : Application
    {

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            for (int i = 0; i != e.Args.Length; ++i)
            {
                if (e.Args[i].ToLower() == "/gui") ConfUCS.IsConsoleMode = false;
                if (e.Args[i].ToLower() == "/default") ConfUCS.IsDefaultMode = true;
                if (e.Args[i].ToLower() == "/nodebug") ConfUCS.DebugMode = false;
            }

            if (ConfUCS.IsConsoleMode == false)
            {
                //MainWindow SC = new MainWindow();
                //SC.Show();
                SplashScreen SC = new SplashScreen();
                SC.Show();

            }
            else
            {

                IntPtr ptr = ConsoleManage.GetForegroundWindow();
                int u;
                ConsoleManage.GetWindowThreadProcessId(ptr, out u);
                Process process = Process.GetProcessById(u);
                if (process.ProcessName == "cmd")
                {
                    ConsoleManage.AttachConsole(process.Id);
                }
                else
                {
                    ConsoleManage.AllocConsole();
                }
                ConsoleManage.DisableConsoleExit();
                Console.Clear();
                Version thisAppVer = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                Console.Title = "UCS Server " + ConfUCS.VersionUCS;
                SplashScreen SC = new SplashScreen();
                SC.Show();
            }
        }
    }
}
