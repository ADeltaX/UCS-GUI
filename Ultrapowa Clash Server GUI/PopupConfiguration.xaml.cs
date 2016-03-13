using System;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Ultrapowa_Clash_Server_GUI
{
    /// <summary>
    /// Logica di interazione per GeneralPopup.xaml
    /// </summary>
    public partial class PopupConfiguration : Window
    {
        public PopupConfiguration()
        {
            Opacity = 0;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadConfig();
            OpInW();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            OpOutW(sender, e);
        }

        private void OpInW()
        {
            var OpIn = new DoubleAnimation(1, TimeSpan.FromSeconds(0.25));
            BeginAnimation(OpacityProperty, OpIn);
        }

        private void OpOutW(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Closing -= Window_Closing;
            e.Cancel = true;
            var OpOut = new DoubleAnimation(0, TimeSpan.FromSeconds(0.25));
            OpOut.Completed += (s, _) => { this.Close(); MainWindow.IsFocusOk = true; };
            BeginAnimation(OpacityProperty, OpOut);
        }

        private void BTN_SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            if (BlockSaves[0] || BlockSaves[1] || BlockSaves[2] || BlockSaves[3] || BlockSaves[4]
                 || BlockSaves[5] || BlockSaves[6] || BlockSaves[7] || BlockSaves[8] || BlockSaves[9])
                MessageBox.Show("There are some invalid values, fix it before saving.\nHelp: Starting items should not exceed the maximum value of 999999999 and the minum value of 0\nPort max value: 65535 and should be same of debug port\nStarting level max value: 9");
            else SaveChanges();
        }

        private void BTN_ReloadConfig_Click(object sender, RoutedEventArgs e)
        {
            LoadConfig();
        }

        private void SaveChanges()
        {
            MessageBox.Show("OK!");
            this.Close();
        }

        private void LoadConfig()
        {
            NeedToCheck = false;
            TB_DarkElixir.Text = ConfigurationManager.AppSettings["startingDarkElixir"];
            TB_Elixir.Text = ConfigurationManager.AppSettings["startingElixir"];
            TB_Gold.Text = ConfigurationManager.AppSettings["startingGold"];
            TB_Gems.Text = ConfigurationManager.AppSettings["startingGems"];
            TB_StartingLevel.Text = ConfigurationManager.AppSettings["startingLevel"];
            TB_Trophies.Text = ConfigurationManager.AppSettings["startingTrophies"];
            TB_Experience.Text = ConfigurationManager.AppSettings["startingExperience"];
            TB_Shield.Text = ConfigurationManager.AppSettings["startingShieldTime"];
            TB_ClientVer.Text = ConfigurationManager.AppSettings["clientVersion"];
            TB_PatchServer.Text = ConfigurationManager.AppSettings["patchingServer"];
            TB_Maintenance.Text = ConfigurationManager.AppSettings["maintenanceTimeleft"];
            TB_LogLevel.Text = ConfigurationManager.AppSettings["loggingLevel"];
            TB_Outdated.Text = ConfigurationManager.AppSettings["oldClientVersion"];
            TB_DebugPort.Text = ConfigurationManager.AppSettings["debugPort"];
            TB_Port.Text = ConfigurationManager.AppSettings["serverPort"];
            TB_ThreadCount.Text = ConfigurationManager.AppSettings["saveThreadCount"];

            var CN = ConfigurationManager.AppSettings["databaseConnectionName"];
            if (CN.ToLower() == "sqliteentities") { CN_T.IsSelected = true; CN_F.IsSelected = false; }
            else { CN_F.IsSelected = true; CN_T.IsSelected = false; }

            var CP = Convert.ToString(Convert.ToBoolean(ConfigurationManager.AppSettings["useCustomPatch"]));
            if (CP.ToLower() == "true") { CP_T.IsSelected = true; CP_F.IsSelected = false; }
            else { CP_F.IsSelected = true; CP_T.IsSelected = false; }

            var AM = Convert.ToString(Convert.ToBoolean(ConfigurationManager.AppSettings["apiManager"]));
            if (AM.ToLower() == "true") { AM_T.IsSelected = true; AM_F.IsSelected = false; }
            else { AM_F.IsSelected = true; AM_F.IsSelected = false; }

            var ED = Convert.ToString(Convert.ToBoolean(ConfigurationManager.AppSettings["debugMode"]));
            if (ED.ToLower() == "true") { ED_T.IsSelected = true; ED_F.IsSelected = false; }
            else { ED_F.IsSelected = true; ED_T.IsSelected = false; }

            var EM = Convert.ToString(Convert.ToBoolean(ConfigurationManager.AppSettings["maintenanceMode"]));
            if (EM.ToLower() == "true") { EM_T.IsSelected = true; EM_F.IsSelected = false; }
            else { EM_F.IsSelected = true; EM_T.IsSelected = false; }

            var PH = ConfigurationManager.AppSettings["expertPve"];
            if (PH.ToLower() == "true") { PH_T.IsSelected = true; PH_F.IsSelected = false; }
            else { PH_F.IsSelected = true; PH_T.IsSelected = false; }

            NeedToCheck = true;
        }

        public bool BlockSaveProcess1 = false, BlockSaveProcess2 = false, BlockSaveProcess3 = false, BlockSaveProcess4 = false,
            BlockSaveProcess5 = false, BlockSaveProcess6 = false, BlockSaveProcess7 = false, BlockSaveProcess8 = false,
            BlockSaveProcess9 = false, BlockSaveProcess10 = false;

        public bool[] BlockSaves = new bool[10];

        public bool NeedToCheck = false;
        

        private bool CheckValues(string X, int maxmax = 999999999, int minmin = 0)
        {
            int WOW;
            try
            {
                WOW = Convert.ToInt32(X);
            }
            catch (Exception)
            {
                return false;
            }
            if (WOW > maxmax || WOW < minmin) return false;

            return true;
        }

        SolidColorBrush GOOD = new SolidColorBrush(Color.FromArgb(0xA5, 0x00, 0x69, 0x7C)); //0
        SolidColorBrush ERR = new SolidColorBrush(Color.FromArgb(0xA5, 0xB4, 0x2A, 0x0C)); //2


        #region EVENTS
        private void TB_Gems_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool IsOk = CheckValues(TB_Gems.Text);
            if (IsOk)
            {
                TB_Gems.Background = GOOD;
                BlockSaves[0] = false;
            }
            else
            {
                TB_Gems.Background = ERR;
                BlockSaves[0] = true;
            }
        }

        private void TB_Port_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (NeedToCheck)
            {
                bool IsOk = CheckValues(TB_Port.Text, 65535);
                if (IsOk)
                {
                    TB_Port.Background = GOOD;
                    BlockSaves[8] = false;

                    int Con1 = 0, Con2 = 0;
                    try
                    {
                        Con1 = Convert.ToInt32(TB_DebugPort.Text);
                    }
                    catch (Exception) { }
                    try
                    {
                        Con2 = Convert.ToInt32(TB_Port.Text);
                    }
                    catch (Exception) { }

                    if (Con1 == Con2)
                    {
                        TB_Port.Background = ERR;
                        TB_DebugPort.Background = ERR;
                        BlockSaves[8] = true;
                        BlockSaves[9] = true;
                    }
                    else
                    {
                        bool IsOk1 = CheckValues(TB_DebugPort.Text, 65535);
                        if (IsOk1)
                        {
                            TB_DebugPort.Background = GOOD;
                            BlockSaves[9] = false;
                        }
                    }
                }
                else
                {
                    TB_Port.Background = ERR;
                    BlockSaves[8] = true;
                }
            }
        }

        private void TB_DebugPort_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (NeedToCheck)
            {
                bool IsOk = CheckValues(TB_DebugPort.Text, 65535);
                if (IsOk)
                {
                    TB_DebugPort.Background = GOOD;
                    BlockSaves[9] = false;
                    int Con1=0, Con2=0;
                    try
                    {
                        Con1 = Convert.ToInt32(TB_DebugPort.Text);
                    }
                    catch (Exception) { }
                    try
                    {
                        Con2 = Convert.ToInt32(TB_Port.Text);
                    }
                    catch (Exception) { }

                    if (Con1 == Con2)
                    {
                        TB_Port.Background = ERR;
                        TB_DebugPort.Background = ERR;
                        BlockSaves[8] = true;
                        BlockSaves[9] = true;
                    }
                    else
                    {
                        bool IsOk1 = CheckValues(TB_Port.Text, 65535);
                        if (IsOk1)
                        {
                            TB_Port.Background = GOOD;
                            BlockSaves[8] = false;
                        }
                    }
                }
                else
                {
                    TB_DebugPort.Background = ERR;
                    BlockSaves[9] = true;
                }
            }
        }

        private void TB_Gold_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool IsOk = CheckValues(TB_Gold.Text);
            if (IsOk)
            {
                TB_Gold.Background = GOOD;
                BlockSaves[1] = false;
            }
            else
            {
                TB_Gold.Background = ERR;
                BlockSaves[1] = true;
            }
        }
        private void TB_Elixir_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool IsOk = CheckValues(TB_Elixir.Text);
            if (IsOk)
            {
                TB_Elixir.Background = GOOD;
                BlockSaves[2] = false;
            }
            else
            {
                TB_Elixir.Background = ERR;
                BlockSaves[2] = true;
            }
        }
        private void TB_Trophies_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool IsOk = CheckValues(TB_Trophies.Text,9999,0);
            if (IsOk)
            {
                TB_Trophies.Background = GOOD;
                BlockSaves[4] = false;
            }
            else
            {
                TB_Trophies.Background = ERR;
                BlockSaves[4] = true;
            }
        }

        private void TB_Experience_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool IsOk = CheckValues(TB_Experience.Text,100);
            if (IsOk)
            {
                TB_Experience.Background = GOOD;
                BlockSaves[5] = false;
            }
            else
            {
                TB_Experience.Background = ERR;
                BlockSaves[5] = true;
            }
        }

        private void TB_Shield_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool IsOk = CheckValues(TB_Shield.Text, 2147483647);
            if (IsOk)
            {
                TB_Shield.Background = GOOD;
                BlockSaves[6] = false;
            }
            else
            {
                TB_Shield.Background = ERR;
                BlockSaves[6] = true;
            }
        }

        private void TB_StartingLevel_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool IsOk = CheckValues(TB_StartingLevel.Text,9);
            if (IsOk)
            {
                TB_StartingLevel.Background = GOOD;
                BlockSaves[7] = false;
            }
            else
            {
                TB_StartingLevel.Background = ERR;
                BlockSaves[7] = true;
            }
        }

        private void TB_DarkElixir_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool IsOk = CheckValues(TB_DarkElixir.Text);
            if (IsOk)
            {
                TB_DarkElixir.Background = GOOD;
                BlockSaves[3] = false;
            }
            else
            {
                TB_DarkElixir.Background = ERR;
                BlockSaves[3] = true;
            }
        }
        #endregion
    }
}
