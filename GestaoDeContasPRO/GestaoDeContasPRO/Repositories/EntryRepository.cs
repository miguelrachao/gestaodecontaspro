using GestaoDeContasPRO.Models;
using GestaoDeContasPRO.Services;
using Microsoft.AspNetCore.Authorization;
using MySql.Data.MySqlClient;

namespace GestaoDeContasPRO.Repositories
{
    [Authorize]
    public class EntryRepository
    {
        private readonly string _connStr;
        private readonly Helpers _helpers;

        public EntryRepository(IConfiguration config, Helpers helpers)
        {
            _connStr = config.GetConnectionString("DefaultConnection") ?? string.Empty;
            _helpers = helpers;
        }

        public void GetProfileEntries(ref List<Entry> entries, int profileId, int categoryId, DateTime? startDate, DateTime? endDate, int userId, ref bool error)
        {
            error = false;

            try
            {
                using (MySqlConnection Conn = new MySqlConnection(_connStr))
                {
                    Conn.Open();

                    const string query = @"SELECT e.id, e.category_id, c.name category_name, c.action_type category_type, e.amount, e.description, e.date
                                           FROM entries e
                                           INNER JOIN profiles p ON p.id = e.profile_id
                                           LEFT JOIN profile_shares ps ON ps.profile_id = @profileId AND ps.user_id = @userId
                                           INNER JOIN users u ON u.id = p.user_id
                                           INNER JOIN categories c ON c.id = e.category_id
                                           WHERE p.id = @profileId 
                                           AND (@categoryId = 0 OR c.id = @categoryId)
                                           AND (@startDate IS NULL OR DATE(e.date) >= DATE(@startDate))
                                           AND (@endDate IS NULL OR DATE(e.date) <= DATE(@endDate))
                                           AND (p.user_id = @userId OR ps.user_id = @userId)";

                    using (MySqlCommand cmd = new MySqlCommand(query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@profileId", profileId);
                        cmd.Parameters.AddWithValue("@categoryId", categoryId);
                        cmd.Parameters.AddWithValue("@startDate", startDate == null ? DBNull.Value : startDate);
                        cmd.Parameters.AddWithValue("@endDate", endDate == null ? DBNull.Value : endDate);
                        cmd.Parameters.AddWithValue("@userId", userId);

                        using (MySqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.HasRows)
                            {
                                while (dr.Read())
                                {
                                    entries.Add(new Entry()
                                    {
                                        Id = (int)dr["id"],
                                        Category =
                                        {
                                            Id = (int)dr["category_id"],
                                            Name = (string)dr["category_name"],
                                            Type = Enum.Parse<ActionType>(dr["category_type"].ToString()!),
                                        },
                                        Amount = Convert.ToDouble(dr["amount"]),
                                        Description = (string)dr["description"],
                                        Date = (DateTime)dr["Date"]
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

                _helpers.CreateLog("EntryRepository - GetProfileEntries: " + ex.Message);
            }
        }

        public bool PostEntry(Entry entry)
        {
            bool flag = true;

            try
            {
                using (MySqlConnection Conn = new MySqlConnection(_connStr))
                {
                    Conn.Open();

                    const string query = "INSERT INTO entries(profile_id, category_id, ammount, description, date) " +
                        "                 VALUES(@profile_id, @category_id, @ammount, @description, @date)";

                    using (MySqlCommand cmd = new MySqlCommand(query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@profile_id", entry.ProfileId);
                        cmd.Parameters.AddWithValue("@category_id", entry.Category.Id);
                        cmd.Parameters.AddWithValue("@ammount", entry.Amount);
                        cmd.Parameters.AddWithValue("@description", entry.Description);
                        cmd.Parameters.AddWithValue("@date", entry.Date);

                        cmd.ExecuteNonQuery();
                    }

                    Conn.Close();
                }
            }
            catch (Exception ex)
            {
                flag = false;

                _helpers.CreateLog("EntryRepository - PostEntry: " + ex.Message);
            }

            return flag;
        }

        public bool UpdateEntry(Entry entry)
        {
            bool flag = true;

            try
            {
                using (MySqlConnection Conn = new MySqlConnection(_connStr))
                {
                    Conn.Open();

                    const string query = @"UPDATE entries SET category_id = @category_id, ammount = @ammount,
                                           description = @description, date = @date WHERE id = @id";

                    using (MySqlCommand cmd = new MySqlCommand(query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@id", entry.Id);
                        cmd.Parameters.AddWithValue("@category_id", entry.Category.Id);
                        cmd.Parameters.AddWithValue("@ammount", entry.Amount);
                        cmd.Parameters.AddWithValue("@description", entry.Description);
                        cmd.Parameters.AddWithValue("@date", entry.Date);

                        cmd.ExecuteNonQuery();
                    }

                    Conn.Close();
                }
            }
            catch (Exception ex)
            {
                flag = false;

                _helpers.CreateLog("EntryRepository - UpdateEntry: " + ex.Message);
            }

            return flag;
        }

        public bool DeleteEntry(int id)
        {
            bool flag = true;

            try
            {
                using (MySqlConnection Conn = new MySqlConnection(_connStr))
                {
                    Conn.Open();

                    const string query = @"DELETE FROM entries WHERE id = @id";

                    using (MySqlCommand cmd = new MySqlCommand(query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);

                        cmd.ExecuteNonQuery();
                    }

                    Conn.Close();
                }
            }
            catch (Exception ex)
            {
                flag = false;

                _helpers.CreateLog("EntryRepository - DeleteEntry: " + ex.Message);
            }

            return flag;
        }
    }
}
