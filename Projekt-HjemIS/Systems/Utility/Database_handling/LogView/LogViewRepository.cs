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

namespace Projekt_HjemIS.Systems
{
    public class LogViewRepository
    {
        private SqlConnection connection = new SqlConnection
    (ConfigurationManager.ConnectionStrings["post"].ConnectionString);
        public LogViewRepository()
        {

        }
        /// <summary>
        /// Hovedforfatter: Jonas
        /// Gets all streets based on city/postalcode/street input from user
        /// </summary>
        /// <param name="loc"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Location>> GetLocations(Location loc)
        {
            List<Location> locs = new List<Location>();
            try
            {
                await connection.OpenAsync();
                string query = $@"SELECT TOP 50 Street, City, PostalCode
                                FROM Locations 
                                WHERE City LIKE '%{loc.City}%' AND PostalCode LIKE '%{loc.PostalCode}%' AND Street LIKE '%{loc.Street}%'";
                SqlCommand command = new SqlCommand(query, connection);
                using(SqlDataReader reader = await command.ExecuteReaderAsync())
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
        /// <summary>
        /// Hovedforfatter: Jonas
        /// Gets all customers related to a specific city, postalcode and street
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Hovedforfatter: Jonas
        /// Gets every unique pair of city and associated postalcode. This is what makes the combobox searchable
        /// </summary>
        /// <returns></returns>
        public async Task<List<Location>> GetCities()
        {
            List<Location> locs = new List<Location>();
            try
            {
                using(SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["post"].ConnectionString))
                {
                    await connection.OpenAsync();
                    string query = $@"SELECT DISTINCT PostalCode, City FROM Locations WHERE PostalCode LIKE '%%' AND City LIKE '%%'";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.CommandTimeout = 0;

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            locs.Add(new Location()
                            {
                                City = (string)reader[$"{nameof(Location.City)}"].ToString().Trim(),
                                PostalCode = (string)reader[$"{nameof(Location.PostalCode)}"]
                            });
                        }
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
        /// <summary>
        /// Hovedforfatter: Jonas
        /// Finds all messages and related customers in a specific geographical location, city/postalcode/street.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Message>> FindMessages(Location location)
        {
            Dictionary<int, Message> msgs = new Dictionary<int, Message>();
            try
            {
                await OpenAndSetArithAbort(connection);
                string query = $@"SELECT Customers.PhoneNumber, [Messages].ID, [Messages].Body, [Messages].[Type], Customers.FirstName, Customers.LastName, DT.[Date] FROM 
                                (SELECT DISTINCT [Customers-Messages].ID, PhoneNumber, [Customers-Messages].[Date]
                                    FROM [Customers-Messages]
                                    WHERE PhoneNumber IN
                                (SELECT PhoneNumber
                                    FROM Locations
                                    CROSS APPLY(SELECT CountyCode, StreetCode, FirstName, LastName, PhoneNumber FROM Customers 
                                    WHERE City LIKE '%{location.City}%' AND PostalCode LIKE '%{location.PostalCode}%' AND STREET LIKE '%{location.Street}%' AND Locations.CountyCode = Customers.CountyCode AND Locations.StreetCode = Customers.StreetCode)A)) AS DT
                                RIGHT JOIN [Messages] ON DT.ID = [Messages].ID
                                LEFT JOIN Customers ON DT.PhoneNumber = Customers.PhoneNumber";
                SqlCommand command = new SqlCommand(query, connection);
                command.CommandTimeout = 0;
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        int id = (int)reader[$"{nameof(Message.ID)}"];
                        string body = reader[$"{nameof(Message.Body)}"].ToString().Trim();
                        string type = reader[$"{nameof(Message.Type)}"].ToString().Trim();
                        string fName = reader[$"FirstName"].ToString().Trim();
                        string lName = reader[$"LastName"].ToString().Trim();
                        int phoneNumber = (int)reader[$"PhoneNumber"];
                        DateTime date = (DateTime)reader[$"{nameof(Message.Date)}"];
                        Customer customer = new Customer(){ FirstName = fName, LastName = lName, PhoneNumber=phoneNumber};
                        if (!msgs.ContainsKey(id)) msgs.Add(id, new Message(){ ID = id, Body = body, Type = type, Date = date });
                        msgs[id].Recipients.Add(customer);
                    }
                }
                return msgs.Values;
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
        /// <summary>
        /// Hovedforfatter: Jonas
        /// Finds all messages related to a specific customer
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public async Task<Customer> FindMessagesByCustomer(Customer customer)
        {
            Dictionary<int, Customer> customers = new Dictionary<int, Customer>();
            try
            {
                await OpenAndSetArithAbort(connection);
                string query = $@"SELECT [Messages].Body,[Messages].[Type], [Customers-Messages].Date, Customers.FirstName, Customers.LastName, Customers.PhoneNumber
                                FROM Customers 
                                INNER JOIN [Customers-Messages] ON Customers.PhoneNumber = [Customers-Messages].PhoneNumber
                                INNER JOIN [Messages] ON [Customers-Messages].ID = [Messages].ID 
                                WHERE Customers.PhoneNumber = {customer.PhoneNumber}";
                SqlCommand command = new SqlCommand(query, connection);
                command.CommandTimeout = 0;
                int number = 0;
                //command.Parameters.Add(CreateParameter("@postalcode", loc.PostalCode, SqlDbType.NVarChar));
                //command.Parameters.Add(CreateParameter("@city", loc.City, SqlDbType.NVarChar));
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        string body = reader[$"{nameof(Message.Body)}"].ToString().Trim();
                        string type = reader[$"{nameof(Message.Type)}"].ToString().Trim();
                        string fName = reader[$"FirstName"].ToString().Trim();
                        string lName = reader[$"LastName"].ToString().Trim();
                        var date = reader[$"Date"];
                        number = (int)reader[$"PhoneNumber"];
                        Message msg = new Message() { Body = body, Type = type, Date = (DateTime)date };
                        Customer cust = new Customer() { FirstName = fName, LastName = lName, PhoneNumber = number };
                        if (!customers.ContainsKey(number)) customers.Add(number, new Customer() { PhoneNumber = number, FirstName = fName, LastName = lName});
                        customers[number].MsgReceived.Add(msg);
                        msg.Recipients.Add(customers[number]);
                    }
                }
                return customers[number];
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
        /// <summary>
        /// Hovedforfatter: Jonas
        /// Enables Arithabort on a query, allegedly increases performance specifically for stored procedures
        /// </summary>
        /// <param name="MyConnection"></param>
        /// <returns></returns>
        public async Task OpenAndSetArithAbort(SqlConnection MyConnection)
        {
            using (SqlCommand _Command = MyConnection.CreateCommand())
            {
                _Command.CommandType = CommandType.Text;
                _Command.CommandText = "SET ARITHABORT ON;";

                await MyConnection.OpenAsync();

                await _Command.ExecuteNonQueryAsync();
            }
            return;
        }
    }
}
