using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ultrapowa_Clash_Server_GUI.Sys
{
    public class ConfUCS
    {

        public static bool DebugMode = true;
        public static string Language = "en-US";
        public static bool EnableRemoteControl = false;
        public static bool IsConsoleMode = false;
        public static bool IsDefaultMode = false;

        public static bool IsUpdateAvailable = false;
        public static Version NewVer = new Version();
        public static string UrlPage = "";
        public static string Changelog = "";

    }
}
