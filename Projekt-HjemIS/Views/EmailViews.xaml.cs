using Projekt_HjemIS.Models;
using Projekt_HjemIS.Systems;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
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
    /// Interaction logic for EmailViews.xaml
    /// </summary>
    public partial class EmailViews : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Location> InternalLocations { get; set; }
        public ObservableCollection<Product> InternalProducts { get; set; } //= DatabaseHandler.GetProducts(); // mangler metode
        public ObservableCollection<Location> ChosenLocations { get; set; }

        public List<Customer> messageRecipients { get; set; }

        public EmailViews()
        {
            InitializeComponent();

            // Setup collections
            InternalLocations = new ObservableCollection<Location>();
            ChosenLocations = new ObservableCollection<Location>();

            // Bind comboboxes
            recipientsDataGrid.ItemsSource = ChosenLocations;

            ComboTo.ItemsSource = InternalLocations;
            ComboTo.DisplayMemberPath = $"{nameof(Location.Street)}";
            ComboOffers.ItemsSource = InternalProducts;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Message message = new Message(messageBodytxt.Text, messageRecipients);
        }

        public void OnPropertyChanged(string property = null)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        private void ComboTo_TextChanged(object sender, TextChangedEventArgs e)
        {
            DatabaseHandler.GetLocation(InternalLocations, ComboTo.Text);
            Debug.WriteLine(InternalLocations.Count);
        }
    }
}
