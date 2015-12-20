using System;
using System.Collections.Generic;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    internal static class GameOpCommandFactory
    {
        private static readonly Dictionary<string, Type> m_vCommands;

        static GameOpCommandFactory()
        {
            m_vCommands = new Dictionary<string, Type>();
            m_vCommands.Add("/attack", typeof (AttackGameOpCommand));
            m_vCommands.Add("/ban", typeof (BanGameOpCommand));
            m_vCommands.Add("/kick", typeof (KickGameOpCommand));
            m_vCommands.Add("/rename", typeof (RenameAvatarGameOpCommand));
            m_vCommands.Add("/setprivileges", typeof (SetPrivilegesGameOpCommand));
            m_vCommands.Add("/shutdown", typeof (ShutdownServerGameOpCommand));
            m_vCommands.Add("/unban", typeof (UnbanGameOpCommand));
            m_vCommands.Add("/visit", typeof (VisitGameOpCommand));
            m_vCommands.Add("/sysmsg", typeof (SystemMessageGameOpCommand));
            m_vCommands.Add("/banchat", typeof (BanChatGameOpCommand));
            m_vCommands.Add("/sysrestart", typeof (SystemRestartMessageGameOpCommand));
            m_vCommands.Add("/serverinfo", typeof (ServerStatusGameOpCommand));
            m_vCommands.Add("/reloadfilter", typeof (ReloadFilterGameOpCommand));
            m_vCommands.Add("/setscore", typeof (SetUsersScoreGameOpCommand));
            m_vCommands.Add("/min", typeof (MinResourcesGameOpCommand));
            m_vCommands.Add("/max", typeof (MaxResourcesGameOpCommand));
            m_vCommands.Add("/setleague", typeof (SetLeagueGameOpCommand));
            m_vCommands.Add("/pm", typeof (PrivateMessageCommand));
            m_vCommands.Add("/myid", typeof (GetIdCommand));

            //m_vCommands.Add("/resethome", typeof(ResetHomeGameOpCommand));
            //m_vCommands.Add("/faq", typeof (FaqGameOpCommand));
            m_vCommands.Add("/info", typeof (InfoGameOpCommand));
            m_vCommands.Add("/team", typeof (TeamGameOpCommand));

            //m_vCommands.Add("/help", typeof (HelpGameOpCommand));
        }

        public static object Parse(string command)
        {
            var commandArgs = command.Split(' ');
            object result = null;
            if (commandArgs.Length > 0)
            {
                if (m_vCommands.ContainsKey(commandArgs[0]))
                {
                    var type = m_vCommands[commandArgs[0]];
                    var ctor = type.GetConstructor(new[] {typeof (string[])});
                    result = ctor.Invoke(new object[] {commandArgs});
                }
            }
            return result;
        }
    }
}