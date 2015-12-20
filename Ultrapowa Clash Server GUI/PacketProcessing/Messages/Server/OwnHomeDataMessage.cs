using System;
using System.Collections.Generic;
using Ultrapowa_Clash_Server_GUI.Helpers;
using Ultrapowa_Clash_Server_GUI.Logic;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    //Packet 24101
    internal class OwnHomeDataMessage : Message
    {
        public OwnHomeDataMessage(Client client, Level level) : base(client)
        {
            SetMessageType(24101);
            Player = level;
        }

        public Level Player { get; set; }

        private byte[] m_vSerializedVillage { get; set; }

        public override void Encode()
        {
            var data = new List<byte>();

            var ch = new ClientHome(Player.GetPlayerAvatar().GetId());
            ch.SetShieldDurationSeconds(Player.GetPlayerAvatar().RemainingShieldTime);
            ch.SetHomeJSON(Player.SaveToJSON());

            //data.AddRange(BitConverter.GetBytes(Player.GetPlayerAvatar().GetSecondsFromLastUpdate()).Reverse());
            data.AddInt32(0); //replace previous after patch
            data.AddInt32(-1);
            data.AddInt32((int) Player.GetTime().Subtract(new DateTime(1970, 1, 1)).TotalSeconds);

            //0x54, 0x47, 0xFD, 0x10 //patch 21/10
            data.AddRange(ch.Encode());
            data.AddRange(Player.GetPlayerAvatar().Encode());

            //7.1
            data.AddInt32(0);
            data.AddInt32(0);

            SetData(data.ToArray());
        }
    }
}