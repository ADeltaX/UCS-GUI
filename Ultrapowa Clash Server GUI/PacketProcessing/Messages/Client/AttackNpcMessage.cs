using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Ultrapowa_Clash_Server_GUI.Helpers;
using Ultrapowa_Clash_Server_GUI.Logic;
using Ultrapowa_Clash_Server_GUI.Network;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    //Packet 14134
    class AttackNpcMessage : Message
    {
        public AttackNpcMessage(Client client, BinaryReader br) : base(client, br)
        {
        }

        public override void Decode()
        {
            using (var br = new BinaryReader(new MemoryStream(GetData())))
            {
                LevelId = br.ReadInt32WithEndian();
            }
        }

        public int LevelId { get; set; }

        public override void Process(Level level)
        {
            NpcDataMessage san = new NpcDataMessage(this.Client, level, this);
            PacketManager.ProcessOutgoingPacket(san);
        }
    }
}
