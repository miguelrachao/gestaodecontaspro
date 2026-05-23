using GestaoDeContasPRO.Models;
using MySql.Data.MySqlClient;

namespace GestaoDeContasPRO.Repositories
{
    public class ProfileRepository
    {
        private readonly string _connStr;

        public ProfileRepository(IConfiguration config)
        {
            _connStr = config.GetConnectionString("DefaultConnection") ?? string.Empty;
        }

        public bool GetUserFavoriteProfile(ref Profile profile, ref bool error)
        {
            bool flag = false;
            error = false;

            try
            {
                using (MySqlConnection Conn = new MySqlConnection(_connStr))
                {
                    Conn.Open();

                    const string query = @"SELECT p.id, p.name 
                                           FROM users u
                                           INNER JOIN profiles p on p.id = u.favorite_profile_id
                                           WHERE u.id = @userId AND p.active = 1;";

                    using (MySqlCommand cmd = new MySqlCommand(query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@userId", profile.UserId);

                        using (MySqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.HasRows)
                            {
                                dr.Read();

                                profile.Id = (int)dr["id"];
                                profile.Name = dr["name"].ToString() ?? string.Empty;

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

        public void GetUserProfiles(ref List<Profile> profiles, int userId, ref bool error)
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
                                    0 shared,
                                    'own' owner_user_name
                                    FROM profiles p 
                                    WHERE p.user_id = @userId
                                    ORDER BY p.name ASC";

                    using (MySqlCommand cmd = new MySqlCommand(query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId);

                        using (MySqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.HasRows)
                            {
                                while (dr.Read())
                                {
                                    profiles.Add(new Profile()
                                    {
                                        Id = (int)dr["id"],
                                        Name = dr["name"].ToString() ?? string.Empty,
                                        Active = Convert.ToBoolean(dr["Active"])
                                    });
                                }

                                dr.Close();

                            }
                        }
                    }

                    Conn.Close();
                }
            }
            catch { error = true; }
        }

        public void GetUserAllProfiles(ref List<Profile> profiles, int userId, ref bool error)
        {
            error = false;

            try
            {
                using (MySqlConnection Conn = new MySqlConnection(_connStr))
                {
                    Conn.Open();

                    const string query = @"(SELECT 
                                    p.id, 
                                    p.name, 
                                    p.active, 
                                    0 shared,
                                    'own' owner_user_name
                                    FROM profiles p 
                                    WHERE p.user_id = @userId
                                    ORDER BY p.name ASC)

                                    UNION ALL

                                    (SELECT 
                                    DISTINCT(p.id), 
                                    p.name, 
                                    p.active,
                                    1 shared,
                                    u.name owner_user_name
                                    FROM profile_shares pf
                                    INNER JOIN profiles p on p.id = pf.profile_id
                                    INNER JOIN users u on u.id = p.user_id
                                    WHERE pf.user_id = @userId
                                    ORDER BY p.name ASC)";

                    using (MySqlCommand cmd = new MySqlCommand(query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId);

                        using (MySqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.HasRows)
                            {
                                while (dr.Read())
                                {
                                    profiles.Add(new Profile()
                                    {
                                        Id = (int)dr["id"],
                                        Name = dr["name"].ToString() ?? string.Empty,
                                        Active = Convert.ToBoolean(dr["Active"])
                                    });
                                }

                                dr.Close();

                            }
                        }
                    }

                    Conn.Close();
                }
            }
            catch { error = true; }
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
            catch { flag = false; }

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

                    const string query = "UPDATE profiles SET name=@name, active=@active WHERE id=@id";

                    using (MySqlCommand cmd = new MySqlCommand(query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@name", profile.Name);
                        cmd.Parameters.AddWithValue("@active", profile.Active);
                        cmd.Parameters.AddWithValue("@id", profile.Id);

                        cmd.ExecuteNonQuery();
                    }

                    Conn.Close();
                }
            }
            catch { flag = false; }

            return flag;
        }
    }
}
