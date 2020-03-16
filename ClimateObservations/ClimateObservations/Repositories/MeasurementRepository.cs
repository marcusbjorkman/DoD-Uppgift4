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

        public static Measurement GetMeasurement(int id)
        {
            string stmt = "SELECT * FROM measurement WHERE id=@id";

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var command = new NpgsqlCommand(stmt, conn))
                {
                    command.Parameters.AddWithValue("id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            return null;
                        } 

                        Measurement found = new Measurement
                        {
                            Id = (int)reader["measurement.id"],
                            Value = (float)reader["Measurement.value"]
                        };

                        return found;
                    }
                }
            }
        }

        public static IEnumerable<Measurement> GetMeasurements()
        {
            string stmt = "SELECT id, value FROM measurement";

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var command = new NpgsqlCommand(stmt, conn))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            return null;
                        }

                        var resulting = new List<Measurement>();

                        while (reader.Read())
                        {
                            Measurement found = new Measurement
                            {
                                Id = (int)reader["id"],
                                Value = (double)reader["value"]
                            };

                            resulting.Add(found);
                        }

                        return resulting;
                    }
                }
            }
        }
    }
}
