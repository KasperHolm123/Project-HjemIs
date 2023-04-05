using Projekt_HjemIS.Models;
using Projekt_HjemIS.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows;

namespace Projekt_HjemIS.ViewModels
{
    public class EditCustomerViewModel : BaseViewModel
    {
        #region Fields

        private ObservableCollection<Customer> _customers;
        public ObservableCollection<Customer> Customers
        {
            get => _customers;
            set
            {
                _customers = value;
                OnPropertyChanged();
            }
        }

        private Customer _selectedCustomer;
        public Customer SelectedCustomer
        {
            get => _selectedCustomer;
            set
            {
                _selectedCustomer = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Commands

        public RelayCommand SelectCustomerCommand { get; set; }
        public RelayCommand SubmitCustomerChangesCommand { get; set; }
        public RelayCommand DeleteCustomerCommand { get; set; }

        #endregion


        public EditCustomerViewModel()
        {
            SelectCustomerCommand = new RelayCommand(p => SelectCustomer(p));
            SubmitCustomerChangesCommand = new RelayCommand(p => SubmitCustomerChanges());
            DeleteCustomerCommand = new RelayCommand(p => DeleteCustomer());


            Refresh();

        }

        private void SelectCustomer(object customer)
        {
            if (customer is Customer)
            {
                SelectedCustomer = customer as Customer;
            }
        }

        private async Task SubmitCustomerChanges()
        {
            try
            {
                var phoneNumber = SelectedCustomer.PhoneNumber;

                string query = $"SELECT 1 FROM Customers WHERE PhoneNumber = {phoneNumber}";

                var exists = await dh.ExistsAsync(query);

                if (exists)
                {
                    UpdateCustomer();

                    MessageBox.Show("Customer updated successfully");
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

                Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
                new SqlParameter("@firstName", SelectedCustomer.FirstName),
                new SqlParameter("@lastName", SelectedCustomer.LastName),
                new SqlParameter("@phoneNum", SelectedCustomer.PhoneNumber),
                new SqlParameter("@streetCode", SelectedCustomer.StreetCode),
                new SqlParameter("@countyCode", SelectedCustomer.CountyCode)
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
                new SqlParameter("@firstName", SelectedCustomer.FirstName),
                new SqlParameter("@lastName", SelectedCustomer.LastName),
                new SqlParameter("@phoneNum", SelectedCustomer.PhoneNumber),
                new SqlParameter("@streetCode", SelectedCustomer.StreetCode),
                new SqlParameter("@countyCode", SelectedCustomer.CountyCode)
            };

            dh.AddData(query, parameters.ToArray());
        }

        public async Task DeleteCustomer()
        {
            var query = $"SELECT 1 FROM Customers WHERE PhoneNumber = {SelectedCustomer.PhoneNumber}";

            var exists = await dh.ExistsAsync(query);

            if (exists)
            {
                query = $"DELETE FROM Customers WHERE PhoneNumber = {SelectedCustomer.PhoneNumber}";

                dh.AddData(query);

                Refresh();

                MessageBox.Show("Customer deleted successfully");
            }
            else
            {
                MessageBox.Show("Customer does not exist");
            }
        }

        private void Refresh()
        {
            try
            {
                var query = "SELECT * FROM Customers";

                Customers = new ObservableCollection<Customer>(dh.GetTable<Customer>(query));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
