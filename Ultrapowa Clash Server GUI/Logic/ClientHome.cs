using Ionic.Zlib;
using System.Collections.Generic;
using Ultrapowa_Clash_Server_GUI.Helpers;

namespace Ultrapowa_Clash_Server_GUI.Logic
{
    internal class ClientHome : Base
    {
        private readonly long m_vId;
        private int m_vRemainingShieldTime;
        private byte[] m_vSerializedVillage;

        public ClientHome() : base(0)
        {
        }

        public ClientHome(long id)
            : base(0)
        {
            m_vId = id;
        }

        public override byte[] Encode()
        {
            var data = new List<byte>();

            data.AddRange(base.Encode());

            data.AddInt64(m_vId);
            data.AddInt32(m_vRemainingShieldTime);

            data.AddRange(new byte[]
            {
                0x00, 0x00, 0x04, 0xB0,
                0x00, 0x00, 0x00, 0x3C,
                0x01
            });

            data.AddInt32(m_vSerializedVillage.Length + 4);

            data.AddRange(new byte[]
            {
                //0xED, 0x0D, 0x00, 0x00,
                0xFF, 0xFF, 0x00, 0x00
            }); //patch 6.407

            data.AddRange(m_vSerializedVillage);

            return data.ToArray();
        }

        public byte[] GetHomeJSON()
        {
            return m_vSerializedVillage;
        }

        public void SetHomeJSON(string json)
        {
            m_vSerializedVillage = ZlibStream.CompressString(json);
        }

        public void SetShieldDurationSeconds(int seconds)
        {
            m_vRemainingShieldTime = seconds;
        }
    }
}