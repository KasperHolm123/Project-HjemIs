using Projekt_HjemIS.Models;
using Projekt_HjemIS.Systems.Utility.Database_handling;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt_HjemIS.Systems
{
    public static class MessageHandler
    {
        public static List<Message> Messages { get; set; }
        public static List<Customer> Customers { get; set; }

        private static string _rootPath = $@"\tempMessages\InternalMessages.txt";

        public static void GetCustomers()
        {
            DatabaseHandler dh = new DatabaseHandler();
            dh.GetTable<Customer>("SELECT * FROM Customers");
        }

        /// <summary>
        /// Writes the content of a message out into a .txt file. Send the message to DatabaseHandler. Returns an int.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        public static int SendMessages<T>(T message) where T : Message
        {
            DatabaseHandler dh = new DatabaseHandler();
            string products = string.Empty;
            if (message.Offers != null)
                foreach (Product item in message.Offers)
                    products += $"{item.Name}, ";
            using (StreamWriter sw = File.AppendText($@"{GetCurrentDirectory()}{_rootPath}"))
            {
                if (typeof(T) == typeof(Message_SMS))
                {
                    Message_SMS sms = message as Message_SMS;
                    string query = "INSERT INTO [Messages] ([Type], Body) OUTPUT Inserted.ID " +
                        "VALUES(@messageType, @body)";
                    SqlParameter[] sp = new SqlParameter[]
                    {
                        CreateParameter("@messageType", sms.Type, SqlDbType.NVarChar),
                        CreateParameter("@body", sms.MessageBody, SqlDbType.NVarChar)
                    };
                    sw.WriteLine(FormatSms(message as Message_SMS));
                    return dh.AddDataReturn<Message_SMS>(query, sp);
                }
                if (typeof(T) == typeof(Message_Mail))
                {
                    Message_Mail mail = message as Message_Mail;
                    string query = "INSERT INTO [Messages] ([Type], [Subject], Body) OUTPUT Inserted.ID " +
                        "VALUES (@messageType, @subject, @body)";
                    SqlParameter[] sp = new SqlParameter[]
                    {
                        CreateParameter("@messageType", mail.Type, SqlDbType.NVarChar),
                        CreateParameter("@subject", mail.Subject, SqlDbType.NVarChar),
                        CreateParameter("@body", mail.MessageBody, SqlDbType.NVarChar)
                    };
                    sw.WriteLine(FormatMail(message as Message_Mail));
                    return dh.AddDataReturn<Message_Mail>(query, sp);
                }
                return -1;
            }
            //return DatabaseHandler.SaveMessage(message);
        }

        private static string FormatSms(Message_SMS sms)
        {
            string internalMessage =
                $"01/{sms.Body}\n" +
                $"02/{PrintMessageDescription(sms.Offers)}\n" +
                $"03/{DateTime.Now}\n" +
                $"04/";
            return internalMessage;
        }

        private static string FormatMail(Message_Mail mail)
        {
            string internalMessage =
                $"00/{mail.Subject}\n" +
                $"01/{mail.Body}\n" +
                $"02/{PrintMessageDescription(mail.Offers)}\n" +
                $"03/{DateTime.Now}\n" +
                $"04/";
            return internalMessage;
        }

        private static string PrintMessageDescription<T>(List<T> items)
        {
            if (items == null)
                return null;
            string formatedItems = string.Empty;
            foreach (T item in items)
                formatedItems += $"{item}";
            return formatedItems;
        }

        /// <summary>
        /// Returns current directory.
        /// </summary>
        /// <returns></returns>
        private static string GetCurrentDirectory()
        {
            var rootPathChild = Directory.GetCurrentDirectory();
            var rootPathParent = Directory.GetParent($"{rootPathChild}");
            var rootPathFolder = Directory.GetParent($"{rootPathParent}");
            var rootPath = rootPathFolder.ToString();
            return rootPath;
        }

        /// <summary>
        /// Create parameter for SQL command.
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
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
