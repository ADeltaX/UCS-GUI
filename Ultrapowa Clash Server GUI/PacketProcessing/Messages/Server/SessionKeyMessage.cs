using System.Collections.Generic;
using Ultrapowa_Clash_Server_GUI.Helpers;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    //Packet 20000
    internal class SessionKeyMessage : Message
    {
        public SessionKeyMessage(Client client)
            : base(client)
        {
            SetMessageType(20000);
            Key = new byte[]
            {0xD7, 0x16, 0x28, 0xA8, 0x08, 0xD2, 0x3C, 0x1D, 0xD9, 0x26, 0xA4, 0xB2, 0x1C, 0xB4, 0xB9, 0x32};
        }

        public byte[] Key { get; set; }

        public override void Encode()
        {
            var pack = new List<byte>();

            pack.AddInt32(Key.Length);
            pack.AddRange(Key);
            pack.AddInt32(1);

            SetData(pack.ToArray());
        }
    }
}