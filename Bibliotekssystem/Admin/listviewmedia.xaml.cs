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
    /// Interaction logic for listviewmedia.xaml
    /// </summary>
    public partial class listviewmedia : UserControl {
        public listviewmedia() {
            InitializeComponent();
        }
        private void StartSideBtn(object sender, RoutedEventArgs e) {
            Content = new Bibliotekssystem.Admin.AdminMainView();
        }
        private void Logoutbt(object sender, RoutedEventArgs e) {
            LogoutManager.Logout();
        }

        private void CreateBookBtn(object sender, RoutedEventArgs e) {
            Content = new Bibliotekssystem.Admin.AddBook();
        }

        private void CreateAudioBookBtn(object sender, RoutedEventArgs e) {
            Content = new Bibliotekssystem.Admin.AddAudioBook();
        }

        private void CreateMovieBtn(object sender, RoutedEventArgs e) {
            Content = new Bibliotekssystem.Admin.AddMovie();
        }

        private void DeleteMediaBtn(object sender, RoutedEventArgs e) {
            
        }
    }
}