using GestaoDeContasPRO.Models;
using GestaoDeContasPRO.Services;
using MySql.Data.MySqlClient;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
                                            WHERE p.id = @profileId
                                            ORDER BY c.Active DESC, c.name ASC";

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
                                        Type = Enum.Parse<ActionType>(dr["action_type"].ToString()!),
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

        public bool PostCategory(ref Category category)
        {
            bool flag = true;

            try
            {
                using (MySqlConnection Conn = new MySqlConnection(_connStr))
                {
                    Conn.Open();

                    const string query = "INSERT INTO categories(name, action_type, user_id, profile_id, active) VALUES(@name, @action_type, @user_id, @profile_id, @active);";

                    using (MySqlCommand cmd = new MySqlCommand(query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@name", category.Name);
                        cmd.Parameters.AddWithValue("@action_type", category.Type.ToString());
                        cmd.Parameters.AddWithValue("@user_id", category.UserId);
                        cmd.Parameters.AddWithValue("@profile_id", category.ProfileId);
                        cmd.Parameters.AddWithValue("@active", category.Active);

                        cmd.ExecuteNonQuery();
                        category.Id = (int)cmd.LastInsertedId;
                    }

                    Conn.Close();
                }
            }
            catch (Exception ex)
            {
                flag = false;

                _helpers.CreateLog("CategoryRepository - PostCategory: " + ex.Message);
            }

            return flag;
        }

        public bool UpdateCategory(Category category)
        {
            bool flag = true;

            try
            {
                using (MySqlConnection Conn = new MySqlConnection(_connStr))
                {
                    Conn.Open();

                    const string query = @"UPDATE categories c

                                            LEFT JOIN profiles p 
	                                            ON p.id = c.profile_id
	
                                            LEFT JOIN profile_shares ps
	                                            ON ps.profile_id = p.id
	                                            AND ps.user_id = @userId
	
                                            SET 
	                                            c.name = @name,
	                                            c.action_type = @actionType,
	                                            c.active = @active
	
                                            WHERE 
	                                            c.id = @id
	                                            AND (
		                                            p.user_id = @userId
		                                            OR ps.user_id = @userId
	                                            )";

                    using (MySqlCommand cmd = new MySqlCommand(query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@id", category.Id);
                        cmd.Parameters.AddWithValue("@name", category.Name);
                        cmd.Parameters.AddWithValue("@actionType", category.Type.ToString());
                        cmd.Parameters.AddWithValue("@active", category.Active);
                        cmd.Parameters.AddWithValue("@userId", category.UserId);

                        cmd.ExecuteNonQuery();
                    }

                    Conn.Close();
                }
            }
            catch (Exception ex)
            {
                flag = false;

                _helpers.CreateLog("CategoryRepository - UpdateCategory: " + ex.Message);
            }

            return flag;
        }
    }
}
