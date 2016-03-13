using Ultrapowa_Clash_Server_GUI.Core;
using Ultrapowa_Clash_Server_GUI.Network;
using Ultrapowa_Clash_Server_GUI.GameFiles;
using Ultrapowa_Clash_Server_GUI.Logic;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    internal class MinResourcesGameOpCommand : GameOpCommand
    {
        private string[] m_vArgs;

        public MinResourcesGameOpCommand(string[] args)
        {
            m_vArgs = args;
            SetRequiredAccountPrivileges(0);
        }

        public override void Execute(Level level)
        {
            if (level.GetAccountPrivileges() >= GetRequiredAccountPrivileges())
            {
                var dt = ObjectManager.DataTables.GetTable(2);
                for (var i = 0; i < dt.GetItemCount(); i++)
                {
                    var rd = (ResourceData)dt.GetItemAt(i);
                    if (!rd.PremiumCurrency)
                    {
                        var ca = level.GetPlayerAvatar();
                        ca.SetResourceCount(rd, ca.GetResourceCap(rd));
                    }
                    PacketManager.ProcessOutgoingPacket(new OwnHomeDataMessage(level.GetClient(), level));
                }
            }
            else
            {
                SendCommandFailedMessage(level.GetClient());
            }
        }
    }
}