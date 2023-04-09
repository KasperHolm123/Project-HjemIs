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

        private RecordTypeLocation _selectedLocation;
        public RecordTypeLocation SelectedLocation
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

        private ObservableCollection<RecordTypeLocation> _streets;
        public ObservableCollection<RecordTypeLocation> Streets
        {
            get { return _streets; }
            set
            {
                _streets = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<RecordTypeLocation> _locations;
        public ObservableCollection<RecordTypeLocation> Locations
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
            Task.Run(() => LoadCollectionsAsync());

            GetCustomersCommand = new RelayCommand(p => GetCustomers());
            GetMessagesCommand = new RelayCommand(p => GetMessages());
        }

        private async Task LoadCollectionsAsync()
        {
            var locations = await dh.GetTable<RecordTypeLocation>("SELECT * FROM [location]");
            Locations = new ObservableCollection<RecordTypeLocation>(locations);

            var messages = await dh.GetTable<Message>("SELECT * FROM message");
            Messages = new ObservableCollection<Message>(messages);
        }


        private async void GetCustomers()
        {
            State = QueryState.Executing;

            var query = $@"SELECT FirstName, LastName, PhoneNumber, DT.CountyCode, DT.StreetCode
                           FROM customer
                           INNER JOIN (
                               SELECT CountyCode, StreetCode
                               FROM [location]
                               WHERE City LIKE @City
                               AND PostalCode LIKE @PostalCode
                               AND Street LIKE @Street
                           ) AS DT
                           ON DT.CountyCode = customer.CountyCode
                           AND DT.StreetCode = customer.StreetCode";
            
            List<SqlParameter> parameters = new List<SqlParameter>();

            try
            {
                parameters = new List<SqlParameter>
                {
                    new SqlParameter("@City", SelectedLocation.CityName),
                    new SqlParameter("@PostalCode", SelectedLocation.PostalCode),
                    new SqlParameter("@Street", SelectedLocation.StreetName)
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
                          FROM customer c
                          INNER JOIN [location] l 
                              ON l.CountyCode = c.CountyCode 
                              AND l.StreetCode = c.StreetCode
                          INNER JOIN Recipients cm 
                              ON c.PhoneNumber = cm.PhoneNumber
                          LEFT JOIN message m 
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
                    new SqlParameter("@City", SelectedLocation.CityName),
                    new SqlParameter("@PostalCode", SelectedLocation.PostalCode),
                    new SqlParameter("@Street", SelectedLocation.StreetName)
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
