using System.Collections.Generic;
using Ultrapowa_Clash_Server_GUI.Logic;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    //Packet 24312
    internal class AllianceStreamEntryMessage : Message
    {
        private StreamEntry m_vStreamEntry;

        public AllianceStreamEntryMessage(Client client) : base(client)
        {
            SetMessageType(24312);
        }

        public override void Encode()
        {
            var pack = new List<byte>();

            pack.AddRange(m_vStreamEntry.Encode());

            SetData(pack.ToArray());
        }

        public void SetStreamEntry(StreamEntry entry)
        {
            m_vStreamEntry = entry;
        }
    }
}