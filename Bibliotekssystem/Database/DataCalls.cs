using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Bibliotekssystem.Database
{
    public class DataCalls
    {
        // Connection string (Update Port/Pwd if you switch between Docker containers)
        private string connectionString = "Server=127.0.0.1;Port=3306;Database=librarystina;Uid=root;Pwd=1234;AllowPublicKeyRetrieval=True;";

        // Test connection
        public void TestConnection()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    Console.WriteLine("Connected to the database.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed: " + ex.Message);
            }
        }

        // Returns user role ("Admin", "Borrower", etc.) for page redirection
        public string? VerifyUserLogin(string inputEmail, string inputPassword)
        {
            string query = "SELECT role FROM user WHERE email = @Email AND password = @Password";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Email", inputEmail);
                        command.Parameters.AddWithValue("@Password", inputPassword);

                        object result = command.ExecuteScalar();
                        if (result != null)
                        {
                            return result.ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Databas-fel: " + ex.Message, "Krasch");
                }
            }
            return null;
        }

        // Retrieves user object if full account context is needed
        public User? GetUserByEmailAndPassword(string inputEmail, string inputPassword)
        {
            string query = "SELECT ID, role, email FROM user WHERE email = @Email AND password = @Password LIMIT 1";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Email", inputEmail);
                        command.Parameters.AddWithValue("@Password", inputPassword);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var user = new User
                                {
                                    Id = reader.IsDBNull(reader.GetOrdinal("ID")) ? 0 : reader.GetInt32("ID"),
                                    Role = reader.IsDBNull(reader.GetOrdinal("role")) ? string.Empty : reader.GetString("role"),
                                    Email = reader.IsDBNull(reader.GetOrdinal("email")) ? string.Empty : reader.GetString("email")
                                };
                                return user;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Database Connection Error: " + ex.Message);
                }
            }
            return null;
        }

        // Search media, books, categories, and authors
        public List<Media> SearchMedia(string searchTerm)
        {
            List<Media> results = new List<Media>();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string query = @"
                    SELECT m.ID, m.title, m.FORcategory, c.description AS categoryName, b.isbn,
                           GROUP_CONCAT(CONCAT(p.fname, ' ', p.lname) SEPARATOR ', ') AS authors
                    FROM media m
                    LEFT JOIN book b ON m.ID = b.ID
                    LEFT JOIN category c ON m.FORcategory = c.sabcode
                    LEFT JOIN personmedia pm ON m.ID = pm.FORmedia
                    LEFT JOIN person p ON pm.FORperson = p.ID
                    WHERE m.title LIKE @search 
                       OR b.isbn LIKE @search 
                       OR c.description LIKE @search 
                       OR p.fname LIKE @search 
                       OR p.lname LIKE @search
                    GROUP BY m.ID";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@search", "%" + searchTerm + "%");
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Media m = new Media
                            {
                                Id = reader.GetInt32("ID"),
                                Title = reader.GetString("title"),
                                ForCategory = reader.IsDBNull(reader.GetOrdinal("FORcategory")) ? 0 : reader.GetInt32("FORcategory"),
                                Isbn = reader.IsDBNull(reader.GetOrdinal("isbn")) ? null : reader.GetString("isbn"),
                                CategoryName = reader.IsDBNull(reader.GetOrdinal("categoryName")) ? "" : reader.GetString("categoryName"),
                                Authors = reader.IsDBNull(reader.GetOrdinal("authors")) ? "" : reader.GetString("authors")
                            };

                            results.Add(m);
                        }
                    }
                }
            }
            return results;
        }

        public Loan? BorrowMedia(int userId, int mediaId)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string findCopyQuery = @"
                    SELECT ID FROM copy 
                    WHERE FORmedia = @mediaId 
                    AND ID NOT IN (SELECT FORcopy FROM loan WHERE status = 'Active') 
                    LIMIT 1";

                int availableCopyId = 0;
                using (MySqlCommand cmd = new MySqlCommand(findCopyQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@mediaId", mediaId);
                    object result = cmd.ExecuteScalar();
                    if (result != null) availableCopyId = Convert.ToInt32(result);
                }

                if (availableCopyId > 0)
                {
                    DateTime startDate = DateTime.Now;
                    DateTime dueDate = startDate.AddDays(21); // 3 weeks 

                    string insertLoanQuery = @"
                        INSERT INTO loan (loanstartdate, returndate, status, FORuser, FORcopy) 
                        VALUES (@start, @end, 'Active', @user, @copy);
                        SELECT LAST_INSERT_ID();";

                    using (MySqlCommand cmd = new MySqlCommand(insertLoanQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@start", startDate);
                        cmd.Parameters.AddWithValue("@end", dueDate);
                        cmd.Parameters.AddWithValue("@user", userId);
                        cmd.Parameters.AddWithValue("@copy", availableCopyId);

                        int newLoanId = Convert.ToInt32(cmd.ExecuteScalar());

                        return new Loan
                        {
                            Id = newLoanId,
                            ForCopy = availableCopyId,
                            LoanStartDate = startDate,
                            ReturnDate = dueDate,
                            ForUser = userId
                        };
                    }
                }
            }
            return null;
        }

        public bool ReturnMedia(int copyId)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "UPDATE loan SET status = 'Returned' WHERE FORcopy = @copy AND status = 'Active'";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@copy", copyId);
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        public List<Invoice> ProcessOverdueLoans()
        {
            List<Invoice> invoices = new List<Invoice>();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string findOverdueQuery = @"
                    SELECT l.ID as LoanID, l.FORuser, m.replacementvalue 
                    FROM loan l
                    JOIN copy c ON l.FORcopy = c.ID
                    JOIN media m ON c.FORmedia = m.ID
                    WHERE l.status = 'Active' AND l.returndate < @now";

                List<Tuple<int, int, decimal>> overdueData = new List<Tuple<int, int, decimal>>();

                using (MySqlCommand cmd = new MySqlCommand(findOverdueQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@now", DateTime.Now.Date);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            overdueData.Add(new Tuple<int, int, decimal>(
                                reader.GetInt32("LoanID"),
                                reader.GetInt32("FORuser"),
                                reader.GetDecimal("replacementvalue")
                            ));
                        }
                    }
                }

                foreach (var data in overdueData)
                {
                    decimal penalty = data.Item3 * 1.5m;

                    string updateLoan = "UPDATE loan SET status = 'Overdue' WHERE ID = @loanId";
                    using (MySqlCommand cmd = new MySqlCommand(updateLoan, conn))
                    {
                        cmd.Parameters.AddWithValue("@loanId", data.Item1);
                        cmd.ExecuteNonQuery();
                    }

                    string insertInvoice = @"
                        INSERT INTO invoice (amount, enddate, status, FORuser) 
                        VALUES (@amount, @enddate, 'Unpaid', @user);
                        SELECT LAST_INSERT_ID();";

                    using (MySqlCommand cmd = new MySqlCommand(insertInvoice, conn))
                    {
                        cmd.Parameters.AddWithValue("@amount", penalty);
                        cmd.Parameters.AddWithValue("@enddate", DateTime.Now.AddDays(30));
                        cmd.Parameters.AddWithValue("@user", data.Item2);

                        int newInvoiceId = Convert.ToInt32(cmd.ExecuteScalar());
                        invoices.Add(new Invoice { Id = newInvoiceId, ForUser = data.Item2, Amount = penalty });
                    }
                }
            }
            return invoices;
        }

        public List<Invoice> GetUnpaidInvoices()
        {
            List<Invoice> results = new List<Invoice>();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT ID, amount, enddate, FORuser FROM invoice WHERE status = 'Unpaid'";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        results.Add(new Invoice
                        {
                            Id = reader.GetInt32("ID"),
                            Amount = reader.GetDecimal("amount"),
                            EndDate = reader.GetDateTime("enddate"),
                            ForUser = reader.GetInt32("FORuser"),
                            Status = "Unpaid"
                        });
                    }
                }
            }
            return results;
        }

        public bool PayInvoice(int invoiceId)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "UPDATE invoice SET status = 'Paid' WHERE ID = @id";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", invoiceId);
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        // Active loans query mapped with Title, Author, and CategoryName
        public List<Loan> GetAllActiveLoans()
        {
            List<Loan> results = new List<Loan>();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string query = @"
                    SELECT 
                        l.ID, 
                        l.loanstartdate, 
                        l.returndate, 
                        l.FORuser, 
                        l.FORcopy,
                        m.title AS Title, 
                        p.fname AS FirstName, 
                        p.lname AS LastName,
                        cat.description AS CategoryName
                    FROM loan l
                    JOIN copy c ON l.FORcopy = c.ID
                    JOIN media m ON c.FORmedia = m.ID
                    LEFT JOIN category cat ON m.FORcategory = cat.sabcode
                    LEFT JOIN personmedia pm ON m.ID = pm.FORmedia
                    LEFT JOIN person p ON pm.FORperson = p.ID
                    WHERE l.status = 'Active'";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Loan l = new Loan
                        {
                            Id = reader.GetInt32("ID"),
                            LoanStartDate = reader.GetDateTime("loanstartdate"),
                            ForUser = reader.GetInt32("FORuser"),
                            ForCopy = reader.GetInt32("FORcopy"),
                            Title = reader.IsDBNull(reader.GetOrdinal("Title")) ? "Okänd Titel" : reader.GetString("Title"),
                            Author = reader.IsDBNull(reader.GetOrdinal("FirstName")) ? "Okänd" : $"{reader.GetString("FirstName")} {reader.GetString("LastName")}",
                            CategoryName = reader.IsDBNull(reader.GetOrdinal("CategoryName")) ? "" : reader.GetString("CategoryName")
                        };

                        if (!reader.IsDBNull(reader.GetOrdinal("returndate")))
                        {
                            l.ReturnDate = reader.GetDateTime("returndate");
                        }

                        results.Add(l);
                    }
                }
            }
            return results;
        }

        public bool AddNewCopy(int mediaId)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "INSERT INTO copy (FORmedia) VALUES (@mediaId)";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@mediaId", mediaId);
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        public bool RemoveCopy(int copyId)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "DELETE FROM copy WHERE ID = @copyId";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@copyId", copyId);
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        public bool AddNewUser(string email, string password, bool isAdmin)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string query = "INSERT INTO user (role, email, password) VALUES (@role, @email, @password)";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    string roleChoice = isAdmin ? "Admin" : "Borrower";

                    cmd.Parameters.AddWithValue("@role", roleChoice);
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@password", password);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }
    }
}