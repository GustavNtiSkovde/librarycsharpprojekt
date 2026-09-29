using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;

namespace Bibliotekssystem.Database
{
    public class DataCalls
    {
        public bool VerifyUserLogin(string inputEmail, string inputPassword) {
            string query = "SELECT COUNT(*) FROM user WHERE email = @Email AND password = @Password";

            using (MySqlConnection connection = new MySqlConnection(connectionString)) {
                try {
                    connection.Open();
                    using (MySqlCommand command = new MySqlCommand(query, connection)) {
                        command.Parameters.AddWithValue("@Email", inputEmail);
                        command.Parameters.AddWithValue("@Password", inputPassword);

                        int userCount = Convert.ToInt32(command.ExecuteScalar());
                        return userCount > 0;
                    }
                }
                catch (Exception ex) {
                    Console.WriteLine("Database Connection Error: " + ex.Message);
                    return false;
                }
            }
        private string connectionString = "Server=127.0.0.1;Database=librarystina;Uid=root;Pwd=1234;AllowPublicKeyRetrieval=True;";         // database connection string

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

        // search
        public List<Media> SearchMedia(string searchTerm)
        {
            List<Media> results = new List<Media>();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                // SQL query to search media, books, categories, and authors
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
                            //grab results and put into media object
                            Media m = new Media();
                            m.Id = reader.GetInt32("ID");
                            m.Title = reader.GetString("title");
                            m.ForCategory = reader.IsDBNull(reader.GetOrdinal("FORcategory")) ? 0 : reader.GetInt32("FORcategory");
                            m.Isbn = reader.IsDBNull(reader.GetOrdinal("isbn")) ? null : reader.GetString("isbn");

                            // grab the actual string names aksed for in the query
                            m.CategoryName = reader.IsDBNull(reader.GetOrdinal("categoryName")) ? "" : reader.GetString("categoryName");
                            m.Authors = reader.IsDBNull(reader.GetOrdinal("authors")) ? "" : reader.GetString("authors");

                            // if match show
                            results.Add(m);
                        }
                    }
                }
            }
            return results;
        }

        public Loan BorrowMedia(int userId, int mediaId)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                // loop to see which is available and filter out active loans
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

                // if copy found create loan in sql 
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

                        // gets the new id from db
                        int newLoanId = Convert.ToInt32(cmd.ExecuteScalar());

                        return new Loan { Id = newLoanId, ForCopy = availableCopyId, LoanStartDate = startDate };
                    }
                }
            }
            return null; // if no copy available
        }

        public bool ReturnMedia(int copyId)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                // mark it as returned and make copy available again
                string query = "UPDATE loan SET status = 'Returned' WHERE FORcopy = @copy AND status = 'Active'";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@copy", copyId);
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0; // returns true if updated
                }
            }
        }

        // identifies overdueloan and creates invoice for it, also writes off the copy
        public List<Invoice> ProcessOverdueLoans()
        {
            List<Invoice> invoices = new List<Invoice>();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                // loop every loan to find overdue ones and grab replacement value
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

                // create invoice and close overdue loans
                foreach (var data in overdueData)
                {
                    decimal penalty = data.Item3 * 1.5m; // 1.5x price

                    // write off loan
                    string updateLoan = "UPDATE loan SET status = 'Overdue' WHERE ID = @loanId";
                    using (MySqlCommand cmd = new MySqlCommand(updateLoan, conn))
                    {
                        cmd.Parameters.AddWithValue("@loanId", data.Item1);
                        cmd.ExecuteNonQuery();
                    }

                    // create invoice
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
    }
}