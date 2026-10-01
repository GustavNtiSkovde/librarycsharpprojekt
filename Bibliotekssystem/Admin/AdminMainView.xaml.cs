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

namespace Bibliotekssystem.Admin {
    /// <summary>
    /// Interaction logic for AdminMainView.xaml
    /// </summary>
    public partial class AdminMainView : UserControl {
        public AdminMainView() {
            InitializeComponent();
        }

        private void Logoutbt(object sender, RoutedEventArgs e) {
            LogoutManager.Logout();
        }

        private void ViewMediaListBtn(object sender, RoutedEventArgs e) {
            Content = new Bibliotekssystem.Admin.listviewmedia();
        }

        private void ViewUserListBtn(object sender, RoutedEventArgs e) {
            Content = new Bibliotekssystem.Admin.listviewusers();
        }
    }
}