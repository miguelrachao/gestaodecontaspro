using GestaoDeContasPRO.Models;
using GestaoDeContasPRO.Services;
using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.Data;

namespace GestaoDeContasPRO.Repositories
{
    public class UserRepository
    {
        private readonly string _connStr;
        private readonly Helpers _helpers;

        public UserRepository(IConfiguration config, Helpers helpers)
        {
            _connStr = config.GetConnectionString("DefaultConnection") ?? string.Empty;
            _helpers = helpers;
        }

        public bool GetById(ref User user, ref bool error)
        {
            bool flag = false;
            error = false;

            try
            {
                using (MySqlConnection Conn = new MySqlConnection(_connStr))
                {
                    Conn.Open();

                    const string query = "SELECT * FROM users WHERE id = @id";

                    using (MySqlCommand cmd = new MySqlCommand(query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@id", user.Id);

                        using (MySqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.HasRows)
                            {
                                dr.Read();

                                user.Id = (int)dr["id"];
                                user.Name = dr["name"].ToString() ?? string.Empty;
                                user.Email = dr["email"].ToString() ?? string.Empty;
                                user.FavoriteProfileId = (int)dr["favorite_profile_id"];

                                dr.Close();

                                flag = true;
                            }
                        }
                    }

                    Conn.Close();
                }
            }
            catch (Exception ex)
            { 
                error = true;

                _helpers.CreateLog("UserRepository - GetById: " + ex.Message);
            }

            return flag;
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
            catch (Exception ex)
            {
                error = true;

                _helpers.CreateLog("UserRepository - GetByEmail: " + ex.Message);
            }

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
            catch (Exception ex)
            {
                flag = false;

                _helpers.CreateLog("UserRepository - PostUser: " + ex.Message);
            }


            return flag;
        }

        public bool UpdateUserName(User user)
        {
            bool flag = true;

            try
            {
                using (MySqlConnection Conn = new MySqlConnection(_connStr))
                {
                    Conn.Open();

                    const string query = "UPDATE users SET name = @name WHERE id = @id";

                    using (MySqlCommand cmd = new MySqlCommand(query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@id", user.Id);
                        cmd.Parameters.AddWithValue("@name", user.Name);
                        

                        cmd.ExecuteScalar();
                    }

                    Conn.Close();
                }
            }
            catch (Exception ex)
            {
                flag = false;

                _helpers.CreateLog("UserRepository - UpdateUserName: " + ex.Message);
            }

            return flag;
        }

        public bool UpdateUserFavoriteProfile(User user)
        {
            bool flag = true;

            try
            {
                using (MySqlConnection Conn = new MySqlConnection(_connStr))
                {
                    Conn.Open();

                    const string query = "UPDATE users SET favorite_profile_id = @favorite_profile_id WHERE id = @id";

                    using (MySqlCommand cmd = new MySqlCommand(query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@id", user.Id);
                        cmd.Parameters.AddWithValue("@favorite_profile_id", user.FavoriteProfileId);


                        cmd.ExecuteScalar();
                    }

                    Conn.Close();
                }
            }
            catch (Exception ex)
            {
                flag = false;

                _helpers.CreateLog("UserRepository - UpdateUserFavoriteProfile: " + ex.Message);
            }

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
            catch (Exception ex)
            {
                flag = false;

                _helpers.CreateLog("UserRepository - UpdateOtp: " + ex.Message);
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
            catch (Exception ex)
            {
                error = true;

                _helpers.CreateLog("UserRepository - ValidateOtp: " + ex.Message);
            }

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
            catch (Exception ex)
            {
                error = true;

                _helpers.CreateLog("UserRepository - HasAlreadyOtp: " + ex.Message);
            }

            return flag;
        }
    }
}
