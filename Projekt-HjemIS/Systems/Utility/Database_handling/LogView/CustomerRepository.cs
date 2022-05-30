using Projekt_HjemIS.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt_HjemIS.Systems
{
    public  class CustomerRepository
    {
        private static SqlConnection connection = new SqlConnection
(ConfigurationManager.ConnectionStrings["post"].ConnectionString);
        public CustomerRepository()
        {

        }
        public async Task<IEnumerable<Customer>> GetCustomers(Customer customer)
        {
            List<Customer> customers = new List<Customer>();
            try
            {
                await connection.OpenAsync();
                string query = $@"SELECT FirstName, LastName, PhoneNumber, DT.CountyCode, DT.StreetCode FROM Customers
                                INNER JOIN (SELECT CountyCode, StreetCode FROM Locations WHERE City LIKE '%{customer.Address.City}%' AND PostalCode LIKE '%{customer.Address.PostalCode}%' AND Street LIKE '%{customer.Address.Street}%') AS DT
                                ON DT.CountyCode = Customers.CountyCode AND DT.StreetCode = Customers.StreetCode";
                SqlCommand command = new SqlCommand(query, connection);
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        customers.Add(new Customer()
                        {
                            FirstName = reader[$"{nameof(Customer.FirstName)}"].ToString().Trim(),
                            LastName = reader[$"{nameof(Customer.LastName)}"].ToString().Trim(),
                            PhoneNumber = (int)reader[$"{nameof(Customer.PhoneNumber)}"]
                        });
                    }
                }
                return customers;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                connection.Close();
            }
            return null;
        }
    }
}
