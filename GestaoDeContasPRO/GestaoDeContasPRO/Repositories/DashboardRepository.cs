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
                                                ROUND(COALESCE(SUM(e.amount), 0), 2) AS amount,

                                                (c.budget * (TIMESTAMPDIFF(MONTH, DATE(@startDate), DATE(@endDate)) + 1)) AS budget,

                                                COALESCE(
                                                    ROUND(
                                                        (
                                                            COALESCE(SUM(e.amount), 0)
                                                            /
                                                            (c.budget * (TIMESTAMPDIFF(MONTH, DATE(@startDate), DATE(@endDate)) + 1))
                                                        ) * 100,
                                                        0
                                                    ),
                                                    0
                                                ) AS budget_coverage,

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
                                                c.name ASC;";

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
                                            Name = dr["category_name"] == DBNull.Value ? string.Empty : (string)dr["category_name"],
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

        public void GetYearBalance(ref List<YearBalance> yearBalances, int profileId, string yearBalance, int userId, ref bool error)
        {
            error = false;

            try
            {
                using (MySqlConnection Conn = new MySqlConnection(_connStr))
                {
                    Conn.Open();

                    const string query = @"SELECT
                                                MONTH(e.date) month,
                                                SUM(CASE WHEN c.action_type = 'CREDIT' THEN e.amount ELSE 0 END) credit,
                                                SUM(CASE WHEN c.action_type = 'DEBIT' THEN ABS(e.amount) ELSE 0 END) debit,
                                                (SUM(CASE WHEN c.action_type = 'CREDIT' THEN e.amount ELSE 0 END)) - (SUM(CASE WHEN c.action_type = 'DEBIT' THEN ABS(e.amount) ELSE 0 END)) balance
                                            FROM entries e

                                            INNER JOIN profiles p ON p.id = e.profile_id
                                            LEFT JOIN profile_shares ps ON ps.profile_id = @profileId AND ps.user_id = @userId
                                            INNER JOIN users u ON u.id = p.user_id
                                            INNER JOIN categories c ON c.id = e.category_id

                                            WHERE p.id = @profileId 

                                            AND YEAR(e.date)=@yearBalance

                                            AND (p.user_id = @userId OR ps.user_id = @userId)

                                            GROUP BY MONTH(e.date)
                                            ORDER BY MONTH(e.date)";

                    using (MySqlCommand cmd = new MySqlCommand(query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@profileId", profileId);
                        cmd.Parameters.AddWithValue("@yearBalance", yearBalance);
                        cmd.Parameters.AddWithValue("@userId", userId);

                        using (MySqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.HasRows)
                            {
                                while (dr.Read())
                                {
                                    yearBalances.Add(new YearBalance()
                                    {
                                        Month = (int)dr["Month"],
                                        Credit = Convert.ToDouble(dr["Credit"]),
                                        Debit = Convert.ToDouble(dr["Debit"]),
                                        Balance = Convert.ToDouble(dr["Balance"])
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

                _helpers.CreateLog("EntryRepository - GetYearBalance: " + ex.Message);
            }
        }
    }
}
