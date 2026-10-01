using System.Windows;
using System.Windows.Controls;
using Bibliotekssystem.Database;

namespace Bibliotekssystem.Admin
{
    public partial class listviewusers : UserControl
    {
        private DataCalls db = new DataCalls();

        public listviewusers()
        {
            InitializeComponent();
            UpdateList("");
        }

        // sök när man skriver
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateList(SearchBox.Text);
        }

        // hämta användare från databasen
        private void UpdateList(string term)
        {
            var results = db.SearchUsers(term);
            UserDisplayInfo.ItemsSource = results;
        }

        // ta bort konto
        private void DeleteAccountBtn(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null && btn.Tag != null)
            {
                int userId = (int)btn.Tag;
                MessageBoxResult result = MessageBox.Show($"Är du säker på att du vill ta bort användare ID {userId}?", "Confirm", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    if (db.DeleteUser(userId))
                    {
                        MessageBox.Show("Användare borttagen!", "Success");
                    }
                    else
                    {
                        MessageBox.Show("Kunde inte ta bort användaren.", "Error");
                    }
                    UpdateList(SearchBox.Text);
                }
            }
        }

        // gå till skapa/redigera konto
        private void CreateAccountBtn(object sender, RoutedEventArgs e)
        {
            Content = new Bibliotekssystem.Admin.CreateAccount();
        }

        // logga ut
        private void Logoutbt(object sender, RoutedEventArgs e)
        {
            MainWindow loginWindow = new MainWindow();
            Application.Current.MainWindow.Content = loginWindow.Content;
        }
        private void StartSideBtn(object sender, RoutedEventArgs e) {
            Content = new Bibliotekssystem.Admin.MainWindowAdmin();
        }
    }
}