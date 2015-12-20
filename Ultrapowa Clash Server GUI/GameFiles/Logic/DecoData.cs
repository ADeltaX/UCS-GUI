﻿using Ultrapowa_Clash_Server_GUI.Core;

namespace Ultrapowa_Clash_Server_GUI.GameFiles
{
    internal class DecoData : Data
    {
        public DecoData(CSVRow row, DataTable dt)
            : base(row, dt)
        {
            LoadData(this, GetType(), row);
        }

        public int BuildCost { get; set; }

        public string BuildResource { get; set; }

        public string ExportName { get; set; }

        public string ExportNameBase { get; set; }

        public string ExportNameBaseNpc { get; set; }

        public string ExportNameBaseWar { get; set; }

        public string ExportNameConstruction { get; set; }

        public int Height { get; set; }

        public string Icon { get; set; }

        public string InfoTID { get; set; }

        public int MaxCount { get; set; }

        public int RequiredExpLevel { get; set; }

        public string SWF { get; set; }

        public string TID { get; set; }

        public int Width { get; set; }

        public int GetBuildCost()
        {
            return BuildCost;
        }

        public ResourceData GetBuildResource()
        {
            return ObjectManager.DataTables.GetResourceByName(BuildResource);
        }

        public int GetSellPrice()
        {
            var calculation = (int) ((BuildCost*(long) 1717986919) >> 32);
            return (calculation >> 2) + (calculation >> 31);
        }
    }
}