using Dapper;
using GestaoDeContasPRO.Models;
using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace GestaoDeContasPRO.Repositories
{
    public class UserRepository
    {
        private readonly string _connectionString;
        public UserRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection") ?? string.Empty;
        }

        public User? GetByEmail(string email)
        {
            using var conn = new MySqlConnection(_connectionString);
            return conn.QueryFirstOrDefault<User>(
                "SELECT * FROM users WHERE email = @Email", new { Email = email });
        }

        public void UpdateOtp(int userId, int otp, DateTime expiration)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Execute(
                "UPDATE users SET otp_code = @otp, otp_expiration = @exp WHERE id = @id",
                new { otp, exp = expiration, id = userId });
        }
    }
}
