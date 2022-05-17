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
    public partial class EmailViews : UserControl
    {

        // Contains all available locations.
        public ObservableCollection<Location> InternalLocations { get; set; }
        // Contains all filtered locations.
        public ObservableCollection<Location> SearchedLocations { get; set; }
        // Contains all available products
        public ObservableCollection<Product> InternalProducts { get; set; } //= DatabaseHandler.GetProducts(); // mangler metode
        // Locations to be treated as recipients
        public List<Location> RecipientsLocations { get; set; }

        public List<Customer> messageRecipients { get; set; }

        public EmailViews()
        {
            InitializeComponent();

            // Setup collections
            InternalLocations = new ObservableCollection<Location>(RecordHandler.GetRecords());
            SearchedLocations = new ObservableCollection<Location>();
            RecipientsLocations = new List<Location>();

            // Bind comboboxes
            recipientsDataGrid.ItemsSource = InternalLocations;

            ComboOffers.ItemsSource = InternalProducts;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)isSMS.IsChecked) // SMS
            {
                Message_SMS sms = new Message_SMS(messageBodytxt.Text, RecipientsLocations);
                MessageHandler.SendMessages(sms);
            }
            if ((bool)isMAIL.IsChecked) // Mail
            {
                Message_Mail mail = new Message_Mail(subjectTxt.Text, messageBodytxt.Text, RecipientsLocations);
                MessageHandler.SendMessages(mail);
            }
            if ((bool)isSMS.IsChecked && (bool)isMAIL.IsChecked) // Both
            {
                Message_Mail mail = new Message_Mail(subjectTxt.Text, messageBodytxt.Text, RecipientsLocations);
                MessageHandler.SendMessages(mail);
                Message_SMS sms = new Message_SMS(messageBodytxt.Text, RecipientsLocations);
                MessageHandler.SendMessages(sms);
            }
        }

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            if (SearchedLocations.Count != 0)
                SearchedLocations.Clear();
            if (searchTxt.Text == "" || searchTxt.Text == null)
            {
                DatabaseHandler.GetLocation(SearchedLocations, searchTxt.Text);
                recipientsDataGrid.ItemsSource = SearchedLocations;
            }
        }

        private void recipientsDataGrid_LostFocus(object sender, RoutedEventArgs e)
        {
            Location loc = recipientsDataGrid.SelectedItem as Location;
            if (loc.IsRecipient)
                RecipientsLocations.Add(loc);
            else
                if (RecipientsLocations.Contains(loc))
                    RecipientsLocations.Remove(loc);
                    
        }
    }
}
