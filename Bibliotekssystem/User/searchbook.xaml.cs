using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System.Collections.Generic;
using Bibliotekssystem.Database;

namespace Bibliotekssystem.User
{
    public partial class searchbook : UserControl
    {
        private DataCalls db = new DataCalls();

        public searchbook()
        {
            InitializeComponent();
            UpdateList("");
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateList(SearchBox.Text);
        }

        private void Filter_Click(object sender, RoutedEventArgs e)
        {
            UpdateList(SearchBox.Text);
        }

        private void UpdateList(string term)
        {
            var results = db.SearchMedia(term);

            // filter type
            var activeTypes = new List<string>();
            if (TypeBook.IsChecked == true) activeTypes.Add("Bok");
            if (TypeMovie.IsChecked == true) activeTypes.Add("Film");
            if (TypeAudio.IsChecked == true) activeTypes.Add("Ljudbok");

            if (activeTypes.Count > 0)
            {
                results = results.Where(m => activeTypes.Contains(m.MediaType)).ToList();
            }

            // filter category
            var activeCats = new List<string>();
            if (CatA.IsChecked == true) activeCats.Add("A -");
            if (CatB.IsChecked == true) activeCats.Add("B -");
            if (CatC.IsChecked == true) activeCats.Add("C -");
            if (CatD.IsChecked == true) activeCats.Add("D -");
            if (CatE.IsChecked == true) activeCats.Add("E -");
            if (CatF.IsChecked == true) activeCats.Add("F -");
            if (CatG.IsChecked == true) activeCats.Add("G -");
            if (CatH.IsChecked == true) activeCats.Add("H -");
            if (CatI.IsChecked == true) activeCats.Add("I -");
            if (CatJ.IsChecked == true) activeCats.Add("J -");
            if (CatK.IsChecked == true) activeCats.Add("K -");
            if (CatL.IsChecked == true) activeCats.Add("L -");
            if (CatM.IsChecked == true) activeCats.Add("M -");
            if (CatN.IsChecked == true) activeCats.Add("N -");
            if (CatO.IsChecked == true) activeCats.Add("O -");
            if (CatP.IsChecked == true) activeCats.Add("P -");
            if (CatQ.IsChecked == true) activeCats.Add("Q -");
            if (CatR.IsChecked == true) activeCats.Add("R -");
            if (CatS.IsChecked == true) activeCats.Add("S -");
            if (CatT.IsChecked == true) activeCats.Add("T -");

            // if box clicked, keep matched
            if (activeCats.Count > 0)
            {
                results = results.Where(m => m.CategoryName != null && activeCats.Any(cat => m.CategoryName.StartsWith(cat))).ToList();
            }

            // update ui
            MediaDisplayInfo.ItemsSource = results;
        }

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
                    MessageBox.Show("Du har lånat boken/filmen!", "Success");
                }
                else
                {
                    MessageBox.Show("Inga lediga kopior finns tillgängliga.", "Error");
                }
            }
        }

        // jump to loans
        private void Minalan(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Content = new Bibliotekssystem.User.myloans();
        }

        // logout
        private void Logoutbtn(object sender, RoutedEventArgs e)
        {
            MainWindow loginWindow = new MainWindow();
            Application.Current.MainWindow.Content = loginWindow.Content;
        }
    }
}