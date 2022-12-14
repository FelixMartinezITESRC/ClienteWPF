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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Timers.Views
{
    /// <summary>
    /// Lógica de interacción para AeropuertoView.xaml
    /// </summary>
    public partial class AeropuertoView : Window
    {
        public AeropuertoView()
        {
            InitializeComponent();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            txtReloj.Text = DateTime.Now.ToString("hh:mm");

        }

        DispatcherTimer timer = new DispatcherTimer();

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            wndPrincipal.WindowState=WindowState.Minimized;
        }

        private void tggNorMax_Checked(object sender, RoutedEventArgs e)
        {
            wndPrincipal.WindowState= WindowState.Maximized;
        }

        private void tggNorMax_Unchecked(object sender, RoutedEventArgs e)
        {
            wndPrincipal.WindowState = WindowState.Normal;
        }

        private void wndPrincipal_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
