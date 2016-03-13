using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using UCS.Sys;

namespace UCS.UI.UC
{
    /// <summary>
    /// Logica di interazione per Menu.xaml
    /// </summary>
    public partial class Menu : UserControl
    {
        public RoutedEvent ClickEvent;
        public RoutedEvent OverEvent;
        public RoutedEvent RetireEvent;

        public Menu()
        {
            InitializeComponent();
            ClickEvent = ButtonBase.ClickEvent.AddOwner(typeof(Menu));
            OverEvent = MouseEnterEvent.AddOwner(typeof(Menu));
            RetireEvent = MouseLeaveEvent.AddOwner(typeof(Menu));

            //WireAllControls();
        }

        private void Menu_Retire(object sender, RoutedEventArgs e)
        {
            
        }

        private void WireAllControls()
        {
            
            MouseLeave += CTL_MouseLeave;
            MouseEnter += CTL_MouseEnter;
            Icon.MouseEnter += CTL_MouseEnter;
            Name.MouseEnter += CTL_MouseEnter;
            Icon.MouseLeave += CTL_MouseLeave;
            Name.MouseLeave += CTL_MouseLeave;
        }

        private void CTL_MouseEnter(object sender, MouseEventArgs e)
        {
            var m_Color = new SolidColorBrush(Color.FromRgb(0x00, 0x4c, 0x65));
            Background = m_Color;
        }

        private void CTL_MouseLeave(object sender, MouseEventArgs e)
        {
            Background = Brushes.Transparent;
        }

        #region Events

        public event RoutedEventHandler Retire
        {
            add { AddHandler(OverEvent, value); }
            remove { RemoveHandler(OverEvent, value); }
        }

        public event RoutedEventHandler Over
        {
            add { AddHandler(OverEvent, value); }
            remove { RemoveHandler(OverEvent, value); }
        }

        public event RoutedEventHandler Click
        {

            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            CaptureMouse();
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            if (IsMouseCaptured)
            {
                ReleaseMouseCapture();
                if (IsMouseOver)
                    RaiseEvent(new RoutedEventArgs(ClickEvent, this));
            }
        }

        #endregion

        public string NameLabel
        {
            get
            {
                return Name.Content.ToString();
            }
            set
            {
                Name.Content = value;
            }
        }

        public ImageSource ImageLink
        {
            get
            {
                return Icon.Source;
            }
            set
            {
                Icon.Source = value;
            }
        }
    }
}
