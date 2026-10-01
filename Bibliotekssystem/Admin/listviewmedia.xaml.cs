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

            // load all media on start
            UpdateList("");
        }

        // updates list when typing
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateList(SearchBox.Text);
        }

        // grab data from db and update list
        private void UpdateList(string term)
        {
            var results = db.SearchMedia(term);
            MediaDisplayInfo.ItemsSource = results;
        }

        // delete btn click
        private void DeleteMediaBtn_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;

            if (btn != null && btn.Tag != null)
            {
                // get id from button tag
                int mediaId = (int)btn.Tag;

                MessageBoxResult result = MessageBox.Show($"Are you sure you want to delete media ID {mediaId}?", "Confirm", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    // add db.DeleteMedia(mediaId) here later

                    // refresh list
                    UpdateList(SearchBox.Text);
                }
            }
        }

        // open create media view
        private void CreateMediaBtn(object sender, RoutedEventArgs e)
        {

        }

        // logout
        private void Logoutbt(object sender, RoutedEventArgs e)
        {

        }
    }
}