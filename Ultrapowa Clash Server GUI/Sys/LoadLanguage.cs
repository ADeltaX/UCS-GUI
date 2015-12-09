using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Ultrapowa_Clash_Server_GUI.Sys
{
    class LoadLanguage
    {
        private void SetLanguage()
        {
            switch (Thread.CurrentThread.CurrentCulture.ToString())
            {
                case "en-US":

                    break;
                case "it-IT":

                    break;
                default:

                    break;
            }

        }
    }
}
