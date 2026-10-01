using System;
using System.Windows;
using System.Windows.Controls;
using Bibliotekssystem.Database;

namespace Bibliotekssystem.User
{
    public partial class loansite : UserControl
    {
        // db connection
        private DataCalls db = new DataCalls();

        public loansite()
        {
            InitializeComponent();
        }

        // borrow btn click
        private void Lana(object sender, RoutedEventArgs e)
        {
            // check if input is a valid number
            if (int.TryParse(MediaIdInput.Text, out int mediaId))
            {
                // totally real user id
                int currentUserId = 1;

                // try to borrow in db
                var loan = db.BorrowMedia(currentUserId, mediaId);

                if (loan != null)
                {
                    MessageBox.Show("Du har lånat boken!", "Success");
                    MediaIdInput.Clear(); // clear input box
                }
                else
                {
                    MessageBox.Show("Inga lediga kopior finns att låna.", "Error");
                }
            }
            else
            {
                MessageBox.Show("Vänligen skriv in en siffra.", "Error");
            }
        }

        private void Minalan(object sender, RoutedEventArgs e) { }
        private void Logoutbt(object sender, RoutedEventArgs e) { }
    }
}