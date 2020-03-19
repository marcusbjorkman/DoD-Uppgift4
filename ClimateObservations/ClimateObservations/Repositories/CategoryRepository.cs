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
    public static class CategoryRepository
    {
        private static string connectionString = ConfigurationManager.ConnectionStrings["dbLocal"].ConnectionString;

        public static Category LoadCategoryRecursively(int categoryId, NpgsqlConnection conn)
        {
            string c_stmt = "SELECT id, name, basecategory_id, unit_id FROM category WHERE id=@id";
            string u_stmt = "SELECT id, type, abbreviation FROM unit WHERE id=@id";

            Category found = null;
            int? basecategoryId = null;
            int? unitId = null;

            using (var command = new NpgsqlCommand(c_stmt, conn))
            {
                command.Parameters.AddWithValue("id", categoryId);
                using (var reader = command.ExecuteReader())
                {
                    if (!reader.HasRows)
                    {
                        return null;
                    }

                    reader.Read();
                    found = new Category
                    {
                        Id = (int)reader["id"],
                        Name = (string)reader["name"]
                    };

                    basecategoryId = reader["basecategory_id"] == DBNull.Value ? null : (int?)reader["basecategory_id"];
                    unitId = reader["unit_id"] == DBNull.Value ? null : (int?)reader["unit_id"];
                }
            }
            
            if (unitId.HasValue)
            {
                using (var command = new NpgsqlCommand(u_stmt, conn))
                {
                    command.Parameters.AddWithValue("id", unitId.Value);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();

                            found.Unit = new Unit
                            {
                                Id = (int)reader["id"],
                                Type = (string)reader["type"],
                                Abbreviation = (string)reader["abbreviation"]
                            };
                        }
                    }
                }
            }

            if (basecategoryId.HasValue)
            {
                found.BaseCategory = LoadCategoryRecursively(basecategoryId.Value, conn);
            }

            return found;
        }
    }
}
