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

namespace Ultrapowa_Clash_Server_GUI.PacketProcessing
{
    //Commande 604 = 0x25C
    class CastSpellCommand : Command
    {
        public CastSpellCommand(BinaryReader br)
        {
            X = br.ReadInt32WithEndian();
            Y = br.ReadInt32WithEndian();
            Spell = (SpellData)br.ReadDataReference();
            Unknown1 = br.ReadUInt32WithEndian();
        }

        public int X { get; set; }
        public int Y { get; set; }
        public SpellData Spell { get; set; }
        public uint Unknown1 { get; set; }

        public override void Execute(Level level)
        {
            List<Component> components = level.GetComponentManager().GetComponents(0);
            for (int i = 0; i < components.Count; i++)
            {
                UnitStorageComponent c = (UnitStorageComponent)components[i];
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
