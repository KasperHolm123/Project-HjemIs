using Projekt_HjemIS.Models;
using Projekt_HjemIS.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Projekt_HjemIS.ViewModels
{
    public class EmailViewModel : BaseViewModel
    {

        #region Fields

        private string _searchQuery;
        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                _searchQuery = value;
                FilteredLocations.View.Refresh();
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Location> _locations;
        public ObservableCollection<Location> Locations
        {
            get => _locations;
            set
            {
                _locations = value;
                OnPropertyChanged();
            }
        }

        private CollectionViewSource _filteredLocations;
        public CollectionViewSource FilteredLocations
        {
            get => _filteredLocations;
            set
            {
                _filteredLocations = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Location> _recipients;
        public ObservableCollection<Location> Recipients
        {
            get => _recipients;
            set
            {
                _recipients = value;
                OnPropertyChanged();
            }
        }

        private Message _message;
        public Message Message
        {
            get => _message;
            set
            {
                _message = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Commands

        public RelayCommand AddRecipientCommand { get; set; }
        public RelayCommand RemoveRecipientCommand { get; set; }
        public RelayCommand CreateMessageCommand { get; set; }

        #endregion

        public EmailViewModel()
        {
            Locations = new ObservableCollection<Location>(dh.GetTable<Location>("SELECT * FROM Locations"));
            Recipients = new ObservableCollection<Location>();

            Message = new Message();

            FilteredLocations = new CollectionViewSource
            {
                Source = Locations
            };
            FilteredLocations.Filter += Search;

            GetCustomersOnRecipient(Locations[0]);

            AddRecipientCommand = new RelayCommand(p => AddRecipient((Location)p));
            RemoveRecipientCommand = new RelayCommand(p => RemoveRecipient((Location)p));
            CreateMessageCommand = new RelayCommand(p => CreateMessage());
        }

        private async void CreateMessage()
        {
            var query = "INSERT INTO Messages ([Subject], Body)" +
                        "VALUES (@Subject, @Body)";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Subject", Message.Subject),
                new SqlParameter("@Body", Message.Body)
            };

            await dh.AddData(query, parameters.ToArray());

            //AddRecipientToMessage();
        }

        private async Task AddRecipientToMessage()
        {
            foreach (var recipient in Recipients)
            {
                var phoneNumbers = await GetCustomersOnRecipient(recipient);
                
                foreach (var phoneNumber in phoneNumbers)
                {
                    var query = "";
                }
            }
        }

        private async Task<List<int>> GetCustomersOnRecipient(Location location)
        {
            var query = "SELECT PhoneNumber FROM Customers " +
                        "INNER JOIN (" +
                        "SELECT CountyCode, StreetCode " +
                        "FROM Locations " +
                        "WHERE CountyCode = @CountyCode " +
                        "AND StreetCode = @StreetCode" +
                        ") AS l " +
                        "ON l.CountyCode = Customers.CountyCode " +
                        "AND l.StreetCode = Customers.StreetCode";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@CountyCode", location.CountyCode),
                new SqlParameter("@StreetCode", location.StreetCode)
            };

            var customers = await dh.GetTable<Customer>(query, parameters);

            var phoneNumbers = customers.Select(x => x.PhoneNumber).ToList();

            return phoneNumbers;
        }

        private void AddRecipient(Location location)
        {
            if (location != null)
            {
                Recipients.Add(location);
                Locations.Remove(location);
            }
        }

        private void RemoveRecipient(Location location)
        {
            if (location != null)
            {
                Recipients.Remove(location);
                Locations.Add(location);
            }
        }

        private void Search(object sender, FilterEventArgs e)
        {
            if (string.IsNullOrEmpty(SearchQuery))
            {
                e.Accepted = true;
            }
            else
            {
                e.Accepted = e.Item is Location item && item.Street.ToUpper().Contains(SearchQuery.ToUpper());
            }
        }
    }
}
