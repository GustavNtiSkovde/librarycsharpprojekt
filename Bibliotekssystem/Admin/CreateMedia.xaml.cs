using System;
using System.Windows;
using System.Windows.Controls;
using Bibliotekssystem.Database;

namespace Bibliotekssystem.Admin
{
    public partial class EditMedia : UserControl
    {
        private DataCalls db = new DataCalls();

        public EditMedia()
        {
            InitializeComponent();
        }

        // save changes and go back
        private void EditAccountDoneBtn(object sender, RoutedEventArgs e)
        {
            try
            {
                int mediaId = Convert.ToInt32(IdInput.Text);
                string newTitle = TitleInput.Text;

                // call db
                if (db.UpdateMedia(mediaId, newTitle))
                {
                    MessageBox.Show("media uppdaterad!", "success");
                    Application.Current.MainWindow.Content = new Bibliotekssystem.Admin.Listviewmedia();
                }
                else
                {
                    MessageBox.Show("kunde inte hitta det id:t.", "error");
                }
            }
            catch
            {
                MessageBox.Show("id måste vara en siffra.", "error");
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