using System.Windows;
using System.Windows.Controls;

namespace Bibliotekssystem.Admin
{
    public partial class MainWindowAdmin : UserControl
    {
        public MainWindowAdmin()
        {
            InitializeComponent();
        }

        // gå till medialistan (med stor L)
        private void StartSideBtn_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Content = new Bibliotekssystem.Admin.Listviewmedia();
        }

        // gå till användarlistan
        private void AccountsBtn_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Content = new Bibliotekssystem.Admin.listviewusers();
        }

        // logga ut
        private void Logoutbt_Click(object sender, RoutedEventArgs e)
        {
            MainWindow loginWindow = new MainWindow();
            Application.Current.MainWindow.Content = loginWindow.Content;
        }
    }
}