using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
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
                bulkCopy.ColumnMappings.Add("StreetCode", "StreetCode");
                bulkCopy.ColumnMappings.Add("CountyCode", "CountyCode");
                bulkCopy.ColumnMappings.Add("Street", "Street");
                bulkCopy.ColumnMappings.Add("PostalCode", "PostalCode");
                bulkCopy.ColumnMappings.Add("City", "City");
                bulkCopy.ColumnMappings.Add("PostalDistrict", "PostalDistrict");

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
