using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ultrapowa_Clash_Server_GUI.Core;
using Ultrapowa_Clash_Server_GUI.Helpers;
using Ultrapowa_Clash_Server_GUI.Logic;
using Ultrapowa_Clash_Server_GUI.Network;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    class WTheHell : Message
    {

        private long m_vAllianceId;

        //Read the hell of the packet 10100

        public WTheHell(Client client, BinaryReader br) : base(client, br)
        {
            MainWindow.RemoteWindow.Dispatcher.BeginInvoke((Action)delegate () {
                MainWindow.RemoteWindow.WriteConsole("If I read this string then this class is readable, if not i'm useless", 3);
                using (var zr = new BinaryReader(new MemoryStream(GetData())))
                {
                    File.WriteAllBytes(Directory.GetCurrentDirectory() + @"\10100.packet",zr.ReadAllBytes());
                    MainWindow.RemoteWindow.WriteConsole(zr.ReadScString(), 3);
                }
                
            });

            

            

        }

        public override void Decode()
        {

        }

        public override void Process(Level level)
        {

        }
    }
}
