using System;
using Ultrapowa_Clash_Server_GUI.Core;
using Ultrapowa_Clash_Server_GUI.Logic;
using Ultrapowa_Clash_Server_GUI.Network;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    internal class MaxResourcesGameOpCommand : GameOpCommand
    {
        private string[] m_vArgs;

        public MaxResourcesGameOpCommand(string[] args)
        {
            m_vArgs = args;
            SetRequiredAccountPrivileges(0);
        }

        public override void Execute(Level level)
        {
            if (level.GetAccountPrivileges() >= GetRequiredAccountPrivileges())
            {
                level.GetPlayerAvatar()
                    .SetResourceCount(ObjectManager.DataTables.GetResourceByName("Gold"), Convert.ToInt32("800000000"));
                level.GetPlayerAvatar()
                    .SetResourceCount(ObjectManager.DataTables.GetResourceByName("Elixir"), Convert.ToInt32("800000000"));
                level.GetPlayerAvatar()
                    .SetResourceCount(ObjectManager.DataTables.GetResourceByName("DarkElixir"),
                        Convert.ToInt32("800000000"));
                level.GetPlayerAvatar().SetDiamonds(775000);
                var alliance = ObjectManager.GetAlliance(level.GetPlayerAvatar().GetAllianceId());

                //player.GetPlayerAvatar().Clean();
                PacketManager.ProcessOutgoingPacket(new OwnHomeDataMessage(level.GetClient(), level));
            }
            else
            {
                SendCommandFailedMessage(level.GetClient());
            }
        }
    }
}