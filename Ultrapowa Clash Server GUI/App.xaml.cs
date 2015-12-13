using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;

namespace Ultrapowa_Clash_Server_GUI
{
    /// <summary>
    /// Logica di interazione per App.xaml
    /// </summary>
    public partial class App : Application
    {
        [DllImport("kernel32.dll")]
        static extern bool AttachConsole(int dwProcessId);
        private const int ATTACH_PARENT_PROCESS = -1;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            for (int i = 0; i != e.Args.Length; ++i)
            {
                if (e.Args[i].ToLower() == "/console") Sys.ConfUCS.IsConsoleMode = true;
                if (e.Args[i].ToLower() == "/default") Sys.ConfUCS.IsDefaultMode = true;
                if (e.Args[i].ToLower() == "/nodebug") Sys.ConfUCS.DebugMode = false;
            }

            if (Sys.ConfUCS.IsConsoleMode == false)
            {
                SplashScreen SC = new SplashScreen();
                SC.Show();
            }
            else
            {
                AttachConsole(ATTACH_PARENT_PROCESS);
                Console.WriteLine("Test");
            }
        }
    }
}
