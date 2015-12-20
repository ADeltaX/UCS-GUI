using System.IO;
using Ultrapowa_Clash_Server_GUI.Helpers;
using Ultrapowa_Clash_Server_GUI.Logic;
using Ultrapowa_Clash_Server_GUI.Network;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    //10212
    internal class ChangeAvatarNameMessage : Message
    {
        public ChangeAvatarNameMessage(Client client, BinaryReader br) : base(client, br)
        {
        }

        public string PlayerName { get; set; }

        public int PlayerNameLength { get; set; }

        public byte Unknown1 { get; set; }

        public override void Decode()
        {
            using (var br = new BinaryReader(new MemoryStream(GetData())))
            {
                PlayerName = br.ReadScString();
                Unknown1 = br.ReadByte();
            }
        }

        public override void Process(Level level)
        {
            level.GetPlayerAvatar().SetName(PlayerName);
            var p = new AvatarNameChangeOkMessage(Client);
            p.SetAvatarName(level.GetPlayerAvatar().GetAvatarName());
            PacketManager.ProcessOutgoingPacket(p);
        }
    }
}