using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.IO;
using System.Reflection;

namespace Ultrapowa_Clash_Server_GUI.GameFiles
{
    class ExperienceLevelData : Data
    {

        public ExperienceLevelData(CSVRow row, DataTable dt)
            : base(row, dt)
        {
            LoadData(this, this.GetType(), row);
        }

        public int ExpPoints { get; set; }
    }
}
