using System;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using ForVlad.Data;

namespace ForVlad.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        public bool Authenticate(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return false;

            try
            {
                using (var connection = new SqlConnection(DatabaseConnection.GetConnectionString()))
                {
                    connection.Open();
                    
                    var command = new SqlCommand(
                        "SELECT PasswordHash, PasswordSalt FROM Users WHERE UserName = @UserName AND IsActive = 1 AND IsLocked = 0", 
                        connection);
                    command.Parameters.AddWithValue("@UserName", username);
                    
                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                            return false;
                        
                        string storedHash = reader["PasswordHash"] as string;
                        string storedSalt = reader["PasswordSalt"] as string;
                        
                        if (string.IsNullOrEmpty(storedHash))
                            return false;
                        
                        // Вычисляем хэш: SHA256(username + salt + password)
                        string inputHash = ComputeSha256Hash(username + (storedSalt ?? "") + password);
                        
                        return string.Equals(storedHash, inputHash, StringComparison.OrdinalIgnoreCase);
                    }
                }
            }
            catch
            {
                return false;
            }
        }
        
        private static string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
