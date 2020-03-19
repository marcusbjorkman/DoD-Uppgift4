using ClimateObservations.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace ClimateObservations.Repositories
{
    public static class ObserverRepository
    {
        private static string connectionString = ConfigurationManager.ConnectionStrings["dbLocal"].ConnectionString;


        // CRUD
        #region CREATE
        public static int AddObserver(Observer observer)
        {
            string stmt = "INSERT INTO observer(firstname, lastname) values(@lastname,@firstname) returning id";

            using (var conn = new NpgsqlConnection(connectionString))
            {
                using (var command = new NpgsqlCommand(stmt, conn))
                {
                    conn.Open();
                    command.Parameters.AddWithValue("firstname", observer.Firstname);
                    command.Parameters.AddWithValue("lastname", observer.Lastname);
                   
                    int id = (int)command.ExecuteScalar();
                    observer.Id = id;
                    return id;
                }
            }
        }

        public static void AddObservers(List<Observer> observers)
        {
            string stmt = "INSERT INTO observer(firstname, lastname) values(@lastname,@firstname) returning id";

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        using (var command = new NpgsqlCommand())
                        {
                            foreach (var observer in observers)
                            {
                                command.Parameters.AddWithValue("firstname", observer.Firstname);
                                command.Parameters.AddWithValue("lastname", observer.Lastname);


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

       /*
        public static void GetObservation(bool withOwners = false)
        {

        }
        */
       /* public static Observation GetObservationWithobserver(int id)
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
                        while (reader.Read())
                        {
                            observation = new Observation
                            {
                                Id = (int)reader["observation.id"],
                                Date = (DateTime)reader["date"], // han har brukt store bokstaver
                                Observer_id = (int)reader["observer_id"],
                                Geolocation_id = (int)reader["observer_id"]




                            };
                            observation.Observer.Add(new Observer
                            {
                                Id = (int)reader["observer.id"],
                                Firstname = (string)reader["firstname"]
                            });
                        }
                    }
                }
                return observation;
            }
        }
        */
        public static IEnumerable<Observer> GetObservers()
        {
            string stmt = "select id, firstname,lastname from observer";

            using (var conn = new NpgsqlConnection(connectionString))
            {
                Observer observer = null;
                List<Observer> observers = new List<Observer>();
                conn.Open();

                using (var command = new NpgsqlCommand(stmt, conn))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            observer = new Observer
                            {
                                Id = (int)reader["id"],
                                Firstname = (string)reader["firstname"], 
                                Lastname = (string)reader["lastname"]
                            };
                            observers.Add(observer);
                        }
                    }
                }
                return observers;
            }
        }

        #endregion
        #region UPDATE
        public static int SaveObserver(Observer observer)
        {
            string stmt = "UPDATE observer set firstname = @firstname, lastname=@lastname where id=@id";

            using (var conn = new NpgsqlConnection(connectionString))
            {
                using (var command = new NpgsqlCommand(stmt, conn))
                {
                    conn.Open();
                    command.Parameters.AddWithValue("firstname", observer.Firstname);
                    command.Parameters.AddWithValue("lastname", observer.Lastname);
                    command.Parameters.AddWithValue("id", observer.Id);
                    return command.ExecuteNonQuery();
                }
            }
        }
        #endregion
        #region DELETE
        public static bool TryDeleteObserver(int id, out string errorCode)
        {
            string stmt = "DELETE FROM observer WHERE id = @id";
            using (var conn = new NpgsqlConnection(connectionString))
            {
                using (var command = new NpgsqlCommand(stmt, conn))
                {
                    conn.Open();
                    command.Parameters.AddWithValue("id", id);
                    try
                    {
                        command.ExecuteScalar();
                    }
                    catch(PostgresException e)
                    {
                        errorCode = e.SqlState;
                        return false;
                    }

                    // observer kan ikke slette hvis de har gjort en observasjon
                }
            }

            errorCode = null;
            return true;
        }

        public static void Delete(object poco)
        {

        }
        #endregion
    }
}


    

