using System.Collections.Generic;
using Ultrapowa_Clash_Server_GUI.Core;
using Ultrapowa_Clash_Server_GUI.Logic;
using Ultrapowa_Clash_Server_GUI.Network;

namespace Ultrapowa_Clash_Server_GUI
{
    internal class ProgramThread
    {
        private readonly MessageManager mm;
        private readonly PacketManager pm;
        private List<Level> list;

        public bool m_vRunning = false;

        public ProgramThread()
        {
            //rm = new ResourcesManager();
            //om = new ObjectManager();
            pm = new PacketManager();
            mm = new MessageManager();
        }

        public ProgramThread(List<Level> list)
        {
            this.list = list;
        }

        public void Start()
        {
            pm.Start();
            mm.Start();
        }

        public void Stop()
        {
            pm.Stop();
            mm.Stop();
        }
    }
}