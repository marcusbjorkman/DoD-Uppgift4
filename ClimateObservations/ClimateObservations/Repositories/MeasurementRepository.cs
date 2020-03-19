using ClimateObservations.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClimateObservations.Repositories
{
    public static class MeasurementRepository
    {
        private static string connectionString = ConfigurationManager.ConnectionStrings["dbLocal"].ConnectionString;

        #region GET
        public static Measurement GetMeasurement(int id)
        {
            string stmt = "SELECT id, value, observation_id, category_id FROM measurement WHERE id=@id";

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                int categoryid;
                Measurement found = null;

                using (var command = new NpgsqlCommand(stmt, conn))
                {
                    command.Parameters.AddWithValue("id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            return null;
                        }

                        reader.Read();
                        found = new Measurement
                        {
                            Id = (int)reader["id"],
                            Value = reader["value"] == DBNull.Value ? null : (double?)reader["value"],
                            ObservationId = (int)reader["observation_id"]
                        };

                        categoryid = (int)reader["category_id"];
                    }
                }

                found.Category = CategoryRepository.LoadCategoryRecursively(categoryid, conn);
                return found;
            }
        }

        public static IEnumerable<Measurement> GetMeasurements(int? observationId = null)
        {
            string stmt = "SELECT id, value, observation_id, category_id FROM measurement";

            if (observationId != null)
            {
                stmt += " WHERE observation_id=@id";
            }

            using (var conn = new NpgsqlConnection(connectionString))
            {
                var resulting = new List<(Measurement, int)>();

                conn.Open();
                using (var command = new NpgsqlCommand(stmt, conn))
                {
                    if (observationId != null)
                    {
                        command.Parameters.AddWithValue("id", observationId.Value);
                    }

                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            return null;
                        }

                        while (reader.Read())
                        {
                            var found = (new Measurement
                            {
                                Id = (int)reader["id"],
                                Value = (double)reader["value"],
                                ObservationId = (int)reader["observation_id"]
                            }, (int)reader["category_id"]);

                            resulting.Add(found);
                        }

                    }
                }

                foreach (var m in resulting)
                {
                    m.Item1.Category = CategoryRepository.LoadCategoryRecursively(m.Item2, conn);
                }

                return resulting.Select(o => o.Item1);
            }
        }
        #endregion

        public static int AddMeasurement(int observationId, Measurement toAdd)
        {
            string stmt = "INSERT INTO measurement(value, category_id, observation_id) VALUES(@value, @categoryId, @observationId) RETURNING id";

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var command = new NpgsqlCommand(stmt, conn))
                {
                    command.Parameters.AddWithValue("value", toAdd.Value ?? Convert.DBNull);
                    command.Parameters.AddWithValue("categoryId", toAdd.Category.Id);
                    command.Parameters.AddWithValue("observationId", observationId);

                    int id = (int)command.ExecuteScalar();
                    toAdd.Id = id;

                    return id;
                }
            }
        }

        public static void PatchMeasurement(Measurement updated)
        {
            string stmt = "UPDATE measurement SET value=@value, category_id=@category_id WHERE id=@id";

            using (var conn = new NpgsqlConnection(connectionString))
            {
                using (var command = new NpgsqlCommand(stmt, conn))
                {
                    conn.Open();
                    command.Parameters.AddWithValue("value", updated.Value);
                    command.Parameters.AddWithValue("category_id", updated.Category.Id);
                    command.Parameters.AddWithValue("id", updated.Id);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
