using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.IO;
using Ultrapowa_Clash_Server_GUI.Helpers;
using Ultrapowa_Clash_Server_GUI.Logic;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    class Command
    {
        public Command() { }

        public virtual void Execute(Level level)
        {
        }

        public virtual byte[] Encode()
        {
            
            return new List<byte>().ToArray();
        }
    }
}
