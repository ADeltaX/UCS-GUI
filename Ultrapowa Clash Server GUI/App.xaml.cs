using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace Ultrapowa_Clash_Server_GUI
{
    /// <summary>
    /// Logica di interazione per App.xaml
    /// </summary>
    public partial class App : Application
    {

        #region Console Manage
        [DllImport("kernel32.dll")]
        static extern bool AttachConsole(int dwProcessId);
        private const int ATTACH_PARENT_PROCESS = -1;

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);

        const uint SW_HIDE = 0;
        const uint SW_SHOWNORMAL = 1;
        const uint SW_SHOWNOACTIVATE = 4;
        public static bool ConsoleVisible { get; private set; }

        public static void HideConsole()
        {
            IntPtr handle = GetConsoleWindow();
            ShowWindow(handle, SW_HIDE);
            ConsoleVisible = false;

        }
        public static void ShowConsole(bool active = true)
        {
            IntPtr handle = GetConsoleWindow();
            if (active) { ShowWindow(handle, SW_SHOWNORMAL); }
            else { ShowWindow(handle, SW_SHOWNOACTIVATE); }
            ConsoleVisible = true;
        }

        // Disable Console Exit Button
        [DllImport("user32.dll")]
        static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32.dll")]
        static extern IntPtr DeleteMenu(IntPtr hMenu, uint uPosition, uint uFlags);

        const uint SC_CLOSE = 0xF060;
        const uint MF_BYCOMMAND = (uint)0x00000000L;

        public static void DisableConsoleExit()
        {
            IntPtr handle = GetConsoleWindow();
            IntPtr exitButton = GetSystemMenu(handle, false);
            if (exitButton != null) DeleteMenu(exitButton, SC_CLOSE, MF_BYCOMMAND);
        }

        #endregion

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
                DisableConsoleExit();
                Console.Clear();
                Version thisAppVer = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                Console.Title = "UCS Server" + thisAppVer.Major + "." + thisAppVer.Minor + "." + thisAppVer.Build + "." + thisAppVer.MinorRevision;
                SplashScreen SC = new SplashScreen();
                SC.Show();
            }
        }
    }
}
