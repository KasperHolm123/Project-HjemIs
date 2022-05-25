using Projekt_HjemIS.Models;
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

namespace Projekt_HjemIS.Systems.Utility.Database_handling
{
    public class DatabaseHandler : IDatabase
    {
        private static SqlConnection connection = new SqlConnection
            (ConfigurationManager.ConnectionStrings["post"].ConnectionString);

        #region IDatabase Implementation
        public void ClearTable(string tableName)
        {
            string query = $"DELETE FROM {tableName};";
            try
            {
                SqlCommand command = new SqlCommand(@query, connection);
                command.ExecuteNonQuery();
            }
            catch (Exception ex) { Debug.WriteLine(ex.Message); }
        }

        public List<T> GetTable<T>(string query)
        {
            PropertyInfo[] AllProps = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance); // Get all the properties
            List<PropertyInfo> Props = new List<PropertyInfo>();
            foreach (PropertyInfo prop in AllProps)
            {
                if (prop.PropertyType == typeof(String) || prop.PropertyType == typeof(int))
                    Props.Add(prop);
            }
            try
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand(query, connection);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    object[] values = new object[Props.Count];
                    // Eventuel forbedring ville være at finde en måde at bruge T i et switch statement, samt en måde at "new T()" på.
                    if (typeof(T) == typeof(Customer))
                    {
                        var internalTable = new List<Customer>();
                        while (reader.Read())
                        {
                            reader.GetValues(values);
                            internalTable.Add(new Customer(values));
                        }
                        return internalTable as List<T>;
                    }
                    else if (typeof(T) == typeof(Location))
                    {
                        var internalTable = new List<Location>();
                        while (reader.Read())
                        {
                            reader.GetValues(values);
                            Location location = new Location(values);

                            if (!internalTable.Contains(location))
                                internalTable.Add(location);
                        }
                        return internalTable as List<T>;
                    }
                    else if (typeof(T) == typeof(User))
                    {
                        var internalTable = new List<User>();
                        while (reader.Read())
                        {
                            reader.GetValues(values);
                            internalTable.Add(new User(values)); // mangler constructor
                        }
                    }
                }
            }
            catch (Exception ex) { Debug.WriteLine(ex.Message); }
            finally { connection.Close(); }
            return null;
        }

        public void AddData(string query, SqlParameter[] parameters)
        {
            try
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddRange(parameters);
                cmd.ExecuteNonQuery();

            }
            catch (Exception ex) { Debug.WriteLine(ex.Message); }
            finally { connection.Close(); }
        }
        #endregion

        /// <summary>
        /// Add data to specified database and return an int(ID).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public int AddDataReturn<T>(string query, SqlParameter[] parameters)
        {
            try
            {
                connection.Open();
                if (typeof(T) == typeof(Message_Mail))
                {
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddRange(parameters);
                    return (int)cmd.ExecuteScalar();
                }
                if (typeof(T) == typeof(Message_SMS))
                {   
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddRange(parameters);
                    return (int)cmd.ExecuteScalar();
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

        /// <summary>
        /// Adds a large amount of data to a database.
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public string AddBulkData<T>(DataTable dt, string tableName)
        {
            connection.Open();
            ClearTable($"{tableName}");
            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
            {
                bulkCopy.DestinationTableName = $"{tableName}";
                PropertyInfo[] allProperties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                List<PropertyInfo> properties = new List<PropertyInfo>();
                foreach (PropertyInfo item in allProperties)
                {
                    if (item.PropertyType == typeof(string))
                        properties.Add(item);
                }
                foreach (PropertyInfo property in properties)
                    bulkCopy.ColumnMappings.Add($"{property.Name}", $"{property.Name}");
                try { bulkCopy.WriteToServer(dt); return "Saving successful"; }
                catch (Exception ex) { return ex.Message; }
                finally { connection.Close(); }
            }
        }
        public int UpdateBulkData(DataTable dt)
        {
            int affected = -1;
            dt.PrimaryKey = new DataColumn[] { dt.Columns["StreetCode"], dt.Columns["CountyCode"] };
            string selectQuery = "SELECT * FROM Locations";
            try
            {
                connection.Open();
                DataSet dbdataSet = new DataSet("Locations");
                DataTable table = new DataTable("Locations");
                SqlDataAdapter adapter = new SqlDataAdapter(selectQuery, connection);
                SqlCommandBuilder cb = new SqlCommandBuilder(adapter);
                adapter.FillSchema(table, SchemaType.Source);
                adapter.Fill(table);
                adapter.SelectCommand = new SqlCommand(selectQuery, connection);
                adapter.DeleteCommand = cb.GetDeleteCommand(true);
                adapter.UpdateCommand = cb.GetUpdateCommand(true);
                adapter.InsertCommand = cb.GetInsertCommand(true);
                table.Merge(dt, false, MissingSchemaAction.Error);
                Debug.WriteLine($"Merged");
                adapter.AcceptChangesDuringUpdate = true;
                affected = adapter.Update(table);
                Debug.WriteLine($"Update completed with {affected} rows");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                connection.Close();
            }
            return affected;
        }
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
