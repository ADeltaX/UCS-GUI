using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Ultrapowa_Clash_Server_GUI.Logic;
using Ultrapowa_Clash_Server_GUI.Helpers;
using Ultrapowa_Clash_Server_GUI.GameFiles;
using Ultrapowa_Clash_Server_GUI.Core;
using Ultrapowa_Clash_Server_GUI.Network;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    class UnbanGameOpCommand : GameOpCommand
    {
        private string[] m_vArgs;

        public UnbanGameOpCommand(string[] args)
        {
            m_vArgs = args;
            SetRequiredAccountPrivileges(2);
        }

        public override void Execute(Level level)
        {
            if(level.GetAccountPrivileges() >= GetRequiredAccountPrivileges())
            {
                if(m_vArgs.Length >= 2)
                {
                    try
                    {
                        long id = Convert.ToInt64(m_vArgs[1]);
                        var l = ResourcesManager.GetPlayer(id);
                        if(l != null)
                        {
                            l.SetAccountStatus(0);
                        }
                        else
                        {
                            Debugger.WriteLine("Unban failed: id " + id + " not found");
                        }
                    }
                    catch(Exception ex)
                    {
                        Debugger.WriteLine("Unban failed with error: " + ex.ToString()); 
                    }
                }
            }
            else
            {
                SendCommandFailedMessage(level.GetClient());
            }
        }
    }
}
