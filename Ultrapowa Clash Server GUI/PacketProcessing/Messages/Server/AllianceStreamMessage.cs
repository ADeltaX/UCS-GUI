using System.Collections.Generic;
using System.Linq;
using Ultrapowa_Clash_Server_GUI.Helpers;
using Ultrapowa_Clash_Server_GUI.Logic;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    //Packet 24311
    internal class AllianceStreamMessage : Message
    {
        private readonly Alliance m_vAlliance;

        public AllianceStreamMessage(Client client, Alliance alliance)
            : base(client)
        {
            SetMessageType(24311);
            m_vAlliance = alliance;
        }

        public override void Encode()
        {
            var pack = new List<byte>();

            var chatMessages = m_vAlliance.GetChatMessages().ToList(); //avoid concurrent access issues

            pack.AddInt32(chatMessages.Count);
            foreach (var chatMessage in chatMessages)
            {
                if (Client.GetLevel().isPermittedUser())
                {
                    var name = chatMessage.GetSenderName();
                    chatMessage.SetSenderName(name + " #" + chatMessage.GetSenderId());
                    pack.AddRange(chatMessage.Encode());
                    chatMessage.SetSenderName(name);
                }
                else
                {
                    pack.AddRange(chatMessage.Encode());
                }
            }

            SetData(pack.ToArray());
        }
    }
}