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
    public static class ListToDataTableConverter
    {
        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            // typeof(T) works by checking the type of the passed-in List<T>
            PropertyInfo[] AllProps = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance); // Get all the properties
            List<PropertyInfo> Props = new List<PropertyInfo>();
            foreach (PropertyInfo prop in AllProps)
            {
                if (prop.PropertyType == typeof(String))
                    Props.Add(prop);
            }

            foreach (PropertyInfo prop in Props)
            {
                // Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }

            foreach (T item in items)
            {
                // Values is an object array because it resembles each row in the DataTable/item
                var values = new object[Props.Count];
                for (int i = 0; i < Props.Count; i++)
                {
                    // Inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item);
                }
                dataTable.Rows.Add(values);
            }
            return dataTable;
        }
    }
}
