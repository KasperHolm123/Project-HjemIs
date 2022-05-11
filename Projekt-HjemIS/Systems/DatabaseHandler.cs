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
    public static class DatabaseHandler
    {
        private static SqlConnection connection = new SqlConnection
            (ConfigurationManager.ConnectionStrings["post"].ConnectionString);


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

        public static void AddBulkData(DataTable dt)
        {
            connection.Open();
            ClearTables();
            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
            {
                bulkCopy.DestinationTableName = "Locations";
                
                PropertyInfo[] properties = typeof(Location).GetProperties(BindingFlags.Public | BindingFlags.Instance);

                foreach (PropertyInfo property in properties)
                {
                    bulkCopy.ColumnMappings.Add($"{property.Name}", $"{property.Name}");
                }
                //bulkCopy.ColumnMappings.Add($"{nameof(Location.StreetCode)}", $"{nameof(Location.StreetCode)}");
                //bulkCopy.ColumnMappings.Add($"{nameof(Location.CountyCode)}", $"{nameof(Location.CountyCode)}");
                //bulkCopy.ColumnMappings.Add($"{nameof(Location.Street)}", $"{nameof(Location.Street)}");
                //bulkCopy.ColumnMappings.Add($"{nameof(Location.PostalCode)}", $"{nameof(Location.PostalCode)}");
                //bulkCopy.ColumnMappings.Add($"{nameof(Location.City)}", $"{nameof(Location.City)}");
                //bulkCopy.ColumnMappings.Add($"{nameof(Location.PostalDistrict)}", $"{nameof(Location.PostalDistrict)}");

                try
                {
                    bulkCopy.WriteToServer(dt);
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
                string query = $@"SELECT * FROM {nameof(Customer)}";
                SqlCommand command = new SqlCommand(query, connection);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Using the nameof keyword becuase Customer models a database table with the same name
                        // This makes it so we don't accidentally make a typo and spend 5 hours
                        // trying to figure out why it doesn't work.
                        InternalCustomers.Add(new Customer(
                            (string)reader[$"{nameof(Customer.StreetCode)}"],
                            (string)reader[$"{nameof(Customer.CountyCode)}"],
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
