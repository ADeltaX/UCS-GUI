using System.Collections.Generic;
using Ultrapowa_Clash_Server_GUI.Helpers;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    //Packet 24111
    internal class AvatarNameChangeOkMessage : Message
    {
        private readonly int m_vServerCommandType;

        private string m_vAvatarName;

        public AvatarNameChangeOkMessage(Client client) : base(client)
        {
            SetMessageType(24111);

            m_vServerCommandType = 0x03;
            m_vAvatarName = "Megapumba";
        }

        public override void Encode()
        {
            var pack = new List<byte>();

            pack.AddInt32(m_vServerCommandType);
            pack.AddString(m_vAvatarName);
            pack.AddInt32(1);
            pack.AddInt32(-1);

            SetData(pack.ToArray());
        }

        public string GetAvatarName()
        {
            return m_vAvatarName;
        }

        public void SetAvatarName(string name)

        {
            m_vAvatarName = name;
        }
    }
}