using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;

namespace Bibliotekssystem.Database
{
    internal class DataCalls
    {
        private string connectionString = "Server=127.0.0.1;Port=3307;Database=librarystina;Uid=root;Pwd=admin123;";

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
        }
    }
}
