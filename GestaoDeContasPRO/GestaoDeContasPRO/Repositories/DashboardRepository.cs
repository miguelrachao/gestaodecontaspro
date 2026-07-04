using GestaoDeContasPRO.Models;
using GestaoDeContasPRO.Services;
using MySql.Data.MySqlClient;

namespace GestaoDeContasPRO.Repositories
{
    public class DashboardRepository
    {
        private readonly string _connStr;
        private readonly Helpers _helpers;

        public DashboardRepository(IConfiguration config, Helpers helpers)
        {
            _connStr = config.GetConnectionString("DefaultConnection") ?? string.Empty;
            _helpers = helpers;
        }

        public void GetEntriesAmountByCategory(ref List<Entry> entries, int profileId, DateTime? startDate, DateTime? endDate, int userId, ref bool error)
        {
            error = false;

            try
            {
                using (MySqlConnection Conn = new MySqlConnection(_connStr))
                {
                    Conn.Open();

                    const string query = @"SELECT
                                            ROUND(COALESCE(SUM(e.amount), 0),2) AS amount,
                                            c.budget, 
                                            COALESCE(ROUND((COALESCE(SUM(e.amount), 0) / c.budget) * 100, 0),0) AS budget_coverage,
                                            c.id AS category_id,
                                            c.name AS category_name,
                                            c.action_type AS category_type
                                            
                                        FROM categories c

                                        INNER JOIN profiles p
                                            ON p.id = c.profile_id

                                        LEFT JOIN profile_shares ps
                                            ON ps.profile_id = p.id
                                            AND ps.user_id = @userId

                                        LEFT JOIN entries e
                                            ON e.category_id = c.id
                                            AND e.profile_id = p.id
                                            AND DATE(e.date) >= DATE(@startDate)
                                            AND DATE(e.date) <= DATE(@endDate)

                                        WHERE
                                            c.profile_id = @profileId
                                            AND (p.user_id = @userId OR ps.user_id = @userId)

                                        GROUP BY
                                            c.id,
                                            c.name,
                                            c.action_type

                                        ORDER BY
                                            c.action_type DESC,
                                            c.name ASC";

                    using (MySqlCommand cmd = new MySqlCommand(query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@profileId", profileId);
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
                                        Category =
                                        {
                                            Id = (int)dr["category_id"],
                                            Name = (string)dr["category_name"],
                                            Type = Enum.Parse<ActionType>(dr["category_type"].ToString()!),
                                            Budget = Convert.ToDouble(dr["budget"]),
                                            BudgetCoverage = Convert.ToDouble(dr["budget_coverage"])
                                        },
                                        Amount = Convert.ToDouble(dr["amount"])
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

                _helpers.CreateLog("EntryRepository - GetEntriesAmountByCategory: " + ex.Message);
            }
        }
    }
}
