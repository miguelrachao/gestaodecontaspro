using GestaoDeContasPRO.Models;
using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.Data;

namespace GestaoDeContasPRO.Repositories
{
    public class UserRepository
    {
        private readonly string _connStr;

        public UserRepository(IConfiguration config)
        {
            _connStr = config.GetConnectionString("DefaultConnection") ?? string.Empty;
        }

        public bool GetByEmail(ref User user, ref bool error)
        {
            bool flag = false;
            error = false;

            try
            {
                using (MySqlConnection Conn = new MySqlConnection(_connStr))
                {
                    Conn.Open();

                    const string query = "SELECT * FROM users WHERE email = @email";

                    using (MySqlCommand cmd = new MySqlCommand(query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@email", user.Email);

                        using (MySqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.HasRows)
                            {
                                dr.Read();

                                user.Id = (int)dr["id"];
                                user.Name = dr["name"].ToString() ?? string.Empty;
                                user.FavoriteProfileId = (int)dr["favorite_profile_id"];
                            
                                dr.Close();

                                flag = true;
                            }
                        }
                    }

                    Conn.Close();
                }
            }
            catch { error = true; }

            return flag;
        }

        public bool PostUser(User user)
        {
            bool flag = true;

            try
            {
                using (MySqlConnection Conn = new MySqlConnection(_connStr))
                {
                    Conn.Open();

                    const string query = "INSERT INTO users(name, email, otp_code, otp_expiration, favorite_profile_id) VALUES(@name, @email, 0, NOW(), 0)";

                    using (MySqlCommand cmd = new MySqlCommand(query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@name", user.Name);
                        cmd.Parameters.AddWithValue("@email", user.Email);

                        cmd.ExecuteScalar();
                    }

                    Conn.Close();
                }
            }
            catch { flag = false; }

            return flag;
        }

        public bool UpdateOtp(User user)
        {
            bool flag = true;

            try
            {
                using (MySqlConnection Conn = new MySqlConnection(_connStr))
                {
                    Conn.Open();

                    const string query = "UPDATE users SET otp_code = @otp_code, otp_expiration = @otp_expiration WHERE id = @id";

                    using (MySqlCommand cmd = new MySqlCommand(query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@otp_code", user.OtpCode);
                        cmd.Parameters.AddWithValue("@otp_expiration", user.OtpExpiration);
                        cmd.Parameters.AddWithValue("@id", user.Id);

                        cmd.ExecuteScalar();
                    }

                    Conn.Close();
                }
            }
            catch
            {
                flag = false;
            }

            return flag;
        }

        public bool ValidateOtp(ref User user, ref bool error)
        {
            bool flag = false;
            error = false;

            try
            {
                using (MySqlConnection Conn = new MySqlConnection(_connStr))
                {
                    Conn.Open();

                    const string query = "SELECT * FROM users WHERE email = @email AND otp_code = @otp_code AND otp_expiration > NOW()";

                    using (MySqlCommand cmd = new MySqlCommand(query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@email", user.Email);
                        cmd.Parameters.AddWithValue("@otp_code", user.OtpCode);

                        using (MySqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.HasRows)
                            {
                                dr.Read();

                                user.Id = (int)dr["id"];
                                user.Name = dr["name"].ToString() ?? string.Empty;
                                user.FavoriteProfileId = (int)dr["favorite_profile_id"];

                                dr.Close();

                                flag = true;
                            }
                        }
                    }

                    Conn.Close();
                }
            }
            catch { error = true; }

            return flag;
        }

        public bool HasAlreadyOtp(User user, ref bool error) {

            bool flag = false;
            error = false;

            try
            {
                using (MySqlConnection Conn = new MySqlConnection(_connStr))
                {
                    Conn.Open();

                    const string query = "SELECT * FROM users WHERE email = @email AND otp_code != 0 AND otp_expiration > NOW()";

                    using (MySqlCommand cmd = new MySqlCommand(query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@email", user.Email);

                        using (MySqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.HasRows)
                            {
                                flag = true;
                            }
                        }
                    }

                    Conn.Close();
                }
            }
            catch { error = true; }

            return flag;
        }
    }
}
