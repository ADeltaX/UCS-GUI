using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Ultrapowa_Clash_Server_GUI
{
    /// <summary>
    /// Logica di interazione per LoadingServerScreen.xaml
    /// </summary>
    public partial class LoadingServerScreen : Window
    {

        public static LoadingServerScreen LSS = new LoadingServerScreen();

        public LoadingServerScreen()
        {
            LSS = this;
            InitializeComponent();
        }
    }
}
