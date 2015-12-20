namespace Ultrapowa_Clash_Server_GUI.GameFiles
{
    internal class WarData : Data
    {
        public WarData(CSVRow row, DataTable dt)
            : base(row, dt)
        {
            LoadData(this, GetType(), row);
        }

        public int BonusPercentDraw { get; set; }

        public int BonusPercentLose { get; set; }

        public int BonusPercentWin { get; set; }

        public bool DisableProduction { get; set; }

        public int PreparationMinutes { get; set; }

        public int TeamSize { get; set; }

        public int WarMinutes { get; set; }
    }
}