﻿using Ultrapowa_Clash_Server_GUI.GameFiles;

namespace Ultrapowa_Clash_Server_GUI.Logic
{
    internal class Trap : ConstructionItem
    {
        public Trap(Data data, Level l) : base(data, l)
        {
            AddComponent(new TriggerComponent());
        }

        public override int ClassId
        {
            get { return 4; }
        }

        public TrapData GetTrapData()
        {
            return (TrapData) GetData();
        }
    }
}