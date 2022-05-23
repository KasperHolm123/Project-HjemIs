﻿using System;
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
        /// Send a small amount of data to a database.
        /// </summary>
        void AddData(string query, SqlParameter[] parameters);

        /// <summary>
        /// Gets a table from a database and returns it.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        List<T> GetTable<T>(string query);

        /// <summary>
        /// Clears a table in a database.
        /// </summary>
        /// <param name="tableName"></param>
        void ClearTable(string tableName);

    }
}
