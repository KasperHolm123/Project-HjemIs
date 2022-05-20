using Projekt_HjemIS.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt_HjemIS.Systems.Utility.Database_handling
{
    public class MessageManager : IDatabase
    {
        private static SqlConnection connection = new SqlConnection
            (ConfigurationManager.ConnectionStrings["post"].ConnectionString);

        public MessageManager()
        {
            
        }

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
            throw new NotImplementedException();
        }

        public void AddData(string query, SqlParameter[] parameters, string[] paramSource)
        {
            throw new NotImplementedException();
        } 
        #endregion

        public int AddData<T>(T message) where T : Message
        {
            try
            {
                connection.Open();
                if (typeof(T) == typeof(Message_Mail))
                {
                    Message_Mail mail = message as Message_Mail;
                    string query = "INSERT INTO [Messages] ([Type], [Subject], Body) OUTPUT Inserted.ID " +
                        "VALUES (@messageType, @subject, @body)";
                    SqlCommand command = new SqlCommand(query, connection);
                    //command.Parameters.Add(IDatabase.CreateParameter("@messageType", mail.Type, SqlDbType.NVarChar));
                    //command.Parameters.Add(IDatabase.CreateParameter("@subject", mail.Subject, SqlDbType.NVarChar));
                    //command.Parameters.Add(IDatabase.CreateParameter("@body", mail.MessageBody, SqlDbType.NVarChar));
                    return (int)command.ExecuteScalar();
                }
                if (typeof(T) == typeof(Message_SMS))
                {
                    Message_SMS sms = message as Message_SMS;
                    string query = "INSERT INTO [Messages] ([Type], Body) OUTPUT Inserted.ID " +
                        "VALUES(@messageType, @body)";
                    SqlCommand command = new SqlCommand(query, connection);
                    //command.Parameters.Add(CreateParameter("@messageType", sms.Type, SqlDbType.NVarChar));
                    //command.Parameters.Add(CreateParameter("@body", sms.MessageBody, SqlDbType.NVarChar));
                    return (int)command.ExecuteScalar();
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
    }
}
