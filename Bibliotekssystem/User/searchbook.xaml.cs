using System.Windows;
using System.Windows.Controls;
using Bibliotekssystem.Database;

namespace Bibliotekssystem.User
{
    public partial class searchbook : UserControl
    {
        // db connection
        private DataCalls db = new DataCalls();

        public searchbook()
        {
            InitializeComponent();
            UpdateList(""); // load all on start
        }

        // text search
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateList(SearchBox.Text);
        }

        // checkbox click
        private void Filter_Click(object sender, RoutedEventArgs e)
        {
            UpdateList(SearchBox.Text);
        }

        // fetch and filter
        private void UpdateList(string term)
        {
            var results = db.SearchMedia(term);
            MediaDisplayInfo.ItemsSource = results;
        }

        // borrow btn click
        private void BorrowBtn_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;

            if (btn != null && btn.Tag != null)
            {
                int mediaId = (int)btn.Tag;
                int currentUserId = 1;

                var loan = db.BorrowMedia(currentUserId, mediaId);

                if (loan != null)
                {
                    MessageBox.Show("Du har lånat boken!", "Success");
                }
                else
                {
                    MessageBox.Show("No copies available", "Error");
                }
            }
        }

        // jump to my loans safely
        private void Minalan(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Content = new Bibliotekssystem.User.myloans();
        }

        // log out safely
        private void Logoutbtn(object sender, RoutedEventArgs e)
        {
            MainWindow loginWindow = new MainWindow();
            Application.Current.MainWindow.Content = loginWindow.Content;
        }
    }
}