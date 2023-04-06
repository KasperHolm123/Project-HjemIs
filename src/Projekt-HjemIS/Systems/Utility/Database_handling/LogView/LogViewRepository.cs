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
using System.Windows;

namespace Projekt_HjemIS.Systems
{
    public class LogViewRepository
    {
        private SqlConnection connection = new SqlConnection(
            ConfigurationManager.ConnectionStrings["post"].ConnectionString);
        
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
                command.CommandTimeout = 0;
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

                var query = @"
                    SELECT Customers.PhoneNumber, Messages.ID, Messages.Body, Messages.Type, Customers.FirstName, Customers.LastName, [Customers-Messages].Date
                    FROM Customers
                    INNER JOIN Locations ON Locations.CountyCode = Customers.CountyCode AND Locations.StreetCode = Customers.StreetCode
                    INNER JOIN [Customers-Messages] ON Customers.PhoneNumber = [Customers-Messages].PhoneNumber
                    LEFT JOIN Messages ON [Customers-Messages].ID = Messages.ID
                    WHERE Locations.City LIKE @City AND Locations.PostalCode LIKE @PostalCode AND Locations.Street LIKE @Street
                ";

                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@City", $"%{location.City}%"),
                    new SqlParameter("@PostalCode", $"%{location.PostalCode}%"),
                    new SqlParameter("@Street", $"%{location.Street}%")
                };

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddRange(parameters.ToArray());
                    command.CommandTimeout = 0;

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        var messages = new Dictionary<int, Message>();

                        while (await reader.ReadAsync())
                        {
                            var id = reader.GetInt32(reader.GetOrdinal("ID"));
                            var body = reader.GetString(reader.GetOrdinal("Body")).Trim();
                            var type = reader.GetString(reader.GetOrdinal("Type")).Trim();
                            var firstName = reader.GetString(reader.GetOrdinal("FirstName")).Trim();
                            var lastName = reader.GetString(reader.GetOrdinal("LastName")).Trim();
                            var phoneNumber = reader.GetInt32(reader.GetOrdinal("PhoneNumber"));
                            var date = reader.GetDateTime(reader.GetOrdinal("Date"));

                            var customer = new Customer
                            {
                                FirstName = firstName,
                                LastName = lastName,
                                PhoneNumber = phoneNumber,
                            };

                            if (!messages.ContainsKey(id))
                            {
                                messages.Add(id, new Message
                                {
                                    ID = id,
                                    Body = body,
                                    Type = type,
                                    Date = date
                                });
                            }

                            messages[id].Recipients.Add(customer);
                        }
                    }
                    
                    return msgs.Values;
                }
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
                                WHERE Customers.PhoneNumber = @PhoneNumber";

                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@PhoneNumber", $"%{customer.PhoneNumber}%")
                };

                using (var command = new SqlCommand(query, connection))
                {
                    command.CommandTimeout = 0;

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        int phoneNumber = 0;
                        while (await reader.ReadAsync())
                        {
                            var body = reader.GetString(reader.GetOrdinal("body")).Trim();
                            var type = reader.GetString(reader.GetOrdinal("Type")).Trim();
                            var firstName = reader.GetString(reader.GetOrdinal("FirstName")).Trim();
                            var lastName = reader.GetString(reader.GetOrdinal("LastName")).Trim();
                            var date = reader.GetDateTime(reader.GetOrdinal("Date"));
                            phoneNumber = reader.GetInt32(reader.GetOrdinal("PhoneNumber"));

                            Message msg = new Message()
                            { 
                                Body = body,
                                Type = type,
                                Date = date 
                            };

                            if (!customers.ContainsKey(phoneNumber))
                            {
                                customers.Add(phoneNumber, new Customer()
                                {
                                    PhoneNumber = phoneNumber,
                                    FirstName = firstName,
                                    LastName = lastName
                                });
                            }

                            customers[phoneNumber].MsgReceived.Add(msg);

                            msg.Recipients.Add(customers[phoneNumber]);
                        }
                        
                        return customers[phoneNumber];
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                connection.Close();
            }
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
        }
    }
}
