using System;
using System.Collections.Generic;

namespace Bibliotekssystem.Database
{
 
    public class User
    {
        public int Id { get; set; }
        public string Role { get; set; } = "User"; 
        public string Email { get; set; } = string.Empty; //  hashed
        public string Password { get; set; } = string.Empty; //  hashed
    }

  
    public class Category
    {
        public int SabCode { get; set; } 
        public string Description { get; set; } = string.Empty; 
    }

    public class Media
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string MediaType { get; set; } = string.Empty;
        public decimal PurchaseValue { get; set; }
        public decimal ReplacementValue { get; set; }
        public string Barcode { get; set; } = string.Empty;
        public string? Isbn { get; set; }
        public string? Ean { get; set; }
        public int ForCategory { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string Authors { get; set; } = string.Empty;
    }

    public class Person
    {
        public int Id { get; set; }
        public string Fnamn { get; set; } = string.Empty; 
        public string Lnamn { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty; 
    }

    
    public class PersonMedia
    {
        public int Id { get; set; }
        public int ForMedia { get; set; } 
        public int ForPerson { get; set; } 
    }

    public class Copy
    {
        public int Id { get; set; }
        public string LibraryCode { get; set; } = string.Empty; 
        public int ForMedia { get; set; } 
    }

    public class Loan
    {
        public int Id { get; set; }
        public DateTime LoanStartDate { get; set; } = DateTime.Now;
        public DateTime? ReturnDate { get; set; }
        public string Status { get; set; } = "Aktiv";
        public int ForUser { get; set; }
        public int ForCopy { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
    }

    public class Invoice
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime EndDate { get; set; } 
        public string Status { get; set; } = "Obetald"; 
        public int ForUser { get; set; }
    }
}