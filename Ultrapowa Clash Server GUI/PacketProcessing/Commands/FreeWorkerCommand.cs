using System.IO;
using Ultrapowa_Clash_Server_GUI.Helpers;
using Ultrapowa_Clash_Server_GUI.Logic;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    //Commande 0x209
    internal class FreeWorkerCommand : Command
    {
        private readonly object m_vCommand;

        private readonly byte m_vIsCommandEmbedded;

        public int m_vTimeLeftSeconds;

        public FreeWorkerCommand(BinaryReader br)
        {
            m_vTimeLeftSeconds = br.ReadInt32WithEndian();
            m_vIsCommandEmbedded = br.ReadByte();
            if (m_vIsCommandEmbedded >= 0x01)
            {
                m_vCommand = CommandFactory.Read(br);
            }
        }

        public override void Execute(Level level)
        {
            if (level.WorkerManager.GetFreeWorkers() == 0)
            {
                level.WorkerManager.FinishTaskOfOneWorker();
                if (m_vIsCommandEmbedded >= 1)
                    ((Command) m_vCommand).Execute(level);
            }
        }
    }
}