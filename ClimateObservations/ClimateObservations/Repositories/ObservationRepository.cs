using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using Npgsql;
using ClimateObservations.Models;

namespace ClimateObservations.Repositories
{
    public static class ObservationRepository
    {
        private static string connectionString = ConfigurationManager.ConnectionStrings["dbLocal"].ConnectionString;


        // CRUD
        #region CREATE
        public static int AddObservation(Observation observation)
        {
            string stmt = "INSERT INTO observation(date, observer_id, geolocation_id) values(@date,@observer_id, @geolocation_id) returning id";

            using (var conn = new NpgsqlConnection(connectionString))
            {
                using (var command = new NpgsqlCommand(stmt, conn))
                {
                    conn.Open();
                    command.Parameters.AddWithValue("date", observation.Date);
                    command.Parameters.AddWithValue("observer_id", observation.Observer_id);
                    command.Parameters.AddWithValue("geolocation_id", observation.Geolocation_id);
                    int id = (int)command.ExecuteScalar();
                    observation.Id = id;
                    return id;
                }
            }
        }

        public static void AddObservation(List<Observation> observations)
        {
            string stmt = "INSERT INTO observation(date, observer_id, geolocation_id) values(@date,@observer_id, @geolocation_id) returning id"; ;

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        using (var command = new NpgsqlCommand())
                        {
                            foreach (var observation in observations)
                            {
                                command.Parameters.AddWithValue("date", observation.Date);
                                command.Parameters.AddWithValue("observer_id", observation.Observer_id);
                                command.Parameters.AddWithValue("geolocation", observation.Geolocation_id);


                                //  command.Parameters.AddWithValue("model", car.Model ?? Convert.DBNull); // hva betyr ?? gjør slik at en verdi kan ha null
                                command.Connection = conn;
                                command.CommandText = stmt;
                                command.Prepare();
                                int result = (int)command.ExecuteScalar();
                                command.Parameters.Clear();
                            }
                        }
                        trans.Commit();
                    }
                    catch (PostgresException)
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }
        #endregion
        #region READ

        public static void GetObservation(bool withOwners = false)
        {

        }
        public static Observation GetObservationWithobserver(int id)
        {
            //EF entity framework



            string stmt = "select id, date,observer_id,geolocation_id from obeservation where id=@id";
            // stmt = "SELECT c.id as \"car.id\", c.model,c.make, p.id as \"person.id\", p.firstname, p.lastname FROM car c INNER JOIN person p on p.car_id = c.id  WHERE c.id = @id";
            stmt = "SELECT observation.id as \"observation.id\", observation.date,observation.observer_id, observer.id as \"observer.id\", observer.firstname, observer.lastname FROM observer observer INNER JOIN observation observation on observation.observer_id = observation.id  WHERE observation.id = @id";

            using (var conn = new NpgsqlConnection(connectionString))
            {
                Observation observation = null;
                conn.Open();

                using (var command = new NpgsqlCommand(stmt, conn))
                {
                    command.Parameters.AddWithValue("id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        reader.Read();
                        observation = new Observation
                        {
                            Id = (int)reader["observation.id"],
                            Date = (DateTime)reader["date"], // han har brukt store bokstaver
                            Observer_id = (int)reader["observer_id"],
                            Geolocation_id = (int)reader["observer_id"]




                        };
                    }
                }
                return observation;
            }
        }
        public static IEnumerable<Observation> GetObservations(int? observationId = null)
        {
            string stmt = "select id, date, observer_id,geolocation_id from observation";
            if (observationId.HasValue)
            {
                stmt += " WHERE observer_id=@observerId";
            }

            using (var conn = new NpgsqlConnection(connectionString))
            {
                Observation observation = null;
                List<Observation> observations = new List<Observation>();
                conn.Open();

                using (var command = new NpgsqlCommand(stmt, conn))
                {
                    if (observationId.HasValue)
                    {
                        command.Parameters.AddWithValue("observerId", observationId.Value);
                    }

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            observation = new Observation
                            {
                                Id = (int)reader["id"],
                                Date = (DateTime)reader["date"], // han har brukt store bokstaver
                                Observer_id = (int)reader["observer_id"],
                                Geolocation_id = (int)reader["geolocation_id"]
                            };
                            observations.Add(observation);
                        }
                    }
                }
                return observations;
            }
        }

        #endregion
        #region UPDATE
        public static int SaveObservation(Observation observation)
        {
            string stmt = "UPDATE observation set date = @date, observer_id= @observer_id, geolocation_id= @geolocation_id where id=@id";

            using (var conn = new NpgsqlConnection(connectionString))
            {
                using (var command = new NpgsqlCommand(stmt, conn))
                {
                    conn.Open();
                    command.Parameters.AddWithValue("date", observation.Date);
                    command.Parameters.AddWithValue("observer_id", observation.Observer_id);
                    command.Parameters.AddWithValue("geolocation", observation.Geolocation_id);
                    command.Parameters.AddWithValue("id", observation.Id);
                    return command.ExecuteNonQuery();
                }
            }
        }
        #endregion
        #region DELETE
        public static void DeleteObservation(int id)
        {
            string stmt = "DELETE FROM observation WHERE id = @id";
            using (var conn = new NpgsqlConnection(connectionString))
            {
                using (var command = new NpgsqlCommand(stmt, conn))
                {
                    conn.Open();
                    command.Parameters.AddWithValue("id", id);
                    command.ExecuteScalar();
                }
            }
        }

        public static void Delete(object poco)
        {

        }
        #endregion
    }
}

