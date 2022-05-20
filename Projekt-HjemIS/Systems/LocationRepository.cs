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
    public class LocationRepository
    {
        private SqlConnection connection = new SqlConnection
    (ConfigurationManager.ConnectionStrings["post"].ConnectionString);
        public LocationRepository()
        {

        }
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
        public async Task<List<Location>> GetCities()
        {
            List<Location> locs = new List<Location>();
            try
            {
                await connection.OpenAsync();
                string query = $@"SELECT DISTINCT PostalCode, City FROM Locations WHERE PostalCode LIKE '%%' AND City LIKE '%%'";
                SqlCommand command = new SqlCommand(query, connection);
                command.CommandTimeout = 0;
                //command.Parameters.Add(CreateParameter("@postalcode", loc.PostalCode, SqlDbType.NVarChar));
                //command.Parameters.Add(CreateParameter("@city", loc.City, SqlDbType.NVarChar));
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
        public async Task<List<Location>> GetCities2()
        {
            List<Location> locs = new List<Location>();
            try
            {
                //await connection.OpenAsync();
                await OpenAndSetArithAbort(connection);
                string query2 = $@"SELECT STRING_AGG(innerQuery.PostalCode, ',') AS PostalCode, innerQuery.City
                FROM
		        (
			    SELECT	PostalCode, City
			    FROM  Locations
			    GROUP BY Locations.PostalCode ,Locations.City) AS innerQuery
                GROUP BY	innerQuery.City";
                string query = $@"CityPostalCode";
                SqlCommand command = new SqlCommand(query, connection);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 0;
                //command.Parameters.Add(CreateParameter("@postalcode", loc.PostalCode, SqlDbType.NVarChar));
                //command.Parameters.Add(CreateParameter("@city", loc.City, SqlDbType.NVarChar));
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        string currentCity = reader[$"{nameof(Location.City)}"].ToString().Trim();
                        string[] postalcodes= reader[$"{nameof(Location.PostalCode)}"].ToString().Split(new char[] { ',' });
                        foreach (string code in postalcodes)
                        {
                            locs.Add(new Location()
                            {
                                City = currentCity,
                                PostalCode = code
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
        public async Task<IEnumerable<Message>> FindMessages(Location location)
        {
            Dictionary<int, Message> msgs = new Dictionary<int, Message>();
            try
            {
                //await connection.OpenAsync();
                await OpenAndSetArithAbort(connection);
                string query = $@"SELECT Customers.PhoneNumber, [Messages].ID, [Messages].Body, [Messages].[Type], Customers.FirstName, Customers.LastName FROM 
                                (SELECT DISTINCT [Customers-Messages].ID, PhoneNumber
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
                //command.Parameters.Add(CreateParameter("@postalcode", loc.PostalCode, SqlDbType.NVarChar));
                //command.Parameters.Add(CreateParameter("@city", loc.City, SqlDbType.NVarChar));
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
                        Customer customer = new Customer(){ FirstName = fName, LastName = lName, PhoneNumber=phoneNumber};
                        if (!msgs.ContainsKey(id)) msgs.Add(id, new Message(){ ID = id, Body = body, Type = type });
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
        public async Task<Customer> FindMessagesByCustomer(Customer customer)
        {
            Dictionary<int, Customer> customers = new Dictionary<int, Customer>();
            try
            {
                //await connection.OpenAsync();
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
        private static SqlParameter CreateParameter(string paramName, object value, SqlDbType type)
        {
            SqlParameter param = new SqlParameter
            {
                ParameterName = paramName,
                Value = value,
                SqlDbType = type
            };
            return param;
        }
    }
}
