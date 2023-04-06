using Projekt_HjemIS.Models;
using Projekt_HjemIS.Systems;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Projekt_HjemIS.ViewModels
{
    public class LogViewModel : BaseViewModel
    {

        #region Fields

        private string _citySearch;
        public string CitySearch
        {
            get => _citySearch;
            set
            {
                _citySearch = value;
                OnPropertyChanged();
            }
        }

        private string _streetSearch;
        public string StreetSearch
        {
            get => _streetSearch;
            set
            {
                _streetSearch = value;
                OnPropertyChanged();
            }
        }

        private QueryState _state;
        public QueryState State
        {
            get => _state;
            set
            {
                _state = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Customer> _customers;
        public ObservableCollection<Customer> Customers
        {
            get => _customers;
            set
            {
                _customers = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Message> _messages;
        public ObservableCollection<Message> Messages
        {
            get => _messages;
            set
            {
                _messages = value;
                OnPropertyChanged();
            }
        }

        #endregion

        public LogViewModel()
        {
            
        }


        private async void GetCustomers_Click(object sender, RoutedEventArgs e)
        {
            State = QueryState.Executing;

            string[] input = CitySearch.Split('-');

            var query = $@"SELECT FirstName, LastName, PhoneNumber, DT.CountyCode, DT.StreetCode
                                FROM Customers
                                INNER JOIN (
                                    SELECT CountyCode, StreetCode
                                    FROM Locations
                                    WHERE City LIKE @City
                                    AND PostalCode LIKE @PostalCode
                                    AND Street LIKE @Street
                                ) AS DT
                                ON DT.CountyCode = Customers.CountyCode
                                AND DT.StreetCode = Customers.StreetCode";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@City", input[0]),
                new SqlParameter("@PostalCode", input[1]),
                new SqlParameter("@Street", StreetSearch)
            };

            IEnumerable<Customer> customers = await Task.Run(() => dh.GetTable<Customer>(query, parameters));

            if (customers != null)
            {
                Customers = new ObservableCollection<Customer>(customers);
            }
            else
            {
                Customers.Clear();
            }

            State = QueryState.Finished;
        }

        private async void FindMessages_Click(object sender, RoutedEventArgs e)
        {
            State = QueryState.Executing;

            string[] input = CitySearch.Split('-');

            if (input.Length > 1)
            {
                Location loc = new Location()
                {
                    City = input[0],
                    PostalCode = input[1],
                    Street = StreetSearch
                };

                /*
                IEnumerable<Message> msgs = await Task.Run(() => locationRepository.FindMessages(loc));

                if (msgs != null)
                {
                    Messages = new ObservableCollection<Message>(msgs);
                }
                else
                {
                    Messages.Clear();
                }
                */

                OnPropertyChanged("MsgsFound");
            }

            State = QueryState.Finished;
        }
    }

    public enum QueryState
    {
        Standby = 0,
        Executing = 1,
        Finished = 2,
    }
}
