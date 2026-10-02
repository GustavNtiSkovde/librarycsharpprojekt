using System;
using System.Windows;
using System.Windows.Controls;
using Bibliotekssystem.Database;

namespace Bibliotekssystem.Admin {
    /// <summary>
    /// Interaction logic for AddMovie.xaml
    /// </summary>
    public partial class AddMovie : UserControl {
        private readonly DataCalls db = new DataCalls();

        public AddMovie() {
            InitializeComponent();
        }
        private void StartSideBtn(object sender, RoutedEventArgs e) {
            Content = new Bibliotekssystem.Admin.MainWindowAdmin();
        }
        private void Logoutbt(object sender, RoutedEventArgs e) {
            MainWindow loginWindow = new MainWindow();
            Application.Current.MainWindow.Content = loginWindow.Content;
        }

        private void EditMovieDoneBtn(object sender, RoutedEventArgs e) {
            string title = TitleInput?.Text?.Trim() ?? "tom";

            if (string.IsNullOrWhiteSpace(title)) {
                MessageBox.Show("Ange en titel för filmen.", "Validering", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try {
                string ean = EanInput?.Text?.Trim() ?? "tom";
                string director = DirectorInput?.Text?.Trim() ?? "tom";
                string barcode = BarcodeInput?.Text?.Trim() ?? "tom";
                string sab = SabInput?.Text?.Trim() ?? "tom";
                string lengthStr = LengthInput?.Text?.Trim() ?? "tom";
                string yearStr = YearInput?.Text?.Trim() ?? "tom";
                string purchaseValueStr = PurchaseValueInput?.Text?.Trim() ?? "tom";
                string replacementValueStr = ReplacementValueInput?.Text?.Trim() ?? "tom";

                int length = int.TryParse(lengthStr, out var len) ? len : 0;
                int year = int.TryParse(yearStr, out var y) ? y : 0;
                decimal purchaseValue = decimal.TryParse(purchaseValueStr, out var pv) ? pv : 0m;
                decimal replacementValue = decimal.TryParse(replacementValueStr, out var rv) ? rv : 0m;


                bool created = db.CreateMovie(title, ean, director, length, year, sab, barcode, purchaseValue, replacementValue, sab);

                if (created) {
                    MessageBox.Show("Filmen skapades.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    Content = new Bibliotekssystem.Admin.MainWindowAdmin();
                }
                else {
                    MessageBox.Show("Kunde inte skapa filmen.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine(ex);
                MessageBox.Show("Ett fel uppstod: " + ex.Message, "Fel", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}