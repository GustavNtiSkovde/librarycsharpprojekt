using System.Windows;
using System.Windows.Controls;
using Bibliotekssystem.Database;

namespace Bibliotekssystem.User
{
    public partial class myloans : UserControl
    {
        private DataCalls db = new DataCalls();

        public myloans()
        {
            InitializeComponent();
            UpdateList();
        }

        private void UpdateList()
        {
            var results = db.GetAllActiveLoans();
            LoanDisplayInfo.ItemsSource = results;
        }

        private void Returnbtn(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null && btn.Tag != null)
            {
                int copyId = (int)btn.Tag;
                if (db.ReturnMedia(copyId))
                {
                    MessageBox.Show("Boken har lämnats tillbaka!", "Success");
                    UpdateList();
                }
                else
                {
                    MessageBox.Show("Något gick fel.", "Error");
                }
            }
        }

        // Hoppar tillbaka till söksidan
        private void GoToSearch(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Content = new Bibliotekssystem.User.searchbook();
        }

        // Loggar ut
        private void Logoutbt(object sender, RoutedEventArgs e)
        {
            MainWindow loginWindow = new MainWindow();
            Application.Current.MainWindow.Content = loginWindow.Content;
        }
    }
}