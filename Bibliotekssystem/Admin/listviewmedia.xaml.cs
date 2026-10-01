using Bibliotekssystem.Database;
using Bibliotekssystem.User;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Bibliotekssystem.Admin
{
    public partial class Listviewmedia : UserControl
    {
        // db connection
        private DataCalls db = new DataCalls();

        public Listviewmedia()
        {
            InitializeComponent();

            // load all media on start[cite: 19]
            UpdateList("");
        }

        // updates list when typing[cite: 19]
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateList(SearchBox.Text);
        }

        // grab data from db and update list[cite: 19]
        private void UpdateList(string term)
        {
            var results = db.SearchMedia(term);
            MediaDisplayInfo.ItemsSource = results;
        }

        // delete media click[cite: 19]
        private void DeleteMediaBtn_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;

            if (btn != null && btn.Tag != null)
            {
                // get id from button tag[cite: 19]
                int mediaId = (int)btn.Tag;

                MessageBoxResult result = MessageBox.Show($"Are you sure you want to delete media ID {mediaId}?", "Confirm", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    // delete from db
                    if (db.DeleteMedia(mediaId))
                    {
                        MessageBox.Show("Media borttagen!", "Success");
                    }
                    else
                    {
                        MessageBox.Show("Kunde inte ta bort media.", "Error");
                    }

                    // refresh list[cite: 19]
                    UpdateList(SearchBox.Text);
                }
            }
        }

        // logout[cite: 19]
        private void Logoutbt(object sender, RoutedEventArgs e)
        {
            MainWindow loginWindow = new MainWindow();
            Application.Current.MainWindow.Content = loginWindow.Content;
        }
        private void StartSideBtn(object sender, RoutedEventArgs e) {
            Content = new Bibliotekssystem.Admin.MainWindowAdmin();
        }

        private void CreateBookBtn(object sender, RoutedEventArgs e) {
            Content = new Bibliotekssystem.Admin.AddBook();
        }

        private void CreateAudioBookBtn(object sender, RoutedEventArgs e)
        {
            Content = new Bibliotekssystem.Admin.AddAudioBook();
        }

        private void CreateMovieBtn(object sender, RoutedEventArgs e)
        {
            Content = new Bibliotekssystem.Admin.AddMovie();
        }
    }
}