using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using Ultrapowa_Clash_Server_GUI.Logic;
using Ultrapowa_Clash_Server_GUI.Helpers;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    static class GameOpCommandFactory
    {
        private static Dictionary<string, Type> m_vCommands;

        static GameOpCommandFactory()
        {
            m_vCommands = new Dictionary<string, Type>();
            m_vCommands.Add("/attack", typeof(AttackGameOpCommand));
            m_vCommands.Add("/ban", typeof(BanGameOpCommand));
            m_vCommands.Add("/kick", typeof(KickGameOpCommand));
            m_vCommands.Add("/rename", typeof(RenameAvatarGameOpCommand));
            m_vCommands.Add("/setprivileges", typeof(SetPrivilegesGameOpCommand));
            m_vCommands.Add("/shutdown", typeof(ShutdownServerGameOpCommand));
            m_vCommands.Add("/unban", typeof(UnbanGameOpCommand));
            m_vCommands.Add("/visit", typeof(VisitGameOpCommand));
            m_vCommands.Add("/sysmsg", typeof(SystemMessageGameOpCommand));
            m_vCommands.Add("/banchat", typeof(BanChatGameOpCommand));
            m_vCommands.Add("/sysrestart", typeof(SystemRestartMessageGameOpCommand));
            m_vCommands.Add("/sysinfo", typeof(ServerStatusGameOpCommand));
        }

        public static object Parse(string command)
        {
            string[] commandArgs = command.Split(' ');
            object result = null;
            if(commandArgs.Length > 0)
            {
                if (m_vCommands.ContainsKey(commandArgs[0]))
                {
                    Type type = m_vCommands[commandArgs[0]];
                    ConstructorInfo ctor = type.GetConstructor(new[] { typeof(string[]) });
                    result = ctor.Invoke(new object[] { commandArgs });
                }
            }
            return result;
        }
    }
}
