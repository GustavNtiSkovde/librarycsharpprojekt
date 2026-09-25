using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Bibliotekssystem.Database;
 //everything is ai just to test 
namespace Bibliotekssystem
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Kör terminaltestet direkt vid start
            RunLateFeeTest();
        }

        private void RunLateFeeTest()
        {
            Console.WriteLine("========================================");
            Console.WriteLine("   KÖR TEST: FÖRSENING & FAKTURALOGIK   ");
            Console.WriteLine("========================================\n");

            DataCalls db = new DataCalls();

            Console.WriteLine("1. Kollar status före kontroll:");
            foreach (var copy in db.Copies)
            {
                Console.WriteLine($"   Kopia {copy.Barcode} (ID: {copy.Id}) - Status: {copy.Status}");
            }

            Console.WriteLine("\n2. Letar efter försenade lån...");
            var invoices = db.ProcessOverdueLoans();

            Console.WriteLine($"\nAntal försenade lån funna: {invoices.Count}");
            Console.WriteLine("----------------------------------------");

            foreach (var inv in invoices)
            {
                Console.WriteLine($"FAKTURA FÖR LÅN #{inv.LoanId}:");
                Console.WriteLine($"  Låntagare:         {inv.BorrowerName}");
                Console.WriteLine($"  Media:             {inv.Title}");
                Console.WriteLine($"  Förfallodatum:     {inv.DueDate:yyyy-MM-dd}");
                Console.WriteLine($"  Ursprungligt pris: {inv.OriginalPrice} kr");
                Console.WriteLine($"  Fakturabelopp:     {inv.InvoiceAmount} kr (1.5x straffavgift)");
                Console.WriteLine("----------------------------------------");
            }

            Console.WriteLine("\n3. Kollar status efter kontroll:");
            foreach (var copy in db.Copies)
            {
                Console.WriteLine($"   Kopia {copy.Barcode} (ID: {copy.Id}) - Ny Status: {copy.Status}");
            }

            Console.WriteLine("\n[TEST KLART] Tryck i konsolen eller stäng fönstret.");
        }
    }
}