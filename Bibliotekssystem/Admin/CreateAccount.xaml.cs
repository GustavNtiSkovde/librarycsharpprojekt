using System.Windows;
using System.Windows.Controls;
using Bibliotekssystem.Database;

namespace Bibliotekssystem.Admin
{
    public partial class EditAccount : UserControl
    {
        private DataCalls db = new DataCalls();

        public EditAccount()
        {
            InitializeComponent();
        }

        // update user and go back
        private void EditAccountDoneBtn(object sender, RoutedEventArgs e)
        {
            string email = EmailInput.Text;
            bool isAdmin = RoleAdmin.IsChecked == true;

            if (db.UpdateUserRole(email, isAdmin))
            {
                MessageBox.Show("Konto uppdaterat!", "Success");
                Application.Current.MainWindow.Content = new Bibliotekssystem.Admin.Listviewmedia();
            }
            else
            {
                MessageBox.Show("Kunde inte hitta en användare med den emailen.", "Error");
            }
        }

        // logout
        private void Logoutbt(object sender, RoutedEventArgs e)
        {
            MainWindow loginWindow = new MainWindow();
            Application.Current.MainWindow.Content = loginWindow.Content;
        }
    }
}