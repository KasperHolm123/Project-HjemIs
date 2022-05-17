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
        private static SqlConnection connection = new SqlConnection
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
                SqlCommand command = new SqlCommand(query2, connection);
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
