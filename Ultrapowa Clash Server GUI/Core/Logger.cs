using System;
using System.IO;
using System.Text.RegularExpressions;
using Ultrapowa_Clash_Server_GUI.PacketProcessing;

namespace Ultrapowa_Clash_Server_GUI.Core
{
    internal static class Logger
    {
        private static readonly object m_vSyncObject = new object();

        private static readonly TextWriter m_vTextWriter;

        private static int m_vLogLevel;

        static Logger()
        {
            m_vTextWriter =
                TextWriter.Synchronized(File.AppendText("logs/data_" + DateTime.Now.ToString("yyyy,MM,dd") + ".log"));
            m_vLogLevel = 1;
        }

        public static void SetLogLevel(int level)
        {
            m_vLogLevel = level;
        }

        public static void WriteLine(Message p, string prefix = null, int logLevel = 4)
        {
            if (logLevel <= m_vLogLevel)
            {
                lock (m_vSyncObject)
                {
                    m_vTextWriter.Write(DateTime.Now.ToString("yyyy/MM/dd/HH/mm/ss"));
                    m_vTextWriter.Write(";");
                    if (!string.IsNullOrEmpty(prefix))
                    {
                        m_vTextWriter.Write(prefix);
                        m_vTextWriter.Write(";");
                    }
                    m_vTextWriter.Write(p.GetMessageType().ToString());
                    m_vTextWriter.Write("(");
                    m_vTextWriter.Write(p.GetMessageVersion().ToString());
                    m_vTextWriter.Write(")");
                    m_vTextWriter.Write(";");
                    m_vTextWriter.Write(p.GetLength().ToString());
                    m_vTextWriter.Write(";");
                    m_vTextWriter.WriteLine(p.ToHexString());
                    m_vTextWriter.WriteLine(Regex.Replace(p.ToString(), @"[^\u0020-\u007F]", "."));
                    m_vTextWriter.Flush();
                }
            }
        }

        public static void WriteLine(string s, string prefix = null, int logLevel = 4)
        {
            if (logLevel <= m_vLogLevel)
            {
                lock (m_vSyncObject)
                {
                    m_vTextWriter.Write("{0} {1}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString());
                    m_vTextWriter.Write(";");
                    if (string.IsNullOrEmpty(prefix))
                    {
                        m_vTextWriter.Write(prefix);
                        m_vTextWriter.Write(";");
                    }
                    m_vTextWriter.WriteLine(s);
                    m_vTextWriter.Flush();
                }
            }
        }
    }
}