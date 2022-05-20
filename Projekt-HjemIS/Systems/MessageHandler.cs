using Projekt_HjemIS.Models;
using System;
using System.Collections.Generic;
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
            DatabaseHandler.GetCustomers();
        }

        /// <summary>
        /// Writes the content of a message out into a .txt file. Send the message to DatabaseHandler. Returns an int.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        public static int SendMessages<T>(T message) where T : Message
        {
            string products = string.Empty;
            if (message.Offers != null)
                foreach (Product item in message.Offers)
                    products += $"{item.Name}, ";
            using (StreamWriter sw = File.AppendText($@"{GetCurrentDirectory()}{_rootPath}"))
            {
                if (typeof(T) == typeof(Message_SMS))
                    sw.WriteLine(FormatSms(message as Message_SMS));
                if (typeof(T) == typeof(Message_Mail))
                    sw.WriteLine(FormatMail(message as Message_Mail));
            }
            return DatabaseHandler.SaveMessage(message);
        }

        private static string FormatSms(Message_SMS sms)
        {
            string internalMessage =
                $"01/{sms.MessageBody}\n" +
                $"02/{PrintMessageDescription(sms.Offers)}\n" +
                $"03/{DateTime.Now}\n" +
                $"04/";
            return internalMessage;
        }

        private static string FormatMail(Message_Mail mail)
        {
            string internalMessage =
                $"00/{mail.Subject}\n" +
                $"01/{mail.MessageBody}\n" +
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
    }
}
