using Bibliotekssystem.Database;
using System.Text;
using System;
using System.Windows;
using Bibliotekssystem.Database;
using Org.BouncyCastle.Asn1.X509;

namespace Bibliotekssystem {
    public partial class MainWindow : Window {
        private DataCalls db;

        public MainWindow() {
            InitializeComponent();
            db = new DataCalls();
        }

        private void login_Click(object sender, RoutedEventArgs e) {
            string username = Email.Text;
            string password = Password.Password; // Use .Password if using a PasswordBox control

            try {
                var user = db.GetUserByEmailAndPassword(username, password);

                if (user != null) {
                    var role = user.Role ?? string.Empty;

                    if (role.Equals("Admin", StringComparison.OrdinalIgnoreCase)) {
                        Content = new Bibliotekssystem.Admin.AdminMainView();
                    }
                    else {
                        Content = new Bibliotekssystem.User.searchbook();
                    }
                }
                else {
                    MessageBox.Show("Invalid username or password.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex) {
                // Log for diagnostics and present a clear user-facing error
                System.Diagnostics.Debug.WriteLine($"Login failed: {ex}");
                MessageBox.Show(
                    "Unable to reach the database. Check MySQL server, network and connection string.\n\nDetails: " + ex.Message,
                    "Database Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }
    }
}