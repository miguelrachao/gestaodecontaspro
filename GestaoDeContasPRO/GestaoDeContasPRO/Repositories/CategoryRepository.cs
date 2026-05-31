using GestaoDeContasPRO.Models;
using GestaoDeContasPRO.Services;
using MySql.Data.MySqlClient;

namespace GestaoDeContasPRO.Repositories
{
    public class CategoryRepository
    {
        private readonly string _connStr;
        private readonly Helpers _helpers;

        public CategoryRepository(IConfiguration config, Helpers helpers)
        {
            _connStr = config.GetConnectionString("DefaultConnection") ?? string.Empty;
            _helpers = helpers;
        }

        public void GetProfileCategories(ref List<Category> categories, int profileId, ref bool error)
        {
            error = false;

            try
            {
                using (MySqlConnection Conn = new MySqlConnection(_connStr))
                {
                    Conn.Open();

                    const string query = @"SELECT c.* FROM categories c
                                            INNER JOIN profiles p ON p.id = c.profile_id
                                            WHERE p.id = @profileId";

                    using (MySqlCommand cmd = new MySqlCommand(query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@profileId", profileId);

                        using (MySqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.HasRows)
                            {
                                while (dr.Read())
                                {
                                    categories.Add(new Category()
                                    {
                                        Id = (int)dr["id"],
                                        Name = dr["name"].ToString() ?? string.Empty,
                                        ActionType = dr["action_type"].ToString() ?? string.Empty,
                                        UserId = (int)dr["user_id"],
                                        ProfileId = (int)dr["profile_id"],
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
            catch (Exception ex)
            {
                error = true;

                _helpers.CreateLog("CategoryRepository - GetProfileCategories: " + ex.Message);
            }
        }
    }
}
