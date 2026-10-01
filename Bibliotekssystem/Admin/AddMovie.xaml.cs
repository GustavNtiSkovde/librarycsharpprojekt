using System;
using System.Collections.Generic;
using SystemText = System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Bibliotekssystem.Admin
{
    /// <summary>
    /// Interaction logic for AddMovie.xaml
    /// </summary>
    public partial class AddMovie : UserControl
    {
        public AddMovie()
        {
            InitializeComponent();
        }
        private void StartSideBtn(object sender, RoutedEventArgs e)
        {
            Content = new Bibliotekssystem.Admin.MainWindowAdmin();
        }
        private void Logoutbt(object sender, RoutedEventArgs e)
        {
            MainWindow loginWindow = new MainWindow();
            Application.Current.MainWindow.Content = loginWindow.Content;
        }

        private void EditMovieDoneBtn(object sender, RoutedEventArgs e)
        {
            // empty handler added to match XAML
        }
    }
}