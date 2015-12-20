using System;
using Ultrapowa_Clash_Server_GUI.Core;
using Ultrapowa_Clash_Server_GUI.Logic;
using Ultrapowa_Clash_Server_GUI.Network;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    internal class VisitGameOpCommand : GameOpCommand
    {
        private readonly string[] m_vArgs;

        public VisitGameOpCommand(string[] args)
        {
            m_vArgs = args;
            SetRequiredAccountPrivileges(2);
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
                        if (l != null)
                        {
                            l.Tick();
                            var p = new VisitedHomeDataMessage(level.GetClient(), l, level);
                            PacketManager.ProcessOutgoingPacket(p);
                        }
                        else
                        {
                            Debugger.WriteLine("Visit failed: id " + id + " not found");
                        }
                    }
                    catch (Exception ex)
                    {
                        Debugger.WriteLine("Visit failed with error: " + ex);
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