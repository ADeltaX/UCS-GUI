using System;
using System.IO;
using Ultrapowa_Clash_Server_GUI.Core;
using Ultrapowa_Clash_Server_GUI.Helpers;
using Ultrapowa_Clash_Server_GUI.Logic;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    //Packet 14102
    internal class ExecuteCommandsMessage : Message
    {
        public ExecuteCommandsMessage(Client client, BinaryReader br) : base(client, br)
        {
        }

        public byte[] NestedCommands { get; private set; }

        public uint NumberOfCommands { get; set; }

        public uint Unknown1 { get; set; }

        //00 00 2B D8 some sort of server tick
        public uint Unknown2 { get; set; }

        public override void Decode()
        {
            using (var br = new BinaryReader(new MemoryStream(GetData())))
            {
                //Console.WriteLine(base.ToHexString());
                Unknown1 = br.ReadUInt32WithEndian();
                Unknown2 = br.ReadUInt32WithEndian();
                NumberOfCommands = br.ReadUInt32WithEndian();

                if (NumberOfCommands > 0)
                {
                    NestedCommands = br.ReadBytes(GetLength() - 12);
                }
            }
        }

        // 01 EB 30 36 some sort of server tick or checksum 
        public override void Process(Level level)
        {
            try
            {
                level.Tick();

                if (NumberOfCommands > 0)
                {
                    using (var br = new BinaryReader(new MemoryStream(NestedCommands)))
                    {
                        for (var i = 0; i < NumberOfCommands; i++)
                        {
                            var obj = CommandFactory.Read(br);
                            if (obj != null)
                            {
                                var player = "";
                                if (level != null)
                                    player += " (" + level.GetPlayerAvatar().GetId() + ", " +
                                              level.GetPlayerAvatar().GetAvatarName() + ")";
                                MainWindow.RemoteWindow.WriteConsoleDebug("\t" + obj.GetType().Name + player, (int)MainWindow.level.DEBUGLOG);
                                ((Command) obj).Execute(level);

                                //Debugger.WriteLine("finished processing of command " + obj.GetType().Name + player);
                            }
                            else
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MainWindow.RemoteWindow.WriteConsoleDebug("Exception occurred during command processing." + ex, (int)MainWindow.level.DEBUGFATAL);
            }
        }
    }
}