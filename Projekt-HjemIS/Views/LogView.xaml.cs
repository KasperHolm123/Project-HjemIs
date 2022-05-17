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
        private LocationRepository repository;
        private ObservableCollection<Location> _locations;
        private ObservableCollection<Location> _streets;
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
        public List<Location> InternalLocations { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        private List<Location> _internalStreets = new List<Location>();
        private ObservableCollection<Location> _locationsToSearch;
        public ObservableCollection<Location> LocationsToSearch
        {
            get { return _locationsToSearch; }
            set
            {
                _locationsToSearch = value;
                OnPropertyChanged("LocationsToSearch");
            }
        }
        string _streetSearchText = "";
        string _citySearchText = "";
        string _postSearchText = "";
        public List<Location> InternalStreets
        {
            get { return _internalStreets; }
            set
            {
                _internalStreets = value;
                OnPropertyChanged("SearchCanExecute");
                Streets = new ObservableCollection<Location>(_internalStreets.Where(p => FilterLocations(p)));
            }
        }
        public string CitySearchText
        {
            get { return _citySearchText; }
            set
            {
                _citySearchText = value;
                if(InternalLocations != null ) Locations = new ObservableCollection<Location>(InternalLocations.Where(p => FilterLocations(p)));
                OnPropertyChanged("CitySearchText");
                OnPropertyChanged("CanExecute");
            }
        }
        public string PostSearchText
        {
            get { return _postSearchText; }
            set
            {
                _postSearchText = value;
                if (InternalLocations != null) Locations = new ObservableCollection<Location>(InternalLocations.Where(p => FilterLocations(p)));
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
                Streets = new ObservableCollection<Location>(InternalStreets.Where(p => FilterStreets(p)));
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
        public LogView(ref List<Location> locations)
        {
            InitializeComponent();
            DataContext = this;
            repository = new LocationRepository(); 
            InternalLocations = locations;
            LocationsToSearch = new ObservableCollection<Location>();
        }
        private bool FilterStreets(Location loc)
        {
            if (loc.Street.StartsWith(_streetSearchText)) return true;
            else return false;
        }
        private bool FilterLocations(Location loc)
        {
            if (loc.City.StartsWith(_citySearchText) && loc.PostalCode.StartsWith(_postSearchText)) return true;
            else return false;
        }
        public void OnPropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        private async void FindStreets_Click(object sender, RoutedEventArgs e)
        {
            Location loc = new Location() { City = _citySearchText, PostalCode = _postSearchText, Street = _streetSearchText };
            IEnumerable<Location> streets = await Task.Run(()=>repository.GetLocations(loc));
            InternalStreets = streets.ToList();
            //Streets = new ObservableCollection<Location>(streets.ToList().GetRange(0, 50));
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox box = sender as ComboBox;
            Location loc;
            if (box.SelectedItem is Location)
            {
                int index = box.SelectedIndex;
                loc = box.Items[index] as Location;
                LocationsToSearch.Add(loc);
                CityBox.Text = loc.City;
                PostalBox.Text = loc.PostalCode;
            }
        }
    }
}
