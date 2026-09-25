using System;
using System.Collections.Generic;
using System.Text;
namespace Bibliotekssystem.Database
{
    public class DataCalls
    {
        // placeholder testing data remove oneday (maybe) 
        public List<User> Users = new List<User>
        {
            new User { Id = 1, Username = "user", Role = UserRole.User },
            new User { Id = 2, Username = "admin", Role = UserRole.Admin }
        };

        public List<Media> MediaList = new List<Media>
        {
            new Book { Id = 1, Title = "Sagan om Ringen", Price = 250m, ISBN = "978-0261102385", SabCategory = "Hc" },
            new Book { Id = 2, Title = "C# för Nybörjare", Price = 400m, ISBN = "978-9144000000", SabCategory = "F" }
        };

        public List<Copy> Copies = new List<Copy>
        {
            new Copy { Id = 101, MediaId = 1, Barcode = "KOP-001", Status = "Utlånad" },
            new Copy { Id = 102, MediaId = 2, Barcode = "KOP-002", Status = "Utlånad" }
        };

        public List<Loan> Loans = new List<Loan>
        {
            // Försenat lån 5dgr
            new Loan
            {
                Id = 1,
                CopyId = 101,
                UserId = 1,
                LoanDate = DateTime.Now.AddDays(-26),
                DueDate = DateTime.Now.AddDays(-5), // overdue
                ReturnedDate = null
            },
            // not overdue
            new Loan
            {
                Id = 2,
                CopyId = 102,
                UserId = 1,
                LoanDate = DateTime.Now.AddDays(-5),
                DueDate = DateTime.Now.AddDays(16),
                ReturnedDate = null
            }
        };

        // shopping logic

        // Regel: Fakturaunderlag motsvarar 1.5 × mediets värde
        public decimal CalculateLateFee(decimal mediaPrice)
        {
            return mediaPrice * 1.5m; 
        }

        // identifies overdueloan and creates invoice for it, also writes off the copy
        public List<OverdueInvoice> ProcessOverdueLoans()
        {
            List<OverdueInvoice> invoices = new List<OverdueInvoice>();

            // loop every loan to find overdue ones
            foreach (Loan loan in Loans)
            {
                // Kontrollera om boken inte är återlämnad och slutdatumet har passerat
                if (loan.ReturnedDate == null && loan.DueDate < DateTime.Now)
                {
                    // find vilken kopia
                    Copy foundCopy = null;
                    foreach (Copy c in Copies)
                    {
                        if (c.Id == loan.CopyId)
                        {
                            foundCopy = c;
                            break; 
                        }
                    }

                    // find price on media
                    Media foundMedia = null;
                    if (foundCopy != null)
                    {
                        foreach (Media m in MediaList)
                        {
                            if (m.Id == foundCopy.MediaId)
                            {
                                foundMedia = m;
                                break;
                            }
                        }
                    }

                    // find user by id
                    User foundUser = null;
                    foreach (User u in Users)
                    {
                        if (u.Id == loan.UserId)
                        { 
                            foundUser = u;
                            break;
                        }
                    }

                    // if all is good write off the copy and create invoice
                    if (foundCopy != null && foundMedia != null && foundUser != null)
                    {
                        foundCopy.Status = "Avskriven";

                        // skapa fakutra och 1.5x priset
                        OverdueInvoice invoice = new OverdueInvoice();
                        invoice.LoanId = loan.Id;
                        invoice.CopyId = foundCopy.Id;
                        invoice.Title = foundMedia.Title;
                        invoice.BorrowerName = foundUser.Username;
                        invoice.DueDate = loan.DueDate;
                        invoice.OriginalPrice = foundMedia.Price;
                        invoice.InvoiceAmount = foundMedia.Price * 1.5m; 

                        
                        invoices.Add(invoice);
                    }
                }
            }

            return invoices;
        }
    }
}
