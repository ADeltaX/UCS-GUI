using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCS.Sys;

namespace UCS.Core
{
    class CoreWriter
    {
        public static void Write(string content)
        {
            if (ConfUCS.IsConsoleMode)
            {
                Console.Write(content);
            }
            else
            {
                try
                {
                    MainWindow.RemoteWindow.RTB_Console.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        MainWindow.RemoteWindow.RTB_Console.AppendText(content);
                    }));
                }
                catch
                { }

            }
        }
        public static void WriteLine(string content)
        {
            if (ConfUCS.IsConsoleMode)
            {
                Console.WriteLine(content);
            }
            else
            {
                try
                {
                    MainWindow.RemoteWindow.RTB_Console.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        MainWindow.RemoteWindow.RTB_Console.AppendText(content + "\n");
                    }));
                }
                catch
                { }
            }
        }
    }
}
