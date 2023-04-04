using System;
using System.Collections.Generic;
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
using System.Collections.ObjectModel;
using Projekt_HjemIS.Models;
using Projekt_HjemIS.Systems.Utility.Database_handling;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Projekt_HjemIS.Views
{
    /// <summary>
    /// Interaction logic for OfferViews.xaml
    /// </summary>
    public partial class EditCustomerView : UserControl
    {
        DatabaseHandler dh = new DatabaseHandler();

        public ObservableCollection<Customer> _customers { get; set; }

        public EditCustomerView()
        {
            InitializeComponent();

            RefreshGrid();
        }

        private void RefreshGrid()
        {
            try
            {
                var query = "SELECT * FROM Customers";

                _customers = new ObservableCollection<Customer>(dh.GetTable<Customer>(query));
                
                customerInfoGrid.ItemsSource = _customers;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void RefreshGrid(string query)
        {
            try
            {
                _customers = new ObservableCollection<Customer>(dh.GetTable<Customer>(query));

                customerInfoGrid.ItemsSource = _customers;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SearchCustomer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var query = "";

                if (firstName_search.Text != "" && lastName_search.Text == "")
                {
                    query = $"SELECT * from Customers WHERE FirstName = '{firstName_search.Text}'";
                }
                if (lastName_search.Text != "" && firstName_search.Text == "")
                {
                    query = $"SELECT * from Customers WHERE LastName = '{lastName_search.Text}'";
                }
                if (firstName_search.Text != "" && lastName_search.Text != "")
                {
                    query = $"SELECT * from Customers WHERE FirstName = '{firstName_search.Text}' AND LastName = '{lastName_search.Text}'";
                }
                if (firstName_search.Text == "" && lastName_search.Text == "")
                {
                    query = "SELECT * FROM Customers";
                }

                RefreshGrid(query);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SelectCustomer_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (customerInfoGrid.SelectedItem is Customer selectedCustomer)
                {
                    firstName.Text = selectedCustomer.FirstName;
                    lastName.Text = selectedCustomer.LastName;
                    phoneNum.Text = selectedCustomer.PhoneNumber.ToString();
                    streetCode.Text = selectedCustomer.StreetCode;
                    countyCode.Text = selectedCustomer.CountyCode;

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void UpdateCustomer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var phoneNumber = phoneNum.Text;

                string query = $"SELECT 1 FROM Customers WHERE PhoneNumber = {phoneNumber}";

                var exists = await dh.ExistsAsync(query);

                if (exists)
                {
                    UpdateCustomer();
                }
                else
                {
                    MessageBoxResult result = MessageBox.Show(
                        "Kunden er ikke fundet. Ønsker du at oprette en ny?", "Error", MessageBoxButton.YesNo);

                    if (result == MessageBoxResult.Yes)
                    {
                        CreateCustomer();
                    }
                }

                RefreshGrid();
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DeleteCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(phoneNum.Text, out int result))
            {
                DeleteCustomer(result);

                RefreshGrid();

                MessageBox.Show("Customer deleted");
            }
            else
            {
                MessageBox.Show("Invalid input");
            }
        }

        public void UpdateCustomer()
        {
            string query = "UPDATE Customers " +
                           "SET FirstName = @firstName, LastName = @lastName, PhoneNumber = @phoneNum, " +
                           "StreetCode = @streetCode, CountyCode = @countyCode " +
                           "WHERE PhoneNumber = @phoneNum AND CountyCode = @countyCode AND StreetCode = @streetCode";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@firstName", firstName.Text),
                new SqlParameter("@lastName", lastName.Text),
                new SqlParameter("@phoneNum", int.Parse(phoneNum.Text)),
                new SqlParameter("@streetCode", streetCode.Text),
                new SqlParameter("@countyCode", countyCode.Text)
            };

            dh.AddData(query, parameters.ToArray());
        }

        public void CreateCustomer()
        {
            /* 
             * A foreign key constraint conflict will occur when inserting a
             * Customer with a (StreetCode, CountyCode) combination that doesn't exist
             * in the Locations table
             */
            string query = "INSERT INTO Customers (FirstName, LastName, PhoneNumber, StreetCode, CountyCode) " +
                                "VALUES (@firstName, @lastName, @phoneNum, @streetCode, @countyCode);";
            //TODO: Add validation on PhoneNumber, StreetCode, and CountyCode

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@firstName", firstName.Text),
                new SqlParameter("@lastName", lastName.Text),
                new SqlParameter("@phoneNum", int.Parse(phoneNum.Text)),
                new SqlParameter("@streetCode", streetCode.Text),
                new SqlParameter("@countyCode", countyCode.Text),
            };

            dh.AddData(query, parameters.ToArray());
        }

        public void DeleteCustomer(int phoneNumber)
        {
            var query = $"DELETE FROM Customers WHERE PhoneNumber = {phoneNumber}";

            var result = dh.AddData(query);

            if (result > 0)
            {

            }
        }

        #region Validation

        private void NumericInputValidation(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        #endregion

    }
}
