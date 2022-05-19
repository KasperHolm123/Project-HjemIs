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
    public class RecordManager : IDatabase
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

        public void GetTable()
        {
            throw new NotImplementedException();
        }

        public T ReturnTable<T>()
        {
            throw new NotImplementedException();
        }

        public void AddData()
        {
            throw new NotImplementedException();
        }

        SqlParameter IDatabase.CreateParameter(string paramName, object value, SqlDbType type)
        {
            SqlParameter param = new SqlParameter
            {
                ParameterName = paramName,
                Value = value,
                SqlDbType = type
            };
            return param;
        }
        #endregion

        public async Task AddBulkData(DataTable dt)
        {
            await connection.OpenAsync();
            ClearTable("Locations");
            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
            {
                bulkCopy.DestinationTableName = "Locations";
                PropertyInfo[] allProperties = typeof(Location).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                List<PropertyInfo> properties = new List<PropertyInfo>();
                foreach (PropertyInfo item in allProperties)
                {
                    if (item.PropertyType == typeof(string))
                        properties.Add(item);
                }
                foreach (PropertyInfo property in properties)
                    bulkCopy.ColumnMappings.Add($"{property.Name}", $"{property.Name}");
                try { bulkCopy.WriteToServer(dt); }
                catch (Exception ex) { Debug.WriteLine(ex.Message); }
                finally { connection.Close(); }
            }
        }

    }
}
