using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ultrapowa_Clash_Server_GUI.Logic;
using Ultrapowa_Clash_Server_GUI.Helpers;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    //Packet 24304
    class JoinableAllianceListMessage : Message
     {
        private List<Alliance> m_vAlliances;

        public JoinableAllianceListMessage(Client client) : base(client)
        {
            SetMessageType(24304);
            m_vAlliances = new List<Alliance>();
        }

        public override void Encode()
        {
            List<Byte> pack = new List<Byte>();
            pack.AddInt32(m_vAlliances.Count);
            foreach(var alliance in m_vAlliances)
            {
                pack.AddRange(alliance.EncodeFullEntry());
            }

            SetData(pack.ToArray());
        }

        public void SetJoinableAlliances(List<Alliance> alliances)
        {
            m_vAlliances = alliances;
        }
    }
}
