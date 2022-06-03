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
using System.Windows;

namespace Projekt_HjemIS.Systems.Utility.Database_handling
{
    /// <summary>
    /// Hovedforfatter: Kasper
    /// </summary>
    public class DatabaseHandler : IDatabase
    {
        private SqlConnection connection = new SqlConnection
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
                    var internalTable = new List<T>();
                    while (reader.Read())
                    {
                        reader.GetValues(values);
                        internalTable.Add((T)Activator.CreateInstance(typeof(T), values)); // Instantier T med argumenter.
                    }
                    return internalTable;
                }
            }
            catch (Exception ex) { Debug.WriteLine(ex.Message); }
            finally { connection.Close(); }
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public int AddData(string query, SqlParameter[] parameters)
        {
            try
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddRange(parameters);
                return cmd.ExecuteNonQuery();

            }
            catch (Exception ex) { Debug.WriteLine(ex.Message); return -1; }
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
        /// Hovedforfatter: Jonas H
        /// Checks if any rows are present in the specified table
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public async Task<object> CheckTable(string table)
        {
            string query = $"SELECT COUNT(*) FROM {table}";
            try
            {
                using (SqlConnection connection = new SqlConnection
            (ConfigurationManager.ConnectionStrings["post"].ConnectionString))
                {
                    await connection.OpenAsync();
                    using(SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        return await cmd.ExecuteScalarAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                connection.Close();
            }
            return null;
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
                    if (item.PropertyType == typeof(string) || item.PropertyType == typeof(int))
                        properties.Add(item);
                }
                foreach (PropertyInfo property in properties)
                    bulkCopy.ColumnMappings.Add($"{property.Name}", $"{property.Name}");
                try 
                { 
                    bulkCopy.WriteToServer(dt);
                    return "Saving successful"; 
                }
                catch (Exception ex) { return ex.Message; }
                finally { connection.Close(); }
            }
        }
        /// <summary>
        /// Hovedforfatter: Jonas H
        /// Updates locations table by a merge
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public int UpdateBulkData(DataTable dt)
        {
            int affected = -1;

            string selectQuery = "SELECT * FROM Locations";
            try
            {
                using(SqlConnection connection = new SqlConnection
            (ConfigurationManager.ConnectionStrings["post"].ConnectionString))
                {
                    connection.Open();
                    DataTable table = new DataTable("Locations");
                    using (SqlDataAdapter adapter = new SqlDataAdapter(selectQuery, connection))
                    {
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
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                MessageBox.Show("Done");
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
