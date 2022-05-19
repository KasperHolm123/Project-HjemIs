using Projekt_HjemIS.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
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
        public async Task<IEnumerable<Location>> GetCustomers(Customer customer)
        {
            List<Location> locs = new List<Location>();
            try
            {
                await connection.OpenAsync();
                string query = $@"SELECT TOP 50 Street, City, PostalCode
                                FROM Locations 
                                WHERE City LIKE '%{loc.City}%' AND PostalCode LIKE '%{loc.PostalCode}%' AND Street LIKE '%{loc.Street}%'";
                SqlCommand command = new SqlCommand(query, connection);
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        locs.Add(new Location()
                        {
                            City = (string)reader[$"{nameof(Location.City)}"].ToString().Trim(),
                            PostalCode = (string)reader[$"{nameof(Location.PostalCode)}"],
                            Street = (string)reader[$"{nameof(Location.Street)}"].ToString().Trim()
                        });
                    }
                }
                return locs;
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
