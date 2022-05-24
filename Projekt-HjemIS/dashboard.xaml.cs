using Projekt_HjemIS.Models;
using Projekt_HjemIS.Systems;
using Projekt_HjemIS.Systems.Utility.Database_handling;
using Projekt_HjemIS.Views;
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
using System.Windows.Shapes;

namespace Projekt_HjemIS
{
    /// <summary>
    /// Interaction logic for dashboard.xaml
    /// </summary>
    public partial class dashboard : Window, INotifyPropertyChanged
    {
        DropzoneObserver dzObserver = new DropzoneObserver();
        DatabaseHandler dh = new DatabaseHandler();

        UserControl userControl = null;
        LocationRepository repository;
        private ObservableCollection<Location> _locations = new ObservableCollection<Location>();
        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<Location> Locations
        {
            get { return _locations; }
            set
            {
                _locations = value;
                if(userControl is LogView)
                {
                    LogView view = (LogView)userControl;
                    view.OnPropertyChanged("SearchCanExecute");                }
            }
        }
        public dashboard()
        {
            InitializeComponent();
            userControl = new HomeViews();
            repository = new LocationRepository();
            GridContent.Children.Add(userControl);
            lablUsername.Content = "Welcome " + User.Username.ToString();
            Loaded += Dashboard_Loaded;
            if (User.Admin == false)
            {
                _Users.IsEnabled = false;
            }
            if (User.Admin == false)
            {
                _Users.IsEnabled = false;
                _Users.Visibility = Visibility.Collapsed;
            }

            Task.Factory.StartNew(() => dzObserver.ObserveDropzone());
            // Setup customers
            dh.AddBulkData<Customer>(ListToDataTableConverter.ToDataTable(
                CustomerFactory.CreateNewCustomer()), "Customers");
        }
        private async void Dashboard_Loaded(object sender, RoutedEventArgs e)
        {
            //List<Location> locations = await Task.Run(() => repository.GetCities2());
            //locations.Sort((x, y) => string.Compare(x.City, y.City));
            //Validate  data
            //foreach (Location location in locations)
            //{
            //    _locations.Add(location);
            //}

            await Task.Delay(10000);
            _locations.Add(new Location() { City = "Skive", PostalCode = "7800" });
            _locations.Add(new Location() { City = "Holstebro", PostalCode = "7500" });
            _locations.Add(new Location() { City = "Gribskov", PostalCode = "3250" });
        }
        #region View Control
        private void _Offers_Click(object sender, RoutedEventArgs e)
        {
            userControl = new OfferViews();
            GridContent.Children.Clear();
            GridContent.Children.Add(userControl);
        }

        private void _Email_Click(object sender, RoutedEventArgs e)
        {
            userControl = new LogView(ref _locations);
            GridContent.Children.Clear();
            GridContent.Children.Add(userControl);
        }

        private void _Home_Click(object sender, RoutedEventArgs e)
        {
            userControl = new HomeViews();
            GridContent.Children.Clear();
            GridContent.Children.Add(userControl);
        }

        private void _SignOut_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            MessageBoxResult result = MessageBox.Show("Are You Sure You Want To Log Out?", "Log Out", MessageBoxButton.YesNo);
            switch(result)
            {
                case MessageBoxResult.Yes:
                    mainWindow.Show();
                    this.Close();
                    break;
                case MessageBoxResult.No:
                    break;
            }
        }

        private void _Users_Click(object sender, RoutedEventArgs e)
        {
            userControl = new UsersViews();
            GridContent.Children.Clear();
            GridContent.Children.Add(userControl);
        }
        private void OnPropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        private void _Emulator_Click(object sender, RoutedEventArgs e)
        {
            userControl = new EmulatorView();
            GridContent.Children.Clear();
            GridContent.Children.Add(userControl);
        }
        #endregion
    }
}
