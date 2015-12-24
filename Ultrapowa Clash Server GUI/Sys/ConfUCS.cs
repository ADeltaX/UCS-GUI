using System;
using System.Collections.Generic;

namespace Ultrapowa_Clash_Server_GUI.Sys
{
    public class ConfUCS
    {
        public static Version thisAppVer = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

        public static bool DebugMode = true;
        public static string Language = "en-US";
        public static bool EnableRemoteControl = false;
        public static bool IsConsoleMode = true; //Will start console first
        public static bool IsDefaultMode = false;
        public static bool IsLogEnabled = true;
        public static string LogDirectory = System.IO.Directory.GetCurrentDirectory() + @"\logs\";
        public static readonly string VersionUCS = thisAppVer.Major + "." + thisAppVer.Minor + "." + thisAppVer.Build + "." + thisAppVer.MinorRevision;

        //Updater section
        public static bool IsUpdateAvailable = false;
        public static Version NewVer = new Version();
        public static string UrlPage = "http://www.ultrapowa.com";
        public static string Changelog = "Error, changelog not downloaded...";

    }
}
