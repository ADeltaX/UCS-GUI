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
using Newtonsoft.Json.Linq;

namespace Ultrapowa_Clash_Server_GUI.Logic
{
    class Trap : ConstructionItem
    {
        public override int ClassId
        {
            get { return 4; }
        }

        public Trap(Data data, Level l) : base(data, l)
        {
            AddComponent(new TriggerComponent());
        }

        public TrapData GetTrapData()
        {
            return (TrapData)GetData();
        }
    }
}
