using System;
using System.Windows;
using System.Windows.Controls;
using Bibliotekssystem.Database;

namespace Bibliotekssystem.Admin {
    /// <summary>
    /// Interaction logic for AddAudioBook.xaml
    /// </summary>
    public partial class AddAudioBook : UserControl {
        private readonly DataCalls db = new DataCalls();

        public AddAudioBook() {
            InitializeComponent();
        }
        private void StartSideBtn(object sender, RoutedEventArgs e) {
            Content = new Bibliotekssystem.Admin.MainWindowAdmin();
        }
        private void Logoutbt(object sender, RoutedEventArgs e) {
            MainWindow loginWindow = new MainWindow();
            Application.Current.MainWindow.Content = loginWindow.Content;
        }

        private void EditAudioBookDoneBtn(object sender, RoutedEventArgs e) {
            string title = TitleInput?.Text?.Trim() ?? "tom";

            if (string.IsNullOrWhiteSpace(title)) {
                MessageBox.Show("Ange en titel för ljudboken.", "Validering", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try {
                string isbn = IsbnInput?.Text?.Trim() ?? "tom";
                string creator = CreatorInput?.Text?.Trim() ?? "tom";
                string barcode = BarcodeInput?.Text?.Trim() ?? "tom";
                string sab = SabInput?.Text?.Trim() ?? "tom";
                string audioFormat = AudioFormatInput?.Text?.Trim() ?? "tom";
                string language = LanguageInput?.Text?.Trim() ?? "tom";
                string lengthStr = LengthInput?.Text?.Trim() ?? "tom";
                string purchaseValueStr = PurchaseValueInput?.Text?.Trim() ?? "tom";
                string replacementValueStr = ReplacementValueInput?.Text?.Trim() ?? "tom";

                int length = int.TryParse(lengthStr, out var len) ? len : 0;
                decimal purchaseValue = decimal.TryParse(purchaseValueStr, out var pv) ? pv : 0m;
                decimal replacementValue = decimal.TryParse(replacementValueStr, out var rv) ? rv : 0m;

                bool created = db.CreateAudiobook(title, isbn, creator, length, audioFormat, "", barcode, purchaseValue, replacementValue, sab);

                if (created) {
                    MessageBox.Show("Ljudboken skapades.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    Content = new Bibliotekssystem.Admin.MainWindowAdmin();
                }
                else {
                    MessageBox.Show("Kunde inte skapa ljudboken.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine(ex);
                MessageBox.Show("Ett fel uppstod: " + ex.Message, "Fel", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}