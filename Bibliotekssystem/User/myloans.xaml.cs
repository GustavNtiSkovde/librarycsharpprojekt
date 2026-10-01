using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Bibliotekssystem.User
{
    /// <summary>
    /// Interaction logic for myloans.xaml
    /// </summary>
    public partial class myloans : UserControl
    {
        public myloans()
        {
            InitializeComponent();
        }

        private void Minalan(object sender, RoutedEventArgs e)
        {

        }
        private void Logoutbt(object sender, RoutedEventArgs e) {
            LogoutManager.Logout();
        }
        private void Returnbtn(object sender, RoutedEventArgs e) {
            // empty handler added to match XAML
        }
    }
}
