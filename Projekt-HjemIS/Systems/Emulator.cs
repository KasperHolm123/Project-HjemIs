using Projekt_HjemIS.Models;
using Projekt_HjemIS.Systems.Utility.Database_handling;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt_HjemIS.Systems
{
    public class Emulator
    {
        public ObservableCollection<string> InternalMessages { get; set; }
        public ObservableCollection<Customer> InternalCustomers { get; set; }
        private DatabaseHandler dh = new DatabaseHandler();

        private string _rootPath = $@"\tempMessages\InternalMessages.txt";


        public Emulator()
        {

        }

        /// <summary>
        /// Shows all sent messages within the program's lifetime.
        /// </summary>
        public void EmulateReceiver()
        {
            string currentLine = string.Empty;
            using (StreamReader sr = new StreamReader(File.OpenRead($@"{GetCurrentDirectory()}{_rootPath}")))
            {
                string temp = "";
                while ((currentLine = sr.ReadLine()) != null)
                {
                    if (currentLine != "____END OF MESSAGE____")
                        temp += currentLine;

                    if (currentLine == "____END OF MESSAGE____")
                        InternalMessages.Add(temp); // by value or by reference? testing..
                }
            }
        }

        /// <summary>
        /// Gets all receivers of sent messages within the program's lifetime.
        /// </summary>
        public void GetReceivers()
        {
            // Hent data af modtagerne af sendte beskeder i gennem DatabaseHandler.
            string _getCustomerQuery = "SELECT * FROM Customers";
            InternalCustomers = new ObservableCollection<Customer>(dh.GetTable<Customer>(_getCustomerQuery));
        }

        /// <summary>
        /// Returns current directory.
        /// </summary>
        /// <returns></returns>
        private string GetCurrentDirectory()
        {
            var rootPathChild = Directory.GetCurrentDirectory();
            var rootPathParent = Directory.GetParent($"{rootPathChild}");
            var rootPathFolder = Directory.GetParent($"{rootPathParent}");
            var rootPath = rootPathFolder.ToString();
            return rootPath;
        }
    }
}
