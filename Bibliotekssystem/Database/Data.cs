using System;
using System.Collections.Generic;
using System.Text;
//most is placeholder shit
namespace Bibliotekssystem.Database
{
    //roles
    public enum UserRole
    {
        User,
        Admin
    }

    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.User; //skiljer user och admin
    }

    // placeholder data model for media items (books, movies, audiobooks)
    public class Media
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string SabCategory { get; set; } = string.Empty; // sab reffer to this shit -> https://en.wikipedia.org/wiki/Swedish_library_classification_system
        public decimal Price { get; set; }                      // Inköps o ersättningsvärde
        public string MediaType { get; set; } = string.Empty;   // bok, film, ljudbokk

    }

    // isbn o författare är specifika för böcker 
    public class Book : Media
    {
        public string ISBN { get; set; } = string.Empty;        // Unikt ISBN
        public List<Author> Authors { get; set; } = new List<Author>(); // M - M
    }

    public class Author
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    //fysisk copia
    public class Copy
    {
        public int Id { get; set; }
        public int MediaId { get; set; }
        public string Barcode { get; set; } = string.Empty;     // Unik streckkod/exemplar-ID
        public string Status { get; set; } = "Tillgänglig";    // Status: Tillgänglig, Utlånad, Avskriven
    }

    // Registrering av lån
    public class Loan
    {
        public int Id { get; set; }
        public int CopyId { get; set; }
        public int UserId { get; set; }
        public DateTime LoanDate { get; set; } = DateTime.Now;
        public DateTime DueDate { get; set; } = DateTime.Now.AddDays(21); // 3 weeks
        public DateTime? ReturnedDate { get; set; }
    }

    // Försenings och fakturaunderlag för administratören
    public class OverdueInvoice
    {
        public int LoanId { get; set; }
        public int CopyId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string BorrowerName { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal InvoiceAmount { get; set; } // 1.5 × media värde
    }
}