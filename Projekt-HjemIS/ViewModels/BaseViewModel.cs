using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Projekt_HjemIS.Systems.Utility.Database_handling;

namespace Projekt_HjemIS.ViewModels
{
    public class BaseViewModel
    {
        public static readonly DatabaseHandler dh = new DatabaseHandler();

        public static SqlParameter CreateParameter(string paramName, object value, SqlDbType type)
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
