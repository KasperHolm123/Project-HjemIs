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
        public static void AddData(Location loc)
        {
            try
            {
                connection.Open();
                string query = "INSERT INTO Locations (PostNr, Kommunekode, Vejkode, Bynavn, Vejnavn)" +
                               "VALUES (@postnr, @komkod, @vejkod, @bynavn, @vejnavn);"; // parametre er allerede strings, så der er ingen grund til at skrive '' ved dem.
                SqlCommand command = new SqlCommand(query, connection);

                
                
                command.ExecuteReader();

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

        private static void FitParameters(SqlCommand command, Location loc)
        {
            command.Parameters.Add(CreateParameter("@recordtype", loc.PostNr, SqlDbType.NVarChar));
            command.Parameters.Add(CreateParameter("@komkod", loc.Kommunekode, SqlDbType.NVarChar));
            command.Parameters.Add(CreateParameter("@vejkod", loc.Vejkode, SqlDbType.NVarChar));
            command.Parameters.Add(CreateParameter("@bynavn", loc.Vejkode, SqlDbType.NVarChar));
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
