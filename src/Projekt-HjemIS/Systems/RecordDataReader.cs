using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Projekt_HjemIS.Systems
{
    /// <summary>
    /// Hovedforfatter: Jonas
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RecordDataReader<T>:IMyDataReader<T>
    {
        public List<T> Records { get; set; }
        int _currentIndex = -1;
        private readonly PropertyInfo[] _propertyInfos;
        private readonly Dictionary<int, string> _nameDictionary;
        private readonly SqlConnection _connection;
        private readonly string _schema = "dbo";
        private readonly string _tableName = "Locations";
        public int FieldCount => _propertyInfos.Length;


        public object GetValue(int i)
         => _propertyInfos
         .First(x => x.Name.Equals(_nameDictionary[i], StringComparison.OrdinalIgnoreCase))
         .GetValue(Records[_currentIndex]);
        public RecordDataReader(List<T> records, SqlConnection connection, string schema, string tableName)
        {
            Records = records;
            _propertyInfos = typeof(T).GetProperties();
            _nameDictionary = new Dictionary<int, string>();
            _connection = connection;
            _schema = schema;
            _tableName = tableName;
            DataTable schemaTable = GetSchemaTable();
            for (int x = 0; x < schemaTable?.Rows.Count; x++)
            {
                DataRow col = schemaTable.Rows[x];
                var columnName = col.Field<string>("ColumnName");
                _nameDictionary.Add(x, columnName);
            }
        }
        public bool Read()
        {
            if (_currentIndex + 1 >= Records.Count)
            {
                return false;
            }
            _currentIndex++;
            return true;
        }
        public DataTable GetSchemaTable()
        {
            using (var schemaCommand = new SqlCommand($"SELECT * FROM {_schema}.{_tableName}", _connection))
            {
                using (var reader = schemaCommand.ExecuteReader(CommandBehavior.SchemaOnly)) return reader.GetSchemaTable();
            }
        }





        int IDataReader.Depth => throw new NotImplementedException();

        bool IDataReader.IsClosed => throw new NotImplementedException();

        int IDataReader.RecordsAffected => throw new NotImplementedException();

        object IDataRecord.this[string name] => throw new NotImplementedException();

        object IDataRecord.this[int i] => throw new NotImplementedException();
        void IDataReader.Close()
        {
            throw new NotImplementedException();
        }

        bool IDataReader.NextResult()
        {
            throw new NotImplementedException();
        }

        void IDisposable.Dispose()
        {
            throw new NotImplementedException();
        }

        string IDataRecord.GetName(int i)
        {
            throw new NotImplementedException();
        }

        string IDataRecord.GetDataTypeName(int i)
        {
            throw new NotImplementedException();
        }

        Type IDataRecord.GetFieldType(int i)
        {
            throw new NotImplementedException();
        }

        int IDataRecord.GetValues(object[] values)
        {
            throw new NotImplementedException();
        }

        int IDataRecord.GetOrdinal(string name)
        {
            throw new NotImplementedException();
        }

        bool IDataRecord.GetBoolean(int i)
        {
            throw new NotImplementedException();
        }

        byte IDataRecord.GetByte(int i)
        {
            throw new NotImplementedException();
        }

        long IDataRecord.GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        char IDataRecord.GetChar(int i)
        {
            throw new NotImplementedException();
        }

        long IDataRecord.GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        Guid IDataRecord.GetGuid(int i)
        {
            throw new NotImplementedException();
        }

        short IDataRecord.GetInt16(int i)
        {
            throw new NotImplementedException();
        }

        int IDataRecord.GetInt32(int i)
        {
            throw new NotImplementedException();
        }

        long IDataRecord.GetInt64(int i)
        {
            throw new NotImplementedException();
        }

        float IDataRecord.GetFloat(int i)
        {
            throw new NotImplementedException();
        }

        double IDataRecord.GetDouble(int i)
        {
            throw new NotImplementedException();
        }

        string IDataRecord.GetString(int i)
        {
            throw new NotImplementedException();
        }

        decimal IDataRecord.GetDecimal(int i)
        {
            throw new NotImplementedException();
        }

        DateTime IDataRecord.GetDateTime(int i)
        {
            throw new NotImplementedException();
        }

        IDataReader IDataRecord.GetData(int i)
        {
            throw new NotImplementedException();
        }

        bool IDataRecord.IsDBNull(int i)
        {
            throw new NotImplementedException();
        }
    }

    public interface IMyDataReader<T> : IDataReader
    {
        List<T> Records { get; set; }
    }
}
