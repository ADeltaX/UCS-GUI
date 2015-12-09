using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Configuration;
using Ultrapowa_Clash_Server_GUI.PacketProcessing;
using Ultrapowa_Clash_Server_GUI.Core;
using Ultrapowa_Clash_Server_GUI.GameFiles;
using Newtonsoft.Json;

namespace Ultrapowa_Clash_Server_GUI.Logic
{
    class BunkerComponent : Component
    {
        private const int m_vType = 0x01AB3F00;

        public BunkerComponent()
        {
            //Deserialization
        }

        public override int Type
        {
            get { return 7; }
        }
    }
}
