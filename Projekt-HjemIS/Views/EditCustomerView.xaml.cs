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

            UpdateGrid();
        }

        private void UpdateGrid()
        {
            try
            {
                _customers = new ObservableCollection<Customer>(dh.GetTable<Customer>("SELECT FirstName, LastName, PhoneNumber FROM Customers"));
                customerInfoGrid.ItemsSource = _customers;
            }
            catch (Exception ex) { MessageBox.Show($"Oops, something went wrong. Error code: {ex}"); }
        }
    }
}
