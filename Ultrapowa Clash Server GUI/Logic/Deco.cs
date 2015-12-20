﻿using Newtonsoft.Json.Linq;
using Ultrapowa_Clash_Server_GUI.GameFiles;

namespace Ultrapowa_Clash_Server_GUI.Logic
{
    internal class Deco : GameObject
    {
        private Level m_vLevel;

        public Deco(Data data, Level l) : base(data, l)
        {
            m_vLevel = l;
        }

        public override int ClassId
        {
            get { return 6; }
        }

        public DecoData GetDecoData()
        {
            return (DecoData) GetData();
        }

        public new void Load(JObject jsonObject)
        {
            base.Load(jsonObject);
        }

        public new JObject Save(JObject jsonObject)
        {
            base.Save(jsonObject);
            return jsonObject;
        }
    }
}