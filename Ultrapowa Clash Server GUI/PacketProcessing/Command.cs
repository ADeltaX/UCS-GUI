using System.Collections.Generic;
using Ultrapowa_Clash_Server_GUI.Logic;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    internal class Command
    {
        public virtual byte[] Encode()
        {
            return new List<byte>().ToArray();
        }

        public virtual void Execute(Level level)
        {
        }
    }
}