using ClimateObservations.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace ClimateObservations.Repositories
{
    class GeolocationRepository
    {
        private static string connectionString = ConfigurationManager.ConnectionStrings["dbLocal"].ConnectionString;


        // CRUD
        #region CREATE
        public static int AddGeolocation(Geolocation geolocation)
        {
            string stmt = "INSERT INTO geolocation( lattitude,longitude, area_id) values(@lattitude,@longitude, @area_id) returning id";

            using (var conn = new NpgsqlConnection(connectionString))
            {
                using (var command = new NpgsqlCommand(stmt, conn))
                {
                    conn.Open();
                    command.Parameters.AddWithValue("latitude", geolocation.Latitude);
                    command.Parameters.AddWithValue("longitude", geolocation.Longitude);
                    command.Parameters.AddWithValue("area_id", geolocation.Area_id);
                    int id = (int)command.ExecuteScalar();
                    geolocation.Id = id;
                    return id;
                }
            }
        }

        public static void AddObservation(List<Geolocation> geolocations)
        {
            string stmt = "INSERT INTO geolocation( lattitude,longitude, area_id) values(@lattitude,@longitude, @area_id) returning id";

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        using (var command = new NpgsqlCommand())
                        {
                            foreach (var geolocation in geolocations)
                            {
                                command.Parameters.AddWithValue("latitude", geolocation.Latitude);
                                command.Parameters.AddWithValue("longitude", geolocation.Longitude);
                                command.Parameters.AddWithValue("area_id", geolocation.Area_id);


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

        public static void GetObservation(bool withObserver = false)
        {

        }
        public static Geolocation GetGeolocationWithArea(int id)
        {
            //EF entity framework



            string stmt = "select id, latitude,longitude,area_id from geolocation where id=@id";
            // stmt = "SELECT c.id as \"car.id\", c.model,c.make, p.id as \"person.id\", p.firstname, p.lastname FROM car c INNER JOIN person p on p.car_id = c.id  WHERE c.id = @id";
            stmt = "SELECT observation.id as \"observation.id\", observation.date,observation.observer_id, observer.id as \"observer.id\", observer.firstname, observer.lastname FROM observer observer INNER JOIN observation observation on observation.observer_id = observation.id  WHERE observation.id = @id";

            using (var conn = new NpgsqlConnection(connectionString))
            {
                Geolocation geolocation = null;
                conn.Open();

                using (var command = new NpgsqlCommand(stmt, conn))
                {
                    command.Parameters.AddWithValue("id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        reader.Read();
                        geolocation = new Geolocation
                        {
                            Id = (int)reader["id"],
                            Latitude = (float)reader["latitude"],
                            Longitude = (float)reader["longitude"],
                            Area_id = (int)reader["area_id"]
                        };
                    }
                }
                return geolocation;
            }
        }
        public static IEnumerable<Geolocation> GetGelocations()
        {
            string stmt = "select id, latitude, longitude, area_id from geolocation";

            using (var conn = new NpgsqlConnection(connectionString))
            {
                Geolocation geolocation = null;
                List<Geolocation> geolocations = new List<Geolocation>();
                conn.Open();

                using (var command = new NpgsqlCommand(stmt, conn))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            geolocation = new Geolocation
                            {
                                Id = (int)reader["id"],
                                Latitude = (double?)reader["latitude"],
                                Longitude = (double?)reader["longitude"],
                                Area_id = (int)reader["area_id"]


                            };
                            geolocations.Add(geolocation);
                        }
                    }
                }
                return geolocations;
            }
        }

        #endregion
        #region UPDATE
        public static int SaveObservation(Observation observation)
        {
            string stmt = "UPDATE geolocation set latitude = @latitude, longitude= @longitude, area_id= @area_id where id=@id";

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
            string stmt = "DELETE FROM geolocation WHERE id = @id";
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


