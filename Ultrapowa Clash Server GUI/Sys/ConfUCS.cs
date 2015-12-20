using System;
using System.Collections.Generic;

namespace Ultrapowa_Clash_Server_GUI.Sys
{
    public class ConfUCS
    {

        public static bool DebugMode = true;
        public static string Language = "en-US";
        public static bool EnableRemoteControl = false;
        public static bool IsConsoleMode = false;
        public static bool IsDefaultMode = false;
        public static bool IsLogEnabled = true;
        public static string LogDirectory = System.IO.Directory.GetCurrentDirectory() + @"\logs\";

        public static bool IsUpdateAvailable = true;
        public static Version NewVer = new Version();
        public static string UrlPage = "http://www.ultrapowa.com";
        public static string Changelog = "Error, changelog not downloaded...";

    }
}
