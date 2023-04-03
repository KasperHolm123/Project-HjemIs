using Projekt_HjemIS.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
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
                using (connection)
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand(@query, connection);
                    
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public List<T> GetTable<T>(string query)
        {
            try
            {
                using (connection)
                {
                    connection.Open();

                    /* This line of code gets all relevant properties and selects them based on name.
                     * This ensure that we can use the GetOrdinal() method on an SqlDataReader object.
                     */
                    var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .Where(p => p.PropertyType == typeof(String) || p.PropertyType == typeof(int))
                        .Select(p => p.Name);

                    SqlCommand cmd = new SqlCommand(query, connection);
                
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        var internalTable = new List<T>();

                        while (reader.Read())
                        {
                            T obj = Activator.CreateInstance<T>();

                            foreach (var propertyName in properties)
                            {
                                int ordinal = reader.GetOrdinal(propertyName);
                            
                                object value = reader.IsDBNull(ordinal) ? null : reader.GetValue(ordinal);

                                typeof(T).GetProperty(propertyName).SetValue(obj, value);
                            }

                            internalTable.Add(obj);
                        }

                        return internalTable;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
        /// <summary>
        /// Adds data to connected database
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <returns>The amount of rows affected</returns>
        public int AddData(string query, SqlParameter[] parameters)
        {
            try
            {
                using (connection)
                {
                    connection.Open();

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddRange(parameters);

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {   
                return -1;
            }
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
                using (connection)
                {
                    connection.Open();

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddRange(parameters);

                        return (int)cmd.ExecuteScalar();
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);

                return -1;
            }
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
                using (connection)
                {
                    connection.Open();

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        return await cmd.ExecuteScalarAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                return null;
            }
        }
        /// <summary>
        /// Adds a large amount of data to a database.
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public string AddBulkData<T>(DataTable dt, string tableName)
        {
            using (connection)
            {
                connection.Open();

                ClearTable(tableName);
            
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                {
                    bulkCopy.DestinationTableName = tableName;
                
                    var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .Where(p => p.PropertyType == typeof(string) || p.PropertyType == typeof(int));

                    foreach (var property in properties)
                    {
                        bulkCopy.ColumnMappings.Add(property.Name, property.Name);
                    }

                    try 
                    { 
                        bulkCopy.WriteToServer(dt);

                        return "Saving successful"; 
                    }
                    catch (Exception ex)
                    {
                        return ex.Message;
                    }
                }
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
                using(connection)
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
                        
                        adapter.AcceptChangesDuringUpdate = true;
                        affected = adapter.Update(table);
                        
                    }
                }

                return affected;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                return -1;
            }
            finally
            {
                MessageBox.Show("Done");
            }
        }

        public async Task<bool> ExistsAsync(string query, List<SqlParameter> parameters)
        {
            using (connection)
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddRange(parameters.ToArray());
                    
                    var result = await command.ExecuteScalarAsync();

                    return (bool)(result ?? false);
                }
            }
        }

        public async Task<bool> ExistsAsync(string query)
        {
            using (connection)
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    var result = await command.ExecuteScalarAsync();

                    return (bool)(result ?? false);
                }
            }
        }
    }
}
