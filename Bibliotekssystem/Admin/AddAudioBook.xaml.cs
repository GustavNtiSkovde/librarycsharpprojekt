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
    /// Interaction logic for AddAudioBook.xaml
    /// </summary>
    public partial class AddAudioBook : UserControl {
        public AddAudioBook() {
            InitializeComponent();
        }
        private void StartSideBtn(object sender, RoutedEventArgs e) {
            Content = new Bibliotekssystem.Admin.AdminMainView();
        }
        private void Logoutbt(object sender, RoutedEventArgs e) {
            LogoutManager.Logout();
        }

        private void EditAudioBookDoneBtn(object sender, RoutedEventArgs e) {
            // empty handler added to match XAML
        }
    }
}