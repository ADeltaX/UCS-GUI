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
    class KickGameOpCommand : GameOpCommand
    {
        private string[] m_vArgs;

        public KickGameOpCommand(string[] args)
        {
            m_vArgs = args;
            SetRequiredAccountPrivileges(1);
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
                        if(ResourcesManager.IsPlayerOnline(l))
                        {
                            var p = new OutOfSyncMessage(l.GetClient());
                            PacketManager.ProcessOutgoingPacket(p);
                        }
                        else
                        {
                            Debugger.WriteLine("Kick failed: id " + id + " not found");
                        }
                    }
                    catch(Exception ex)
                    {
                        Debugger.WriteLine("Kick failed with error: " + ex.ToString()); 
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
