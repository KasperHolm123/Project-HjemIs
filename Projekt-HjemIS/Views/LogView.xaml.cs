using Projekt_HjemIS.Models;
using Projekt_HjemIS.Systems;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Projekt_HjemIS.Views
{
    /// <summary>
    /// Interaction logic for LogView.xaml
    /// </summary>
    public partial class LogView : UserControl, INotifyPropertyChanged
    {
        private LocationRepository locationRepository;
        private CustomerRepository customerRepository;
        private ObservableCollection<Location> _locations;
        private ObservableCollection<Location> _streets;
        private ObservableCollection<Message> _messages;
        private ObservableCollection<Customer> _customers;
        public List<Location> InternalLocations { get; set; }
        public Customer CurrentCustomer { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        private List<Location> _internalStreets = new List<Location>();
        private ObservableCollection<Location> _locationsToSearch;

        public ObservableCollection<Customer> Customers
        {
            get { return _customers; }
            set
            {
                _customers = value;
                OnPropertyChanged("Customers");
            }
        }

        public ObservableCollection<Message> Messages
        {
            get { return _messages; }
            set
            {
                _messages = value;
                OnPropertyChanged("Messages");
            }
        }

        public ObservableCollection<Location> Streets
        {
            get { return _streets; }
            set
            {
                _streets = value;
                OnPropertyChanged("Streets");
            }
        }
        public ObservableCollection<Location> Locations
        {
            get { return _locations; }
            set
            {
                _locations = value;
                OnPropertyChanged("Locations");
            }
        }
        public ObservableCollection<Location> LocationsToSearch
        {
            get { return _locationsToSearch; }
            set
            {
                _locationsToSearch = value;
                OnPropertyChanged("LocationsToSearch");
            }
        }
        public List<Location> InternalStreets
        {
            get { return _internalStreets; }
            set
            {
                _internalStreets = value;
                OnPropertyChanged("SearchCanExecute");
                //Streets = new ObservableCollection<Location>(_internalStreets.Where(p => FilterLocations(p)));
            }
        }
        //Text-field properties
        #region
        string _streetSearchText = "";
        string _citySearchText = "";
        string _postSearchText = "";
        private QueryState _state;
        public string CitySearchText
        {
            get { return _citySearchText; }
            set
            {
                if (_citySearchText == value) return;
                _citySearchText = value;
                //if(InternalLocations != null ) Locations = new ObservableCollection<Location>(InternalLocations.Where(p => FilterLocations(p)));
                OnPropertyChanged("CitySearchText");
                OnPropertyChanged("CanExecute");
            }
        }
        public string PostSearchText
        {
            get { return _postSearchText; }
            set
            {
                if (_postSearchText == value) return;
                _postSearchText = value;
                //if (InternalLocations != null) Locations = new ObservableCollection<Location>(InternalLocations.Where(p => FilterLocations(p)));
                OnPropertyChanged("PostSearchText");
                OnPropertyChanged("CanExecute");
            }
        }
        public string StreetSearchText
        {
            get { return _streetSearchText; }
            set
            {
                _streetSearchText = value;
            }
        }
        private string _customerSearchText;

        public string CustomerSearchText
        {
            get { return _customerSearchText; }
            set
            {
                _customerSearchText = value;
                OnPropertyChanged("CustomerSearchText");
            }
        }

        public bool CanExecute
        {
            get { return (CitySearchText.Length > 2 && PostSearchText.Length > 2); }
        }
        public bool SearchCanExecute
        {
            get { return InternalLocations.Any() ? true : false; }
        }
        public QueryState State
        {
            get { return _state; }
            set
            {
                _state = value;
                OnPropertyChanged("State");
            }
        }
        #endregion
        public LogView(ref ObservableCollection<Location> locations)
        {
            InitializeComponent();
            DataContext = this;
            locationRepository = new LocationRepository(); 
            Locations = locations;
            customerRepository = new CustomerRepository();
        }

        public void OnPropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        private void PreviewText(object sender, TextCompositionEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            comboBox.IsDropDownOpen = true;
        }
        //Event handlers related to streets
        #region 
        private async void FindStreets_Click(object sender, RoutedEventArgs e)
        {
            State = Views.QueryState.Executing;
            string[] input = _citySearchText.Split('-');
            if (input.Length > 1)
            {
                Location loc = new Location() { City = input[0], PostalCode = input[1], Street = _streetSearchText };
                IEnumerable<Location> streets = await Task.Run(() => locationRepository.GetLocations(loc));
                if (streets != null) Streets = new ObservableCollection<Location>(streets);
            }
            State = Views.QueryState.Finished;
            //Streets = new ObservableCollection<Location>(streets.ToList().GetRange(0, 50));
        }

        //Useless
        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            if (!comboBox.IsLoaded)
                return;
            Location loc;
            if (comboBox.SelectedItem is Location)
            {
                int index = comboBox.SelectedIndex;
                loc = comboBox.Items[index] as Location;
                LocationsToSearch.Add(loc);
            }
        }
        #endregion


        //Event handlers related to messages
        #region
        private async void GetMessages(object sender, SelectionChangedEventArgs e)
        {
            State = Views.QueryState.Executing;
            ComboBox comboBox = (ComboBox)sender;
            if (!comboBox.IsLoaded)
                return;
            if (comboBox.SelectedItem is Location)
            {
                int index = comboBox.SelectedIndex;
                Location loc = comboBox.Items[index] as Location;
                IEnumerable<Message> msgs = await Task.Run(() => locationRepository.FindMessages(loc));
                if (msgs != null) Messages = new ObservableCollection<Message>(msgs);
                else Messages.Clear();
            }
            State = Views.QueryState.Finished;
        }
        private async void FindMessages_Click(object sender, RoutedEventArgs e)
        {
            State = Views.QueryState.Executing;
            string[] input = _citySearchText.Split('-');
            if (input.Length > 1)
            {
                Location loc = new Location() { City = input[0], PostalCode = input[1] };
                IEnumerable<Message> msgs = await Task.Run(() => locationRepository.FindMessages(loc));
                if (msgs != null) Messages = new ObservableCollection<Message>(msgs);
                else Messages.Clear();
            }
            State = Views.QueryState.Finished;
        }
        #endregion

        //Event handlers related to customers
        #region
        private async void GetCustomers_Click(object sender, RoutedEventArgs e)
        {
            State = Views.QueryState.Executing;
            string[] input = _citySearchText.Split('-');
            if (input.Length > 1)
            {
                Location loc = new Location() { City = input[0], PostalCode = input[1], Street = StreetSearchText };
                Customer customer = new Customer() { Address = loc };
                IEnumerable<Customer> cstms = await Task.Run(() => customerRepository.GetCustomers(customer));
                if (cstms != null) Customers = new ObservableCollection<Customer>(cstms);
                else Customers.Clear();
            }
            State = Views.QueryState.Finished;
        }
        private async void CustomerBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            State = Views.QueryState.Executing;
            ComboBox comboBox = (ComboBox)sender;
            if (!comboBox.IsLoaded)
                return;
            Customer customer;
            if (comboBox.SelectedItem is Customer)
            {
                int index = comboBox.SelectedIndex;
                customer = await Task.Run(() => locationRepository.FindMessagesByCustomer(comboBox.Items[index] as Customer));
                if (customer != null) Messages = new ObservableCollection<Message>(customer.MsgReceived);
            }
            State = Views.QueryState.Finished;
        }
        #endregion
    }
    public enum QueryState
    {
        Standby = 0,
        Executing = 1,
        Finished = 2,
    }
}
