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

        public List<Customer> messageRecipients { get; set; }

        public EmailViews()
        {
            InitializeComponent();

            // Setup collections
            InternalLocations = new ObservableCollection<Location>();

            // Bind comboboxes
            ComboTo.ItemsSource = InternalLocations;
            ComboOffers.ItemsSource = InternalProducts;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Message message = new Message(messageBodytxt.Text, messageRecipients);
        }

        private void ComboOffers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            InternalProducts.Add(ComboOffers.SelectedItem as Product);
        }

        private void ComboTo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            InternalLocations.Add(DatabaseHandler.GetLocation(ComboTo.Text) as Location);
        }

        public void OnPropertyChanged(string property = null)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }
    }
}
