using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Projekt_HjemIS.Models;

namespace Projekt_HjemIS.Systems
{
    public static class DatabaseHandler_Old
    {
        private static SqlConnection connection = new SqlConnection
            (ConfigurationManager.ConnectionStrings["post"].ConnectionString);

        public static List<Location> Locations { get; set; }
        /// <summary>
        /// Execute SQL query to add data to designated database.
        /// </summary>
        /// <param name="record"></param>
        public static void AddData(IEnumerable<Location> locList)
        {
            try
            {
                connection.Open();
                ClearTables();
                string query = "INSERT INTO Locations (StreetCode, CountyCode, Street, PostalCode, City, PostalDistrict)" +
                               "VALUES (@streetcode, @countycode, @street, @postalcode, @city, @postaldistrict)"; // parametre er allerede strings, så der er ingen grund til at skrive '' ved dem.

                foreach (var item in locList)
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.Add(CreateParameter("@streetcode", item.StreetCode, SqlDbType.NVarChar));
                    command.Parameters.Add(CreateParameter("@countycode", item.CountyCode, SqlDbType.NVarChar));
                    command.Parameters.Add(CreateParameter("@street", DbValueExtensions.AsDbValue(item.Street), SqlDbType.NVarChar));
                    command.Parameters.Add(CreateParameter("@postalcode", DbValueExtensions.AsDbValue(item.PostalCode), SqlDbType.NVarChar));
                    command.Parameters.Add(CreateParameter("@city", DbValueExtensions.AsDbValue(item.City), SqlDbType.NVarChar));
                    command.Parameters.Add(CreateParameter("@postaldistrict", DbValueExtensions.AsDbValue(item.PostalDistrict), SqlDbType.NVarChar));
                    command.ExecuteNonQuery();
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + "\n");
            }
            finally
            {
                connection.Close();
            }
        }

        public static string AddBulkData(DataTable dt)
        {
            connection.Open();
            ClearTables();
            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
            {
                bulkCopy.DestinationTableName = "Locations";
                PropertyInfo[] allProperties = typeof(Location).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                List<PropertyInfo> properties = new List<PropertyInfo>();
                foreach (PropertyInfo item in allProperties)
                {
                    if (item.PropertyType == typeof(String))
                        properties.Add(item);
                }
                foreach (PropertyInfo property in properties)
                {
                    bulkCopy.ColumnMappings.Add($"{property.Name}", $"{property.Name}");
                }

                try { bulkCopy.WriteToServer(dt); return "Saving successful"; }
                catch (Exception ex) { return ex.Message; }
                finally { connection.Close(); }
            }
        }

        /// <summary>
        /// Saves any type of message in a database.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <returns></returns>
        public static int SaveMessage<T>(T message) where T : Message
        {
            try
            {
                connection.Open();
                if (typeof(T) == typeof(Message_Mail))
                {
                    Message_Mail mail = message as Message_Mail;
                    string query = "INSERT INTO [Messages] ([Type], [Subject], Body) OUTPUT Inserted.ID " +
                        "VALUES (@messageType, @subject, @body)";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.Add(CreateParameter("@messageType", mail.Type, SqlDbType.NVarChar));
                    command.Parameters.Add(CreateParameter("@subject", mail.Subject, SqlDbType.NVarChar));
                    command.Parameters.Add(CreateParameter("@body", mail.Body, SqlDbType.NVarChar));
                    return (int)command.ExecuteScalar();
                }
                if (typeof(T) == typeof(Message_SMS))
                {
                    Message_SMS sms = message as Message_SMS;
                    string query = "INSERT INTO [Messages] ([Type], Body) OUTPUT Inserted.ID " + 
                        "VALUES(@messageType, @body)";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.Add(CreateParameter("@messageType", sms.Type, SqlDbType.NVarChar));
                    command.Parameters.Add(CreateParameter("@body", sms.Body, SqlDbType.NVarChar));
                    return (int)command.ExecuteScalar();
                }
                return -1;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return -1;
            }
            finally { connection.Close(); }
        }

        public static void ConnectMessage(int ID, string countyCode, string streetCode)
        {
            try
            {
                connection.Open();
                string query = "INSERT INTO [Customers-Messages] (ID, PhoneNumber, [Date]) " +
                    "SELECT @messageID, PhoneNumber, GETDATE() FROM Customers WHERE " +
                    "CountyCode = @countyCode AND StreetCode = @streetCode;";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.Add(CreateParameter("@messageID", ID, SqlDbType.Int));
                cmd.Parameters.Add(CreateParameter("@countyCode", countyCode, SqlDbType.NVarChar));
                cmd.Parameters.Add(CreateParameter("@streetCode", streetCode, SqlDbType.NVarChar));
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex) { Debug.WriteLine(ex.Message); }
            finally { connection.Close(); }
        }

