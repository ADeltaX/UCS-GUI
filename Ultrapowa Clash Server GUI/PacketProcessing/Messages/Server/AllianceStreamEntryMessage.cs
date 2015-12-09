using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ultrapowa_Clash_Server_GUI.Logic;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    //Packet 24312
    class AllianceStreamEntryMessage : Message
    {
        private StreamEntry m_vStreamEntry;

        public AllianceStreamEntryMessage(Client client) : base(client)
        {
            SetMessageType(24312);
        }

        public void SetStreamEntry(StreamEntry entry)
        {
            m_vStreamEntry = entry;
        }

        public override void Encode()
        {
            List<Byte> pack = new List<Byte>();

            pack.AddRange(m_vStreamEntry.Encode());

            SetData(pack.ToArray());
        }
    }
}
