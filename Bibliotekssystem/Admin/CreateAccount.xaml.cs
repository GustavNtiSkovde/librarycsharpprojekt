using System.Windows;
using System.Windows.Controls;
using Bibliotekssystem.Database;

namespace Bibliotekssystem.Admin
{
    public partial class CreateAccount : UserControl
    {
        private DataCalls db = new DataCalls();

        public CreateAccount()
        {
            InitializeComponent();
        }

        // skapa nytt konto
        private void CreateAccountBtn(object sender, RoutedEventArgs e)
        {
            string email = EmailInput.Text;
            string pwd = PasswordInput.Password;
            bool isAdmin = RoleAdmin.IsChecked == true;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(pwd))
            {
                MessageBox.Show("Fyll i både email och lösenord!", "Fel");
                return;
            }

            if (db.CreateUserAccount(email, pwd, isAdmin))
            {
                MessageBox.Show("Konto skapat!", "Success");
                Application.Current.MainWindow.Content = new Bibliotekssystem.Admin.listviewusers();
            }
            else
            {
                MessageBox.Show("Kunde inte skapa konto.", "Error");
            }
        }

        // uppdatera lösenord / roll
        private void EditAccountDoneBtn(object sender, RoutedEventArgs e)
        {
            string email = EmailInput.Text;
            string pwd = PasswordInput.Password;
            bool isAdmin = RoleAdmin.IsChecked == true;

            if (!string.IsNullOrWhiteSpace(pwd))
            {
                db.UpdateUserPassword(email, pwd);
            }

            if (db.UpdateUserRole(email, isAdmin))
            {
                MessageBox.Show("Konto uppdaterat!", "Success");
                Application.Current.MainWindow.Content = new Bibliotekssystem.Admin.listviewusers();
            }
            else
            {
                MessageBox.Show("Kunde inte hitta användaren.", "Error");
            }
        }

        private void StartSideBtn(object sender, RoutedEventArgs e) {
            Content = new Bibliotekssystem.Admin.MainWindowAdmin();
        }
        private void Logoutbt(object sender, RoutedEventArgs e)
        {
            MainWindow loginWindow = new MainWindow();
            Application.Current.MainWindow.Content = loginWindow.Content;
        }
    }
}