using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public static void AddData(string[] record)
        {
            try
            {
                connection.Open();
                string query = "INSERT INTO Locations (RecordType, Kommunekode, Vejkode)" +
                               "VALUES (@recordtype, @komkod, @vejkod);"; // parametre er allerede strings, så der er ingen grund til at skrive '' ved dem.
                SqlCommand command = new SqlCommand(query, connection);
                switch (record[0])
                {
                    case "001":
                        FitParameters(command, record, 1, 2);
                        break;
                    case "002":
                        FitParameters(command, record, 1, 2);
                        break;
                    case "003":
                        FitParameters(command, record, 1, 2);
                        break;
                    case "004":
                        FitParameters(command, record, 1, 2);
                        break;
                    case "005":
                        FitParameters(command, record, 1, 2);
                        break;
                    case "006":
                        FitParameters(command, record, 1, 2);
                        break;
                    case "007":
                        FitParameters(command, record, 1, 2);
                        break;
                    case "008":
                        FitParameters(command, record, 1, 2);
                        break;
                    case "009":
                        FitParameters(command, record, 1, 2);
                        break;
                    case "010":
                        FitParameters(command, record, 1, 2);
                        break;
                    case "011":
                        FitParameters(command, record, 1, 2);
                        break;
                    case "012":
                        FitParameters(command, record, 1, 2);
                        break;
                    case "013":
                        FitParameters(command, record, 1, 2);
                        break;
                    case "014":
                        FitParameters(command, record, 1, 2);
                        break;
                    case "015":
                        FitParameters(command, record, 1, 2);
                        break;
                }
                
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

        private static void FitParameters(SqlCommand command, string[] record, int komKod, int vejKod)
        {
            command.Parameters.Add(CreateParameter("@recordtype", record[0], SqlDbType.NVarChar));
            command.Parameters.Add(CreateParameter("@komkod", record[komKod], SqlDbType.NVarChar));
            command.Parameters.Add(CreateParameter("@vejkod", record[vejKod], SqlDbType.NVarChar));
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
