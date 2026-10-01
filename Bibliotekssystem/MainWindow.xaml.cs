using System.Windows;
using Bibliotekssystem.Database;

namespace Bibliotekssystem
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void login_Click(object sender, RoutedEventArgs e)
        {
            // get input
            string email = Email.Text;
            string password = Password.Password;

            // check db
            DataCalls db = new DataCalls();
            string userRole = db.VerifyUserLogin(email, password);

            // send to correct page based on role
            if (userRole == "Admin")
            {
                Content = new Bibliotekssystem.Admin.Listviewmedia();
            }
            else if (userRole == "Borrower" || userRole == "User")
            {
                Content = new Bibliotekssystem.User.searchbook();
            }
            else
            {
                MessageBox.Show("Invalid username or password.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    }