using System;
using System.Threading.Tasks;
using System.Windows;
using Bibliotekssystem.Database;

namespace Bibliotekssystem
{
    public partial class MainWindow : Window
    {
        private DataCalls db;

        public MainWindow()
        {
            InitializeComponent();
            db = new DataCalls();

            Task.Run(() => StartConsoleLoop());
        }

        private void StartConsoleLoop()
        {
            while (true)
            {
                Console.Write("> ");
                string input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                    continue;

                string[] parts = input.Split(' ');
                string command = parts[0].ToLower();

                if (command == "/exit")
                {
                    Application.Current.Dispatcher.Invoke(() => Application.Current.Shutdown());
                    break;
                }
                else if (command == "/search" && parts.Length > 1)
                {
                    TestQuery(parts[1]);
                }
                else if (command == "/overdue")
                {
                    RunLateFeeTest();
                }
                else if (command == "/lana" && parts.Length == 3)
                {
                    int mediaId = int.Parse(parts[1]);
                    int userId = int.Parse(parts[2]);

                    var loan = db.BorrowMedia(userId, mediaId);

                    if (loan != null)
                    {
                        Console.WriteLine($"Lån skapat. KopiaID: {loan.CopyId} | Förfaller: {loan.DueDate:yyyy-MM-dd}");
                    }
                    else
                    {
                        Console.WriteLine("Fel: Ingen ledig kopia hittades för detta media.");
                    }
                }
                else if (command == "/aterlamna" && parts.Length == 2)
                {
                    int copyId = int.Parse(parts[1]);
                    bool success = db.ReturnMedia(copyId);

                    if (success)
                    {
                        Console.WriteLine($"Kopia {copyId} har lämnats tillbaka och är tillgänglig.");
                    }
                    else
                    {
                        Console.WriteLine("Fel: Hittade inget aktivt lån för denna kopia.");
                    }
                }
            }
        }

        private void RunLateFeeTest()
        {
            var invoices = db.ProcessOverdueLoans();
            Console.WriteLine($"Försenade lån/Fakturor: {invoices.Count}");

            foreach (var inv in invoices)
            {
                Console.WriteLine($"LånID: {inv.LoanId} | Användare: {inv.BorrowerName} | Media: {inv.Title} | Belopp: {inv.InvoiceAmount}kr");
            }

            Console.WriteLine("Kopior i systemet:");
            foreach (var copy in db.Copies)
            {
                Console.WriteLine($"KopiaID: {copy.Id} | Status: {copy.Status}");
            }
        }

        private void TestQuery(string term)
        {
            var results = db.SearchMedia(term);
            Console.WriteLine($"Träffar: {results.Count}");

            foreach (var item in results)
            {
                Console.WriteLine($"ID: {item.Id} | Titel: {item.Title} | SAB: {item.SabCategory}");
            }
        }
    }
}