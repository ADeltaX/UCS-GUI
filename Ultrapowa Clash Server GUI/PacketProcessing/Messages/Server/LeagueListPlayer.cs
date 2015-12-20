using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using Ultrapowa_Clash_Server_GUI.Helpers;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    //Packet 20108
    internal class LeagueListPlayer : Message
    {
        public LeagueListPlayer(Client client)
            : base(client)
        {
            SetMessageType(24403);
        }

        public override void Encode()
        {
            var data = new List<byte>();
            data.AddInt64(1);
            data.AddInt32(300);
            data.AddInt32(21);
            data.AddInt32(4000);
            data.AddInt32(21);
            data.AddInt32(6);
            data.AddInt32(9999999);
            data.AddInt32(7);
            data.AddInt32(9999999);
            data.AddInt32(1);
            data.AddInt32(2);
            data.AddInt32(3);
            data.AddInt32(4);
            data.AddInt32(5);
            data.AddString("false");
            data.AddInt32(8);
            data.AddInt32(9);
            SetData(data.ToArray());
        }
    }
}