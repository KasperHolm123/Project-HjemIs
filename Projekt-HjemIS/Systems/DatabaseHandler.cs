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
        public static void AddData(List<Location> locList)
        {
            try
            {
                connection.Open();

                string query = "INSERT INTO Locations (StreetCode, CountyCode, Street, PostalCode, PostalDistrict)" +
                               "VALUES (@streetcode, @countycode, @street, @postalcode, @postaldistrict)"; // parametre er allerede strings, så der er ingen grund til at skrive '' ved dem.

                foreach (var item in locList)
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.Add(CreateParameter("@streetcode", item.Vejkode, SqlDbType.NVarChar));
                    command.Parameters.Add(CreateParameter("@countycode", item.Kommunekode, SqlDbType.NVarChar));
                    command.Parameters.Add(CreateParameter("@street", item.VejNavn, SqlDbType.NVarChar));
                    command.Parameters.Add(CreateParameter("@postalcode", item.PostNr, SqlDbType.NVarChar));
                    //command.Parameters.Add(CreateParameter("@city", loc.Bynavn, SqlDbType.NVarChar));
                    command.Parameters.Add(CreateParameter("@postaldistrict", item.Postdistrikt, SqlDbType.NVarChar));
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

        /// <summary>
        /// Clear database tables to make room for new data.
        /// </summary>
        public static void ClearTables()
        {
            string query = "DELETE FROM Locations;";
            try
            {
                connection.Open();
                SqlCommand command = new SqlCommand(@query, connection);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {

            }
            finally
            {
                connection.Close();
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
}
