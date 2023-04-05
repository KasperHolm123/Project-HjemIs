using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt_HjemIS.Systems.Utility.Database_handling
{
    public interface IDatabase
    {
        /// <summary>
        /// Hovedforfatter: Kasper
        /// Send a small amount of data to a database.
        /// </summary>
        int AddData(string query, SqlParameter[] parameters);

        /// <summary>
        /// Method for fetching a single entry.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <returns>Object of type T</returns>
        Task<T> GetEntry<T>(string query);

        /// <summary>
        /// Hovedforfatter: Kasper
        /// Gets a table from a database and returns it.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        List<T> GetTable<T>(string query);

        /// <summary>
        /// Hovedforfatter: Kasper
        /// Clears a table in a database.
        /// </summary>
        /// <param name="tableName"></param>
        void ClearTable(string tableName);

        /// <summary>
        /// Checks whether or not a specific record exists based on a record's primary key
        /// </summary>
        /// <returns> Greater than 0 if an element with specified ID exists</returns>
        Task<bool> ExistsAsync(string query, List<SqlParameter> parameters);

        /// <summary>
        /// Checks whether or not a specific record exists based on a record's primary key
        /// </summary>
        /// <returns> Greater than 0 if an element with specified ID exists</returns>
        Task<bool> ExistsAsync(string query);

    }
}
