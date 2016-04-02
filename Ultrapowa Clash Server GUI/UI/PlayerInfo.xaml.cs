using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using UCS.Core;
using UCS.Helpers;

namespace UCS.UI
{
    /// <summary>
    /// Logica di interazione per Popup.xaml
    /// </summary>
    public partial class PlayerInfo : Window
    {

        int DeltaVariation = 100;

        public PlayerInfo()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            OpInW();

            MainWindow.RemoteWindow.UpdateTheListPlayers();
            CB_Player.ItemsSource = MainWindow.RemoteWindow.Players;

            AnimationLib.MoveToTargetY(btn_ok, DeltaVariation, 0.25, 50);
            AnimationLib.MoveToTargetY(CB_Player, DeltaVariation, 0.25, 100);
            AnimationLib.MoveToTargetY(LB_Main, DeltaVariation, 0.25, 150);
            AnimationLib.MoveToTargetY(img_Commands, DeltaVariation, 0.25, 200);

            AnimationLib.MoveWindowToTargetY(this, DeltaVariation, Top, 0.25);

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            OpOutW(sender, e);
        }

        private void OpInW()
        {
            var OpIn = new DoubleAnimation(1, TimeSpan.FromSeconds(0.125));
            BeginAnimation(OpacityProperty, OpIn);
        }

        private void OpOutW(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Closing -= Window_Closing;
            e.Cancel = true;
            var OpOut = new DoubleAnimation(0, TimeSpan.FromSeconds(0.125));
            OpOut.Completed += (s, _) => { this.Close(); MainWindow.IsFocusOk = true; };
            BeginAnimation(OpacityProperty, OpOut);
        }

        private void btn_ok_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
