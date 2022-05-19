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
        public CustomerFactory()
        {
            CreateNewCustomer();
        }





        Customer customer = new Customer();

        List<Customer> createdCustomers = new List<Customer>();

        public static Random rand = new Random();

        public void CreateNewCustomer()
        {

         
            if (createdCustomers.Count == 0)
            {

                using (StreamReader sr = File.OpenText(GetCurrentDirectory() + @"\fakeNameList\names.txt"))
                {

                    string currentLine = string.Empty;

                    // Read each line from current file.
                    while ((currentLine = sr.ReadLine()) != null)
                    {
                        string[] fullName = currentLine.Split(' ');

                        customer.FirstName = fullName[0];
                        customer.LastName = fullName[1];

                        createdCustomers.Add(customer);

                        Debug.WriteLine(customer);
                    }

                }

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
