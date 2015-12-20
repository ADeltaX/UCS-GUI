using System;
using Ultrapowa_Clash_Server_GUI.Core;
using Ultrapowa_Clash_Server_GUI.Logic;
using Ultrapowa_Clash_Server_GUI.Network;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    internal class KickGameOpCommand : GameOpCommand
    {
        private readonly string[] m_vArgs;

        public KickGameOpCommand(string[] args)
        {
            m_vArgs = args;
            SetRequiredAccountPrivileges(1);
        }

        public override void Execute(Level level)
        {
            if (level.GetAccountPrivileges() >= GetRequiredAccountPrivileges())
            {
                if (m_vArgs.Length >= 2)
                {
                    try
                    {
                        var id = Convert.ToInt64(m_vArgs[1]);
                        var l = ResourcesManager.GetPlayer(id);
                        if (ResourcesManager.IsPlayerOnline(l))
                        {
                            ResourcesManager.LogPlayerOut(l);
                            var p = new OutOfSyncMessage(l.GetClient());
                            PacketManager.ProcessOutgoingPacket(p);
                        }
                        else
                        {
                            Debugger.WriteLine("Kick failed: id " + id + " not found");
                        }
                    }
                    catch (Exception ex)
                    {
                        Debugger.WriteLine("Kick failed with error: " + ex);
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