using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ultrapowa_Clash_Server_GUI.Core;
using Ultrapowa_Clash_Server_GUI.Helpers;
using Ultrapowa_Clash_Server_GUI.Logic;
using Ultrapowa_Clash_Server_GUI.Network;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    internal class Askfor20100 : Message
    {
        /// <summary>
        /// Unknown integer 1.
        /// </summary>
        public int Unknown1;
        /// <summary>
        /// Unknown integer 2.
        /// </summary>
        public int Unknown2;
        /// <summary>
        /// Unknown integer 3.
        /// </summary>
        public int Unknown3;
        /// <summary>
        /// Unknown integer 4.
        /// </summary>
        public int Unknown4;
        /// <summary>
        /// Unknown integer 5.
        /// </summary>
        public int Unknown5;
        /// <summary>
        /// String that is probably needed for the new encryption
        /// schema.
        /// </summary>
        public string TheString;
        /// <summary>
        /// Unknown integer 6.
        /// </summary>
        public int Unknown6;
        /// <summary>
        /// Unknown integer 7.
        /// </summary>
        public int Unknown7;

        public Askfor20100(Client client, BinaryReader br) : base(client, br)
        {
            //Not sure if there should be something here o.O
        }

        public override void Decode()
        {
            using (var br = new BinaryReader(new MemoryStream(GetData())))
            {
                Unknown1 = br.ReadInt32();
                Unknown2 = br.ReadInt32();
                Unknown3 = br.ReadInt32();
                Unknown4 = br.ReadInt32();
                Unknown5 = br.ReadInt32();
                Unknown6 = br.ReadInt32();
                Unknown7 = br.ReadInt32();

            }
        }

        public override void Process(Level level)
        {
            MainWindow.RemoteWindow.WriteConsole(Convert.ToString(Unknown1), (int)MainWindow.level.WARNING);
            MainWindow.RemoteWindow.WriteConsole(Convert.ToString(Unknown2), (int)MainWindow.level.WARNING);
            MainWindow.RemoteWindow.WriteConsole(Convert.ToString(Unknown3), (int)MainWindow.level.WARNING);
            MainWindow.RemoteWindow.WriteConsole(Convert.ToString(Unknown4), (int)MainWindow.level.WARNING);
            MainWindow.RemoteWindow.WriteConsole(Convert.ToString(Unknown5), (int)MainWindow.level.WARNING);
            MainWindow.RemoteWindow.WriteConsole(Convert.ToString(Unknown6), (int)MainWindow.level.WARNING);
            MainWindow.RemoteWindow.WriteConsole(Convert.ToString(Unknown7), (int)MainWindow.level.WARNING);
            var p = new LoginFailedMessage(Client);
            p.SetErrorCode(10);
            p.RemainingTime(0);
            p.SetReason("You are connecting with 8.67 client but UCS not support it yet");
            PacketManager.ProcessOutgoingPacket(p);
        }
    }
}