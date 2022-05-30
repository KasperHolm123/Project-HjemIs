using Projekt_HjemIS.Models;
using Projekt_HjemIS.Systems;
using Projekt_HjemIS.Systems.Utility.Database_handling;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
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


        private DatabaseHandler dh = new DatabaseHandler();

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
        }



        private void ConnectMessage<T>(T type) where T : Message
        {
            string query = "INSERT INTO [Customers-Messages] (ID, PhoneNumber, [Date]) " +
                    "SELECT @messageID, PhoneNumber, GETDATE() FROM Customers WHERE " +
                    "CountyCode = @countyCode AND StreetCode = @streetCode;";
            foreach (Location location in RecipientsLocations) // IsRecipient virker stadig ikke.
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                    CreateParameter("@messageID", MessageHandler.SendMessages(type), SqlDbType.Int),
                    CreateParameter("@countyCode", location.CountyCode, SqlDbType.NVarChar),
                    CreateParameter("@streetCode", location.StreetCode, SqlDbType.NVarChar)
                };
                dh.AddData(query, parameters);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (RecipientsLocations.Count != 0)
                {
                    if ((bool)isSMS.IsChecked) // SMS
                    {
                        Message_SMS sms = new Message_SMS(messageBodytxt.Text, RecipientsLocations, "SMS");
                        ConnectMessage(sms);
                    }
                    else if ((bool)isMAIL.IsChecked) // Mail
                    {
                        Message_Mail mail = new Message_Mail(subjectTxt.Text, messageBodytxt.Text, RecipientsLocations, "Mail");
                        ConnectMessage(mail);
                    }
                    else if ((bool)isSMS.IsChecked && (bool)isMAIL.IsChecked) // Both
                    {
                        Message_Mail mail = new Message_Mail(subjectTxt.Text, messageBodytxt.Text, RecipientsLocations, "Mail");
                        ConnectMessage(mail);
                        Message_SMS sms = new Message_SMS(messageBodytxt.Text, RecipientsLocations, "SMS");
                        ConnectMessage(sms);
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                else
                {
                    throw new Exception();
                }
                MessageBox.Show("Besked sendt.");
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            if (SearchedLocations.Count != 0)
                SearchedLocations.Clear();
            if (searchTxt.Text != "" || searchTxt.Text != null)
            {
                string query = $@"SELECT TOP (100) * FROM Locations WHERE Street LIKE '%{searchTxt.Text}%';";
                SearchedLocations = new ObservableCollection<Location>(dh.GetTable<Location>(query));
                recipientsDataGrid.ItemsSource = SearchedLocations;
            }
            else
                recipientsDataGrid.ItemsSource = InternalLocations;
        }

        // Virker ikke
        private void recipientsDataGrid_LostFocus(object sender, RoutedEventArgs e)
        {
            Location loc = recipientsDataGrid.SelectedItem as Location;
            if (loc.IsRecipient)
                RecipientsLocations.Add(loc);
            else
                if (RecipientsLocations.Contains(loc))
                RecipientsLocations.Remove(loc);

        }

        /// <summary>
        /// Create parameter for SQL command.
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private static SqlParameter CreateParameter(string paramName, object value, SqlDbType type)
        {
            SqlParameter param = new SqlParameter
            {
                ParameterName = paramName,
                Value = value,
                SqlDbType = type
            };
            return param;
        }
    }
}
