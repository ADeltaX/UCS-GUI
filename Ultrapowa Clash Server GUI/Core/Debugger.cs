using System;
using System.Configuration;
using System.IO;

namespace Ultrapowa_Clash_Server_GUI.Core
{
    internal static class Debugger
    {
        private static readonly object m_vSyncObject = new object();

        private static readonly TextWriter m_vTextWriter;

        private static int m_vLogLevel;

        static Debugger()
        {
            m_vTextWriter =
                TextWriter.Synchronized(File.AppendText("logs/debug_" + DateTime.Now.ToString("yyyy,MM,dd") + ".log"));
            m_vLogLevel = 1;
        }

        public static int GetLogLevel()
        {
            return m_vLogLevel;
        }

        public static void SetLogLevel(int level)
        {
            m_vLogLevel = level;
        }

        public static void WriteLine(string text, Exception ex = null, int logLevel = 4,
            ConsoleColor color = ConsoleColor.White)
        {
            var content = text;
            if (logLevel <= m_vLogLevel)
            {
                if (ex != null)
                    content += ex.ToString();
                if (color != ConsoleColor.White)
                {
                    Console.ForegroundColor = color;
                }
                else
                {
                    if (logLevel == 5)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                    }
                }
                if (Convert.ToBoolean(ConfigurationManager.AppSettings["debugMode"]))
                {
                    Console.WriteLine(content);
                    Console.ResetColor();
                }
                else
                {
                    Console.ResetColor();
                }
            }
            if (logLevel <= m_vLogLevel)
            {
                lock (m_vSyncObject)
                {
                    m_vTextWriter.Write(DateTime.Now.ToString("yyyy/MM/dd/HH/mm/ss"));
                    m_vTextWriter.Write("\t");
                    m_vTextWriter.WriteLine(content);
                    if (ex != null)
                        m_vTextWriter.WriteLine(ex.ToString());
                    m_vTextWriter.Flush();
                }
            }
        }
    }
}