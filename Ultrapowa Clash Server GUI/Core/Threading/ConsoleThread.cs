using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Configuration;
using UCS.Helpers;
using UCS.Sys;
namespace UCS.Core.Threading
{
    class ConsoleThread
    {
        public static string Name = "Console Thread";
        public static string Description = "Manages Console I/O";
        public static string Version = "1.0.0";
        public static string Author = "ExPl0itR";

        /// <summary>
        /// Variable holding the thread itself
        /// </summary>
        private static Thread T { get; set; }

        private static string Title, Tmp, Command;
        /// <summary>
        /// Starts the Thread
        /// </summary>
        [STAThread]
        public void Start()
        {
            T = new Thread(() =>
            {
                if (ConfUCS.IsConsoleMode) Console.Title = ConfUCS.UnivTitle;
                CancelEvent(); //N00b proof

                /* ASCII Art centered */
                Console.WriteLine(
                      @"
                    888     888  .d8888b.   .d8888b.  
                    888     888 d88P  Y88b d88P  Y88b 
                    888     888 888    888 Y88b.      
                    888     888 888         ""Y888b.   
                    888     888 888            ""Y88b. 
                    888     888 888    888       ""888 
                    Y88b. .d88P Y88b  d88P Y88b  d88P 
                     ""Y88888P""   ""Y8888P""   ""Y8888P""  
                  ");
                
                Console.WriteLine("Ultrapowa Clash Server");
                Console.WriteLine("Visit www.ultrapowa.com | www.shard.site");
                Console.WriteLine("Starting the server...");
                Preload PT = new Preload();
                PT.PreloadThings();
                ControlTimer.StartPerformanceCounter();
                Console.WriteLine("");
                Debugger.SetLogLevel(int.Parse(ConfigurationManager.AppSettings["loggingLevel"]));
                Logger.SetLogLevel(int.Parse(ConfigurationManager.AppSettings["loggingLevel"]));
                NetworkThread.Start();
                MemoryThread.Start();
                ConfUCS.UnivTitle = "Ultrapowa Clash Server " + ConfUCS.VersionUCS + " | " + "ONLINE";
               if (ConfUCS.IsConsoleMode) CommandParser.ManageConsole();
            });
            T.SetApartmentState(ApartmentState.STA);
            T.Start();
        }

        private void CancelEvent()
        {
            var exitEvent = new ManualResetEvent(false);

            Console.CancelKeyPress += (sender, e) => {
                e.Cancel = true;
                exitEvent.Set();
            };
        }

        /// <summary>
        /// Stops the Thread
        /// </summary>
        public static void Stop()
        {
            if (T.ThreadState == ThreadState.Running)
                T.Abort();
        }
    }
}
