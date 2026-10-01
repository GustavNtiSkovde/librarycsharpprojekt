using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Bibliotekssystem.Database
{
    public class DataCalls
    {
        // db connection
        private string connectionString = "Server=127.0.0.1;Port=3307;Database=librarystina;Uid=root;Pwd=admin123;AllowPublicKeyRetrieval=True;";

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

        // get role
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
                        if (result != null) return result.ToString();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Databas-fel: " + ex.Message, "Krasch");
                }
            }
            return null;
        }

        // get user data
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
                                return new User
                                {
                                    Id = reader.IsDBNull(reader.GetOrdinal("ID")) ? 0 : reader.GetInt32("ID"),
                                    Role = reader.IsDBNull(reader.GetOrdinal("role")) ? string.Empty : reader.GetString("role"),
                                    Email = reader.IsDBNull(reader.GetOrdinal("email")) ? string.Empty : reader.GetString("email")
                                };
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

        // search everything
        public List<Media> SearchMedia(string searchTerm)
        {
            List<Media> results = new List<Media>();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                // join tables to get the media type
                string query = @"
                    SELECT m.ID, m.title, m.FORcategory, c.description AS categoryName, 
                           b.isbn, mov.ID as movieID, au.ID as audioID,
                           GROUP_CONCAT(CONCAT(p.fname, ' ', p.lname) SEPARATOR ', ') AS authors
                    FROM media m
                    LEFT JOIN book b ON m.ID = b.ID
                    LEFT JOIN movie mov ON m.ID = mov.ID
                    LEFT JOIN audiobook au ON m.ID = au.ID
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

                            // check type
                            if (!reader.IsDBNull(reader.GetOrdinal("isbn"))) m.MediaType = "Bok";
                            else if (!reader.IsDBNull(reader.GetOrdinal("movieID"))) m.MediaType = "Film";
                            else if (!reader.IsDBNull(reader.GetOrdinal("audioID"))) m.MediaType = "Ljudbok";
                            else m.MediaType = "Okänd";

                            results.Add(m);
                        }
                    }
                }
            }
            return results;
        }

        // borrow if copy is free
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
                    DateTime dueDate = startDate.AddDays(21);

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

                        return new Loan
                        {
                            Id = Convert.ToInt32(cmd.ExecuteScalar()),
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

        // return copy
        public bool ReturnMedia(int copyId)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "UPDATE loan SET status = 'Returned' WHERE FORcopy = @copy AND status = 'Active'";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@copy", copyId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // deal with late returns
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
                                reader.GetInt32("LoanID"), reader.GetInt32("FORuser"), reader.GetDecimal("replacementvalue")));
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
                        invoices.Add(new Invoice { Id = Convert.ToInt32(cmd.ExecuteScalar()), ForUser = data.Item2, Amount = penalty });
                    }
                }
            }
            return invoices;
        }

        // get unpaid stuff
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

        // pay it
        public bool PayInvoice(int invoiceId)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand("UPDATE invoice SET status = 'Paid' WHERE ID = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", invoiceId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // list loans, group by id to stop dupes
        public List<Loan> GetAllActiveLoans()
        {
            List<Loan> results = new List<Loan>();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string query = @"
                    SELECT 
                        l.ID, l.loanstartdate, l.returndate, l.FORuser, l.FORcopy,
                        m.title AS Title, 
                        GROUP_CONCAT(CONCAT(p.fname, ' ', p.lname) SEPARATOR ', ') AS Author,
                        cat.description AS CategoryName
                    FROM loan l
                    JOIN copy c ON l.FORcopy = c.ID
                    JOIN media m ON c.FORmedia = m.ID
                    LEFT JOIN category cat ON m.FORcategory = cat.sabcode
                    LEFT JOIN personmedia pm ON m.ID = pm.FORmedia
                    LEFT JOIN person p ON pm.FORperson = p.ID
                    WHERE l.status = 'Active'
                    GROUP BY l.ID";

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
                            Author = reader.IsDBNull(reader.GetOrdinal("Author")) ? "Okänd" : reader.GetString("Author"),
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

        // add copy
        public bool AddNewCopy(int mediaId)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand("INSERT INTO copy (FORmedia) VALUES (@mediaId)", conn))
                {
                    cmd.Parameters.AddWithValue("@mediaId", mediaId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // delete copy
        public bool RemoveCopy(int copyId)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand("DELETE FROM copy WHERE ID = @copyId", conn))
                {
                    cmd.Parameters.AddWithValue("@copyId", copyId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // new user
        public bool AddNewUser(string email, string password, bool isAdmin)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand("INSERT INTO user (role, email, password) VALUES (@role, @email, @password)", conn))
                {
                    cmd.Parameters.AddWithValue("@role", isAdmin ? "Admin" : "Borrower");
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@password", password);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
        // update user role in db
        public bool UpdateUserRole(string email, bool isAdmin)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand("UPDATE user SET role = @role WHERE email = @email", conn))
                {
                    cmd.Parameters.AddWithValue("@role", isAdmin ? "Admin" : "Borrower");
                    cmd.Parameters.AddWithValue("@email", email);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
        // change media title in db
        public bool UpdateMedia(int mediaId, string newTitle)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand("UPDATE media SET title = @title WHERE ID = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@title", newTitle);
                    cmd.Parameters.AddWithValue("@id", mediaId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
   
        public bool DeleteMedia(int mediaId)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        using (var cmd = new MySqlCommand("DELETE FROM loan WHERE FORcopy IN (SELECT ID FROM copy WHERE FORmedia = @id)", conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@id", mediaId);
                            cmd.ExecuteNonQuery();
                        }

                        using (var cmd = new MySqlCommand("DELETE FROM copy WHERE FORmedia = @id", conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@id", mediaId);
                            cmd.ExecuteNonQuery();
                        }

                        using (var cmd = new MySqlCommand("DELETE FROM book WHERE ID = @id", conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@id", mediaId);
                            cmd.ExecuteNonQuery();
                        }

                        using (var cmd = new MySqlCommand("DELETE FROM movie WHERE ID = @id", conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@id", mediaId);
                            cmd.ExecuteNonQuery();
                        }

                        using (var cmd = new MySqlCommand("DELETE FROM audiobook WHERE ID = @id", conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@id", mediaId);
                            cmd.ExecuteNonQuery();
                        }

                        using (var cmd = new MySqlCommand("DELETE FROM personmedia WHERE FORmedia = @id", conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@id", mediaId);
                            cmd.ExecuteNonQuery();
                        }

                        using (var cmd = new MySqlCommand("DELETE FROM media WHERE ID = @id", conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@id", mediaId);
                            int rows = cmd.ExecuteNonQuery();

                            transaction.Commit();
                            return rows > 0;
                        }
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
        // sök användare
        public List<User> SearchUsers(string searchTerm)
        {
            List<User> results = new List<User>();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT ID, role, email FROM user WHERE email LIKE @search OR role LIKE @search";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@search", "%" + searchTerm + "%");
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            results.Add(new User
                            {
                                Id = reader.GetInt32("ID"),
                                Role = reader.GetString("role"),
                                Email = reader.GetString("email")
                            });
                        }
                    }
                }
            }
            return results;
        }

        // ta bort användare
        public bool DeleteUser(int userId)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        using (var cmd = new MySqlCommand("DELETE FROM loan WHERE FORuser = @id", conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@id", userId);
                            cmd.ExecuteNonQuery();
                        }

                        using (var cmd = new MySqlCommand("DELETE FROM invoice WHERE FORuser = @id", conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@id", userId);
                            cmd.ExecuteNonQuery();
                        }

                        using (var cmd = new MySqlCommand("DELETE FROM user WHERE ID = @id", conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@id", userId);
                            int rows = cmd.ExecuteNonQuery();

                            transaction.Commit();
                            return rows > 0;
                        }
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
        // skapa ny media i databasen
        public bool CreateMedia(string title, int categoryCode, string mediaType)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        string insertMedia = "INSERT INTO media (title, FORcategory, replacementvalue) VALUES (@title, @cat, 150.00); SELECT LAST_INSERT_ID();";
                        int mediaId = 0;

                        using (var cmd = new MySqlCommand(insertMedia, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@title", title);
                            cmd.Parameters.AddWithValue("@cat", categoryCode);
                            mediaId = Convert.ToInt32(cmd.ExecuteScalar());
                        }

                        if (mediaType == "Bok")
                        {
                            using (var cmd = new MySqlCommand("INSERT INTO book (ID, isbn) VALUES (@id, '9789100000000')", conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@id", mediaId);
                                cmd.ExecuteNonQuery();
                            }
                        }
                        else if (mediaType == "Film")
                        {
                            using (var cmd = new MySqlCommand("INSERT INTO movie (ID) VALUES (@id)", conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@id", mediaId);
                                cmd.ExecuteNonQuery();
                            }
                        }
                        else if (mediaType == "Ljudbok")
                        {
                            using (var cmd = new MySqlCommand("INSERT INTO audiobook (ID) VALUES (@id)", conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@id", mediaId);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        // skapa en standardkopia direkt
                        using (var cmd = new MySqlCommand("INSERT INTO copy (FORmedia) VALUES (@id)", conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@id", mediaId);
                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        // skapa konto med lösenord
        public bool CreateUserAccount(string email, string password, bool isAdmin)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "INSERT INTO user (role, email, password) VALUES (@role, @email, @password)";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@role", isAdmin ? "Admin" : "Borrower");
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@password", password);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // ändra lösenord för användare
        public bool UpdateUserPassword(string email, string newPassword)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "UPDATE user SET password = @pwd WHERE email = @email";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@pwd", newPassword);
                    cmd.Parameters.AddWithValue("@email", email);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}