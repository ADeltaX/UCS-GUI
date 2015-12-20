using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace Ultrapowa_Clash_Server_GUI
{
    /// <summary>
    /// Logica di interazione per Popup.xaml
    /// </summary>
    public partial class Popup : Window
    {

        bool IsRequiredSecPage = false;

        public enum cause
        {
            BAN = 0,
            BANIP = 1,
            TEMPBAN = 2,
            TEMPBANIP = 3,
            UNBAN = 4,
            UNBANIP = 5,
            KICK = 6
        }

        public Popup(int Slc_cause = -1)
        {
            Opacity = 0;
            InitializeComponent();

            LB_Main.Content = Slc_cause == (int)cause.BAN ? "Select a player to ban" : Slc_cause == (int)cause.BANIP ?
                "Select a player to ban ip" : Slc_cause == (int)cause.TEMPBAN ? "Select a player to ban temporarily" :
                Slc_cause == (int)cause.TEMPBANIP ? "Select a player to ban ip" : Slc_cause == (int)cause.UNBAN ?
                "Select a player to unban" : Slc_cause == (int)cause.UNBANIP ? "Select a player to unban ip" :
                Slc_cause == (int)cause.KICK ? "Select a player to kick" : "Error";

            if (Slc_cause == (int)cause.UNBAN || Slc_cause == (int)cause.UNBANIP || Slc_cause == (int)cause.KICK) { btn_ok.Content = "OK"; IsRequiredSecPage = false; }
            else { btn_ok.Content = "Continue"; IsRequiredSecPage = true; }

        }

        private void btn_cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            OpInW();

            int DeltaVariation = 50;

            AnimationLib.MoveToTarget(btn_cancel, DeltaVariation, 0.25);
            AnimationLib.MoveToTarget(btn_ok, DeltaVariation, 0.25, 50);
            AnimationLib.MoveToTarget(CB_Player, DeltaVariation, 0.25, 100);
            AnimationLib.MoveToTarget(LB_Main, DeltaVariation, 0.25, 150);

            AnimationLib.MoveWindowToTarget(this, DeltaVariation, Top, 0.25);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            OpOutW(sender, e);
        }

        private void OpInW()
        {
            var OpIn = new DoubleAnimation(1, (Duration)TimeSpan.FromSeconds(0.125));
            this.BeginAnimation(Window.OpacityProperty, OpIn);
        }

        private void OpOutW(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Closing -= Window_Closing;
            e.Cancel = true;
            var OpOut = new DoubleAnimation(0, TimeSpan.FromSeconds(0.125));
            OpOut.Completed += (s, _) => { this.Close(); MainWindow.IsFocusOk = true; };
            BeginAnimation(Window.OpacityProperty, OpOut);
        }


    }
}
