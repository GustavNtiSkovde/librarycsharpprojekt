using Bibliotekssystem.Database;
using System.Text;
using System;
using System.Windows;
using Bibliotekssystem.Database;

namespace Bibliotekssystem
{
    public partial class MainWindow : Window
    {
        private DataCalls db;

        public MainWindow()
        {
            InitializeComponent();

        }

        private void login_Click(object sender, RoutedEventArgs e)
        {
            // Replace txtUsername and txtPassword with the x:Name attributes from your MainWindow.xaml
            string username = Email.Text;
            string password = Password.Password; // Use .Password if using a PasswordBox control

            DataCalls dataCalls = new DataCalls();
            bool isLoggedIn = dataCalls.VerifyUserLogin(username, password);

            if (isLoggedIn) {
                MessageBox.Show("Login Successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else {
                MessageBox.Show("Invalid username or password.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
