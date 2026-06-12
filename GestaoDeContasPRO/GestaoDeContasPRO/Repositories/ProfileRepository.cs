using GestaoDeContasPRO.Models;
using GestaoDeContasPRO.Services;
using MySql.Data.MySqlClient;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GestaoDeContasPRO.Repositories
{
    public class ProfileRepository
    {
        private readonly string _connStr;
        private readonly Helpers _helpers;

        public ProfileRepository(IConfiguration config, Helpers helpers)
        {
            _connStr = config.GetConnectionString("DefaultConnection") ?? string.Empty;
            _helpers = helpers;
        }

        public void GetProfile(ref Profile profile, ref bool error)
        {
            error = false;

            try
            {
                using (MySqlConnection Conn = new MySqlConnection(_connStr))
                {
                    Conn.Open();

                    const string query = @"SELECT p.id, p.name, u.name user_name, p.active,
                                            CASE
                                                WHEN owner.favorite_profile_id = p.id THEN 1
                                                ELSE 0
                                            END AS favorite
                                            FROM profiles p
                                            LEFT JOIN profile_shares ps on ps.profile_id = @id AND ps.user_id = @userId
                                            INNER JOIN users u ON u.id = p.user_id
                                            INNER JOIN users owner ON owner.id = @userId
                                            WHERE p.id = @id AND (p.user_id = @userId OR ps.user_id = @userId)";

                    using (MySqlCommand cmd = new MySqlCommand(query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@id", profile.Id);
                        cmd.Parameters.AddWithValue("@userId", profile.UserId);

                        using (MySqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.HasRows)
                            {
                                dr.Read();

                                profile.Name = dr["name"].ToString() ?? string.Empty;
                                profile.UserName = dr["user_name"].ToString() ?? string.Empty;
                                profile.Active = Convert.ToBoolean(dr["active"]);
                                profile.Favorite = Convert.ToBoolean(dr["Favorite"]);

                                dr.Close();

                            }
                            else
                            {
                                profile.Id = 0;
                            }
                        }
                    }

                    Conn.Close();
                }
            }
            catch (Exception ex)
            {
                error = true;

                _helpers.CreateLog("UserRepository - GetProfile: " + ex.Message);
            }
        }

        public void GetUserProfiles(ref List<Profile> profiles, int userId, bool? active, ref bool error)
        {
            error = false;

            try
            {
                using (MySqlConnection Conn = new MySqlConnection(_connStr))
                {
                    Conn.Open();

                    const string query = @"SELECT 
                                            p.id, 
                                            p.name, 
                                            p.active,
                                            0 AS shared,
                                            u.name AS user_name,
                                            CASE
                                                WHEN owner.favorite_profile_id = p.id THEN 1
                                                ELSE 0
                                            END AS favorite
                                        FROM profiles p
                                        INNER JOIN users u ON u.id = p.user_id
                                        INNER JOIN users owner ON owner.id = @userId
                                        WHERE p.user_id = @userId AND (@active IS NULL OR p.active = @active)

                                        UNION ALL

                                        SELECT 
                                            p.id, 
                                            p.name, 
                                            p.active,
                                            1 AS shared,
                                            u.name AS user_name,
                                            CASE
                                                WHEN owner.favorite_profile_id = p.id THEN 1
                                                ELSE 0
                                            END AS favorite
                                        FROM profile_shares pf
                                        INNER JOIN profiles p ON p.id = pf.profile_id
                                        INNER JOIN users u ON u.id = p.user_id
                                        INNER JOIN users owner ON owner.id = @userId
                                        WHERE pf.user_id = @userId AND (@active IS NULL OR p.active = @active)

                                        ORDER BY active DESC, favorite DESC, name ASC";

                    using (MySqlCommand cmd = new MySqlCommand(query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId);
                        cmd.Parameters.AddWithValue("@active", active == null ? DBNull.Value : active);

                        using (MySqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.HasRows)
                            {
                                while (dr.Read())
                                {
                                    profiles.Add(new Profile()
                                    {
                                        Id = (int)dr["id"],
                                        Name = dr["name"] == DBNull.Value ? string.Empty : (string)dr["name"],
                                        UserName = dr["user_name"] == DBNull.Value ? string.Empty : (string)dr["user_name"],
                                        Favorite = Convert.ToBoolean(dr["favorite"]),
                                        Active = Convert.ToBoolean(dr["active"])
                                    });
                                }

                                dr.Close();

                            }
                        }
                    }

                    Conn.Close();
                }
            }
            catch (Exception ex)
            {
                error = true;

                _helpers.CreateLog("UserRepository - GetUserProfiles: " + ex.Message);
            }
        }

        public void GetProfileShares(ref List<ProfileShare> profileShares, int profileId, ref bool error)
        {
            error = false;

            try
            {
                using (MySqlConnection Conn = new MySqlConnection(_connStr))
                {
                    Conn.Open();

                    const string query = @"SELECT ps.id, u.name, ps.date_log FROM profile_shares ps
                                           INNER JOIN users u ON u.id = ps.user_id
                                           WHERE ps.profile_id = @profileId";

                    using (MySqlCommand cmd = new MySqlCommand(query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@profileId", profileId);

                        using (MySqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.HasRows)
                            {
                                while (dr.Read())
                                {
                                    profileShares.Add(new ProfileShare()
                                    {
                                        Id = (int)dr["id"],
                                        UserName = (string)dr["name"],
                                        DateLog = dr["date_log"] == DBNull.Value ? null: (DateTime)dr["date_log"]
                                    });
                                }

                                dr.Close();

                            }
                        }
                    }

                    Conn.Close();
                }
            }
            catch (Exception ex)
            {
                error = true;

                _helpers.CreateLog("UserRepository - GetProfileShares: " + ex.Message);
            }
        }

        public bool PostProfile(ref Profile profile)
        {
            bool flag = true;

            try
            {
                using (MySqlConnection Conn = new MySqlConnection(_connStr))
                {
                    Conn.Open();

                    const string query = "INSERT INTO profiles(name, user_id, active) VALUES(@name, @user_id, @active);";

                    using (MySqlCommand cmd = new MySqlCommand(query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@name", profile.Name);
                        cmd.Parameters.AddWithValue("@user_id", profile.UserId);
                        cmd.Parameters.AddWithValue("@active", profile.Active);

                        cmd.ExecuteNonQuery();
                        profile.Id = (int)cmd.LastInsertedId;
                    }

                    Conn.Close();
                }
            }
            catch (Exception ex)
            {
                flag = false;

                _helpers.CreateLog("UserRepository - PostProfile: " + ex.Message);
            }

            return flag;
        }

        public bool UpdateProfile(Profile profile)
        {
            bool flag = true;

            try
            {
                using (MySqlConnection Conn = new MySqlConnection(_connStr))
                {
                    Conn.Open();

                    const string query = @"UPDATE profiles p
                                            LEFT JOIN profile_shares ps
                                                ON ps.profile_id = p.id
                                                AND ps.user_id = @userId
                                            SET
                                                p.name = @name,
                                                p.active = @active
                                            WHERE
                                                p.id = @id
                                                AND (
                                                    p.user_id = @userId
                                                    OR ps.user_id = @userId
                                                )";

                    using (MySqlCommand cmd = new MySqlCommand(query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@name", profile.Name);
                        cmd.Parameters.AddWithValue("@active", profile.Active);
                        cmd.Parameters.AddWithValue("@userId", profile.UserId);
                        cmd.Parameters.AddWithValue("@id", profile.Id);

                        cmd.ExecuteNonQuery();
                    }

                    Conn.Close();
                }
            }
            catch (Exception ex)
            {
                flag = false;

                _helpers.CreateLog("UserRepository - UpdateProfile: " + ex.Message);
            }

            return flag;
        }

        public bool PostProfileShare(ProfileShare profileShare)
        {
            bool flag = false;

            try
            {
                using (MySqlConnection Conn = new MySqlConnection(_connStr))
                {
                    Conn.Open();

                    const string query = @"INSERT INTO profile_shares (profile_id, user_id, date_log)
                                           SELECT p.id, u.id, NOW()
                                           FROM profiles p
                                           JOIN users u ON u.email = @userEmail
                                           WHERE p.id = @profileId
                                               AND p.user_id = @userId
                                               AND NOT EXISTS (
                                                   SELECT 1
                                                   FROM profile_shares ps
                                                   WHERE ps.profile_id = p.id
                                                   AND ps.user_id = u.id
                                            )";

                    using (MySqlCommand cmd = new MySqlCommand(query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@userEmail", profileShare.UserEmail);
                        cmd.Parameters.AddWithValue("@profileId", profileShare.ProfileId);
                        cmd.Parameters.AddWithValue("@userId", profileShare.UserId);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                            flag = true;
                    }

                    Conn.Close();
                }
            }
            catch (Exception ex)
            {
                flag = false;

                _helpers.CreateLog("UserRepository - PostProfileShare: " + ex.Message);
            }

            return flag;
        }

        public bool DeleteProfileShare(ProfileShare profileShare)
        {
            bool flag = false;

            try
            {
                using (MySqlConnection Conn = new MySqlConnection(_connStr))
                {
                    Conn.Open();

                    const string query = @"DELETE ps
                                           FROM profile_shares ps
                                           INNER JOIN profiles p
                                           ON p.id = ps.profile_id
                                           WHERE ps.id = @id
                                           AND p.user_id = @userId;";

                    using (MySqlCommand cmd = new MySqlCommand(query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@id", profileShare.Id);
                        cmd.Parameters.AddWithValue("@userId", profileShare.UserId);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                            flag = true;
                    }

                    Conn.Close();
                }
            }
            catch (Exception ex)
            {
                flag = false;

                _helpers.CreateLog("UserRepository - DeleteProfileShare: " + ex.Message);
            }

            return flag;
        }

    }
}
