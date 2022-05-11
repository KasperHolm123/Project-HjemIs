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

        private static void GetCustomers()
        {
            DatabaseHandler.GetCustomers();
        }

        public static void SendMessages(List<Customer> recipients, string subject, List<bool> messageType, string body, List<Product> offers = null)
        {
            string products = string.Empty;
            foreach (Product item in offers)
            {
                products += $"{item.Name}, ";
            }
            using (StreamWriter sw = File.AppendText($@"{GetCurrentDirectory()}{_rootPath}"))
            {
                if (offers != null)
                {
                    sw.WriteLine(
                        $"To:{PrintItemDescription(recipients)}      | Offers:{products}\n" +
                        $"Subject:{subject}    | SMS|Mail\n" +
                        $"{body}\n" +
                        $"{PrintItemDescription(offers)}\n" +
                        "____END OF MESSAGE____");
                }
            }
        }

        private static string PrintItemDescription<T>(List<T> items)
        {
            string formatedItems = string.Empty;
            foreach (T item in items)
            {
                formatedItems += $"{item}";
            }
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
