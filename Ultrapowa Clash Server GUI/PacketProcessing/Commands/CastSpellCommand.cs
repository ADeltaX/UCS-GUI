using System.IO;
using Ultrapowa_Clash_Server_GUI.GameFiles;
using Ultrapowa_Clash_Server_GUI.Helpers;
using Ultrapowa_Clash_Server_GUI.Logic;

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    //Commande 604 = 0x25C
    internal class CastSpellCommand : Command
    {
        public CastSpellCommand(BinaryReader br)
        {
            X = br.ReadInt32WithEndian();
            Y = br.ReadInt32WithEndian();
            Spell = (SpellData)br.ReadDataReference();
            Unknown1 = br.ReadUInt32WithEndian();
        }

        public SpellData Spell { get; set; }

        public uint Unknown1 { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public override void Execute(Level level)
        {
            var components = level.GetComponentManager().GetComponents(0);
            for (var i = 0; i < components.Count; i++)
            {
                //UCS.Core.Debugger.WriteLine("Spell : " + Spell + "Unknown1 : " + Unknown1 + "X : " + X + "Y : " + Y , null, 4);
                var c = (UnitStorageComponent)components[i];
                if (c.GetUnitTypeIndex(Spell) != -1)
                {
                    var storageCount = c.GetUnitCountByData(Spell);
                    if (storageCount >= 1)
                    {
                        c.RemoveUnits(Spell, 1);
                        break;
                    }
                }
            }
        }
    }
}