        //public static void GetLocation(ObservableCollection<Location> collection, string streetName)
        //{
        //    try
        //    {
        //        connection.Open();
        //        string query = $"SELECT TOP (100) * FROM Locations WHERE Street LIKE '%{streetName}%';";
        //        SqlCommand command = new SqlCommand(query, connection);
        //        int streetAmount = 0;
        //        using (SqlDataReader reader = command.ExecuteReader())
        //        {
        //            while (reader.Read())
        //            {
        //                Location newLocation = new Location(
        //                    (string)reader[$"{nameof(Location.StreetCode)}"],
        //                    (string)reader[$"{nameof(Location.CountyCode)}"],
        //                    (string)reader[$"{nameof(Location.Street)}"],
        //                    (string)reader[$"{nameof(Location.PostalCode)}"],
        //                    (string)reader[$"{nameof(Location.City)}"],
        //                    (string)reader[$"{nameof(Location.PostalDistrict)}"]
        //                    );
        //                if (!collection.Contains(newLocation))
        //                    collection.Add(newLocation);
        //                streetAmount++;
        //            }
        //        }
        //        Debug.WriteLine($"Returned: {streetAmount} streets");
        //    }
        //    catch (Exception e)
        //    {
        //        Debug.WriteLine(e.Message);
        //    }
        //    finally { connection.Close(); }
        //}

        /// <summary>
        /// Get all Customers from database and return them as an ObservableCollection.
        /// </summary>
        /// <returns></returns>
        public static ObservableCollection<Customer> GetCustomers()
        {
            ObservableCollection<Customer> InternalCustomers = new ObservableCollection<Customer>();
            try
            {
                connection.Open();
                string query = $@"SELECT * FROM Customers";
                SqlCommand command = new SqlCommand(query, connection);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Using the nameof keyword becuase Customer models a database table with the same name
                        // This makes it so we don't accidentally make a typo and spend 5 hours
                        // trying to figure out why it doesn't work.
                        InternalCustomers.Add(new Customer(
                            (string)reader[$"{nameof(Customer.FirstName)}"],
                            (string)reader[$"{nameof(Customer.LastName)}"],
                            (int)reader[$"{nameof(Customer.PhoneNumber)}"],
                            (string)reader[$"{nameof(Customer.StreetCode)}"],
                            (string)reader[$"{nameof(Customer.CountyCode)}"]
                            ));
                    }
                }
                return InternalCustomers;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                connection.Close();
            }
            return null;
        }

        public static ObservableCollection<User> GetUsers()
        {
            ObservableCollection<User> InternalUsers = new ObservableCollection<User>();
            try
            {
                connection.Open();
                string query = $@"SELECT [username] FROM Users";
                SqlCommand command = new SqlCommand(query, connection);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        //InternalUsers.Add(new User(
                        //    (string)reader[$"{nameof(User.Username)}"]));
                    }

                }
                return InternalUsers;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                connection.Close();
            }
            return null;
        }

        /// <summary>
        /// Clear database tables to make room for new data.
        /// </summary>
        public static void ClearTables()
        {
            string query = "DELETE FROM Locations;";
            try
            {
                SqlCommand command = new SqlCommand(@query, connection);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Create parameter for SQL command.
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private static SqlParameter CreateParameter(string paramName, object value, SqlDbType type)
        {
            SqlParameter param = new SqlParameter
            {
                ParameterName = paramName,
                Value = value,
                SqlDbType = type
            };
            return param;
        }
        public static void GetCities()
        {
            List<Location> locs = new List<Location>();
            try
            {
                connection.Open();
                string query = $@"SELECT DISTINCT PostalCode, City FROM Locations WHERE PostalCode LIKE '%%' AND City LIKE '%%'";
                SqlCommand command = new SqlCommand(query, connection);
                command.CommandTimeout = 0;
                //command.Parameters.Add(CreateParameter("@postalcode", loc.PostalCode, SqlDbType.NVarChar));
                //command.Parameters.Add(CreateParameter("@city", loc.City, SqlDbType.NVarChar));
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        locs.Add(new Location()
                        {
                            City = (string)reader[$"{nameof(Location.City)}"].ToString().Trim(),
                            PostalCode = (string)reader[$"{nameof(Location.PostalCode)}"]
                        });
                    }
                }
                Locations = locs;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }
    }
    public static class DbValueExtensions
    {
        // Used to convert values coming from the db
        public static T As<T>(this object source)
        {
            return source == null || source == DBNull.Value
                ? default(T)
                : (T)source;
        }

        // Used to convert values going to the db
        public static object AsDbValue(this object source)
        {
            return source ?? DBNull.Value;
        }
    }
}
