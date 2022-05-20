using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Projekt_HjemIS.Models;
using System.IO;
using System.Text;
using System.Diagnostics;

namespace Projekt_HjemIS.Systems
{
    class CustomerFactory
    {
        // Call the CreateNewCustomer method when CustomerFactory gets instantiated in Main
        public CustomerFactory()
        {
            CreateNewCustomer();
        }



        // Allow access to the fields of Customer
        Customer customer = new Customer();

        public static RecordHandler recordHandler = new RecordHandler();

        
        // Contains the customers with values assigned to them
        List<Customer> createdCustomers = new List<Customer>();

        // Contains created phone numbers for checking against duplicates
        public static List<int> phoneBook = new List<int>();

        public List<Location> locations = new List<Location>();

        public static Random rand = new Random();
 
        public int temp;
        

        // Handles generating a unique 8 digit phone number
        public int GeneratePhoneNumber()
        {
            int result;
            do // Do atleast once, repeat if the generated phone number ends up already existing in the phoneBook
            {
                int[] phoneNum = new int[8];

                for (int i = 0; i < phoneNum.Length; i++)
                {
                    phoneNum[i] = rand.Next(0, 9);
                }

                // convert int[] to string and parse the string to end up with result as a int
                result = Int32.Parse(string.Join("", phoneNum));

                // temp stores the value of IndexOf
                temp = phoneBook.IndexOf(result);

            } while (temp > 0);

            phoneBook.Add(result);
            return result;
        }

        
        // Parameter 0 returns street, 1 returns county
        public string GenerateAddress(int chooseOne)
        {
            Location loc = new Location();

            temp = rand.Next(recordHandler._locationsList.Count());

            loc = recordHandler._locationsList[temp];

            string street = loc.StreetCode;
            string county = loc.CountyCode;

            if (chooseOne > 1)
            {
                return street;
            }
            else
            {
                return county;
            }
            

        }





        // Handles assigning values to the customer and storing them in a list
        public void CreateNewCustomer()
        {
            // Only create customers if no customers have been created 
            if (createdCustomers.Count == 0)
            {

                // Grabs a list of random names from a text file
                using (StreamReader sr = File.OpenText(GetCurrentDirectory() + @"\fakeNameList\names.txt"))
                {

                    string currentLine = string.Empty;

                    // Read each line from current file.
                    while ((currentLine = sr.ReadLine()) != null)
                    {
                        // Split each name into first name and last name
                        string[] fullName = currentLine.Split(' ');
                        customer = new Customer();
                        customer.FirstName = fullName[0];
                        customer.LastName = fullName[fullName.Length - 1]; // Takes the last word in each line as the last name, skipping all middle names
                        customer.PhoneNumber = GeneratePhoneNumber();
                        customer.StreetCode = GenerateAddress(0);
                        customer.CountyCode = GenerateAddress(1);
                        

                        createdCustomers.Add(customer);

                    }
                }
            }
            foreach (var item in createdCustomers)
            {
                Debug.WriteLine(customer);

            }
        }

        

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
