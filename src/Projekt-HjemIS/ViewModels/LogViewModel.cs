using Projekt_HjemIS.Models;
using Projekt_HjemIS.Services;
using Projekt_HjemIS.Systems;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Diagnostics;
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

        private string _citySearch = "";
        public string CitySearch
        {
            get => _citySearch;
            set
            {
                _citySearch = value;
                OnPropertyChanged();
            }
        }

        private string _streetSearch = "";
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

        private Location _selectedLocation;
        public Location SelectedLocation
        {
            get => _selectedLocation;
            set
            {
                _selectedLocation = value;
                OnPropertyChanged();
            }
        }

        #region Collections

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

        private ObservableCollection<Location> _streets;
        public ObservableCollection<Location> Streets
        {
            get { return _streets; }
            set
            {
                _streets = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Location> _locations;
        public ObservableCollection<Location> Locations
        {
            get { return _locations; }
            set
            {
                _locations = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #endregion

        #region Commands

        public RelayCommand GetCustomersCommand { get; set; }
        public RelayCommand GetMessagesCommand { get; set; }

        #endregion

        public LogViewModel()
        {
            GetCustomersCommand = new RelayCommand(p => GetCustomers());
            GetMessagesCommand = new RelayCommand(p => GetMessages());

            Locations = new ObservableCollection<Location>(dh.GetTable<Location>("SELECT * FROM Locations"));
            Messages = new ObservableCollection<Message>(dh.GetTable<Message>("SELECT * FROM Messages"));
        }


        private async void GetCustomers()
        {
            State = QueryState.Executing;

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
            
            List<SqlParameter> parameters = new List<SqlParameter>();

            try
            {
                parameters = new List<SqlParameter>
                {
                    new SqlParameter("@City", SelectedLocation.City),
                    new SqlParameter("@PostalCode", SelectedLocation.PostalCode),
                    new SqlParameter("@Street", SelectedLocation.Street)
                };
            }
            catch (Exception ex)
            {

            }

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

        private async void GetMessages()
        {
            State = QueryState.Executing;

            var query = @"SELECT 
                              c.PhoneNumber, 
                              c.FirstName, 
                              c.LastName, 
                              m.Body, 
                              m.Type, 
                              cm.ID, 
                              cm.Date
                          FROM Customers c
                          INNER JOIN Locations l 
                              ON l.CountyCode = c.CountyCode 
                              AND l.StreetCode = c.StreetCode
                          INNER JOIN [Customers-Messages] cm 
                              ON c.PhoneNumber = cm.PhoneNumber
                          LEFT JOIN Messages m 
                              ON cm.ID = m.ID
                          WHERE 
                              l.City = @City 
                              AND l.PostalCode = @PostalCode 
                              AND l.Street = @Street";

            List<SqlParameter> parameters = new List<SqlParameter>();

            try
            {
                parameters = new List<SqlParameter>
                {
                    new SqlParameter("@City", SelectedLocation.City),
                    new SqlParameter("@PostalCode", SelectedLocation.PostalCode),
                    new SqlParameter("@Street", SelectedLocation.Street)
                };
            }
            catch (Exception ex)
            {

            }

            IEnumerable<Message> msgs = await Task.Run(() => dh.GetTable<Message>(query, parameters));

            if (msgs != null)
            {
                Messages = new ObservableCollection<Message>(msgs);
            }
            else
            {
                Messages.Clear();
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
