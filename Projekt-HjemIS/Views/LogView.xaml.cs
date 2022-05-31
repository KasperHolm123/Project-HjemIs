using Projekt_HjemIS.Models;
using Projekt_HjemIS.Systems;
using System;
using System.Collections;
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
        private LogViewRepository locationRepository;
        private CustomerRepository customerRepository;
        private ObservableCollection<Location> _locations;
        private ObservableCollection<Location> _streets;
        private ObservableCollection<Message> _messages;
        private ObservableCollection<Customer> _customers;
        private bool SortOrder;
        public CollectionViewSource FilteredView { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

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
                if (FilteredView.View != null) FilteredView.Source = Messages;
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

        //Text-field properties
        #region
        private DateTime _endDate;
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
            get { return (CitySearchText.Contains('-')) ; }
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

        public DateTime EndDate
        {
            get { return _endDate; }
            set
            {
                _endDate = value;
                OnPropertyChanged("EndDate");
            }
        }

        #endregion
        public LogView(ref ObservableCollection<Location> locations)
        {
            InitializeComponent();
            DataContext = this;
            locationRepository = new LogViewRepository(); 
            Locations = locations;
            customerRepository = new CustomerRepository();
            FilteredView = new CollectionViewSource();
            Messages = new ObservableCollection<Message>();
            FilteredView.Source = Messages;
            FilterAndSort();
        }
        private void FilterAndSort()
        {
            FilteredView.Filter += (s, e) =>
            {
                Message msg = e.Item as Message;
                e.Accepted = false;
                if (msg.Date == null) e.Accepted = true;
                if (msg.Date.Date >= _endDate.Date && DateTime.Now.Date >= msg.Date.Date) e.Accepted = true;
                if (msg.Date.Date <= _endDate.Date && DateTime.Now.Date <= msg.Date.Date) e.Accepted = false;
            };
            ListCollectionView view = FilteredView.View as ListCollectionView;
            view.CustomSort = new CustomMessageComparer();
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

        #endregion


        //Event handlers related to messages
        #region
        private void SortList_Click(object sender, RoutedEventArgs e)
        {
            MessageLog.Items.SortDescriptions.Clear();
            if (SortOrder)
            {
                MessageLog.Items.SortDescriptions.Add(new SortDescription("Date", ListSortDirection.Ascending));
                SortOrder = false;
            }
            else
            {
                MessageLog.Items.SortDescriptions.Add(new SortDescription("Date", ListSortDirection.Descending));
                SortOrder = true;
            }
        }
        private void OnDateChanged(object sender, SelectionChangedEventArgs e)
        {
            FilteredView.View.Refresh();
        }
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
                Location loc = new Location() { City = input[0], PostalCode = input[1], Street = StreetSearchText };
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
                Location loc = new Location() { City = input[0], PostalCode = input[1], Street = _streetSearchText };
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

        #region
        protected class CustomMessageComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                Message msg1 = (Message)x;
                Message msg2 = (Message)y;
                if (msg1.Date > msg2.Date) return -1;
                else if (msg1.Date < msg2.Date) return 1;
                else return 0;
            }
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
