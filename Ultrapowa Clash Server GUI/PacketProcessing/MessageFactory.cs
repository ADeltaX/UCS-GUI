using System;
using System.Collections.Generic;
using System.IO;
using Ultrapowa_Clash_Server_GUI.Core;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    //Command list: LogicCommand::createCommand
    internal static class MessageFactory
    {
        private static readonly Dictionary<int, Type> m_vMessages;

        static MessageFactory()
        {
            m_vMessages = new Dictionary<int, Type>();
            m_vMessages.Add(10100, typeof(Askfor20100));
            m_vMessages.Add(10101, typeof (LoginMessage));  
            m_vMessages.Add(10108, typeof (KeepAliveMessage));
            m_vMessages.Add(10212, typeof (ChangeAvatarNameMessage));
            m_vMessages.Add(14101, typeof (GoHomeMessage));
            m_vMessages.Add(14102, typeof (ExecuteCommandsMessage));
            m_vMessages.Add(14113, typeof (VisitHomeMessage));
            m_vMessages.Add(14134, typeof (AttackNpcMessage));
            m_vMessages.Add(14316, typeof (EditClanSettingsMessage));
            m_vMessages.Add(14301, typeof (CreateAllianceMessage));
            m_vMessages.Add(14302, typeof (AskForAllianceDataMessage));
            m_vMessages.Add(14303, typeof (AskForJoinableAlliancesListMessage));
            m_vMessages.Add(14305, typeof (JoinAllianceMessage));
            m_vMessages.Add(14306, typeof (PromoteAllianceMemberMessage));
            m_vMessages.Add(14308, typeof (LeaveAllianceMessage));
            m_vMessages.Add(14315, typeof (ChatToAllianceStreamMessage));
            m_vMessages.Add(14324, typeof (SearchAlliancesMessage));
            m_vMessages.Add(14325, typeof (AskForAvatarProfileMessage));
            m_vMessages.Add(14715, typeof (SendGlobalChatLineMessage));
            m_vMessages.Add(14403, typeof (AskForPlayerLeagueList));
        }

        public static object Read(Client c, BinaryReader br, int packetType)
        {
            if (m_vMessages.ContainsKey(packetType))
            {
                return Activator.CreateInstance(m_vMessages[packetType], c, br);
            }
            Debugger.WriteLine("[U] " + packetType + " Unhandled Message (ignored)", null, 5, ConsoleColor.Red);
            return null;
        }
    }
}