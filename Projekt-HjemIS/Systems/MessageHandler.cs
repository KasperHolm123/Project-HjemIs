using Projekt_HjemIS.Models;
using System;
using System.Collections.Generic;
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

        public static void SendMessages(Message message)
        {
            string products = string.Empty;
            if (message.Offers != null)
                foreach (Product item in message.Offers)
                    products += $"{item.Name}, ";
            using (StreamWriter sw = File.AppendText($@"{GetCurrentDirectory()}{_rootPath}"))
            {
                sw.WriteLine(FormatMessage(message));
            }
            DatabaseHandler.SaveMessage(message);
        }

        private static string FormatMessage(Message message)
        {
            string internalMessage = 
                $"Subject:{message.Subject}\n\n" +
                $"{message.MessageBody}\n" +
                $"{PrintMessageDescription(message.Offers)}\n" +
                "____END OF MESSAGE____";
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
