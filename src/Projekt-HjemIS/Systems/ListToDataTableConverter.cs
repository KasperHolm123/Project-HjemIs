using Projekt_HjemIS.Systems.Utility.Database_handling;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Projekt_HjemIS.Systems
{
    /// <summary>
    /// Hovedforfatter: Jonas
    /// </summary>
    public static class ListToDataTableConverter
    {
        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            var properties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => p.PropertyType == typeof(string));

            foreach ( var property in properties )
            {
                dataTable.Columns.Add(property.Name);
            }

            foreach (T item in items)
            {
                var row = dataTable.NewRow();
                
                foreach ( var property in properties )
                {
                    row[property.Name] = property.GetValue(item);
                }

                dataTable.Rows.Add(row);
            }
            return dataTable;
        }
    }
}
