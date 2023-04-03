using Projekt_HjemIS.Models;
using Projekt_HjemIS.Systems.Utility.Database_handling;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Hovedforfatter: Kasper 
    /// Interaction logic for ProductsView.xaml
    /// </summary>
    public partial class ProductsView : UserControl
    {
        DatabaseHandler dh = new DatabaseHandler();

        public ObservableCollection<Product> InternalProducts { get; set; }

        public ProductsView()
        {
            InitializeComponent();

            //Setup collections
            UpdateGrid();
        }

        private void UpdateGrid()
        {
            try
            {
                InternalProducts = new ObservableCollection<Product>(dh.GetTable<Product>("SELECT * FROM Products"));

                mainGrid.ItemsSource = InternalProducts;
            }
            catch (Exception ex)
            {
                MessageBox.Show("No products found");
            }
        }

        private async void UpdateProduct_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "SELECT 1 FROM Products WHERE [Name] = @Name AND ID = @ID";

                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@Name", nameTxt.Text),
                    new SqlParameter("@ID", idTxt.Text)
                };

                var exists = await dh.ExistsAsync(query, parameters);

                if (exists)
                {
                    UpdateProduct();
                }
                else
                {
                    var result = MessageBox.Show(
                        "Produkt ikke fundet. Ønsker du at oprette et nyt?", "Error", MessageBoxButton.YesNo);

                    if (result == MessageBoxResult.Yes)
                    {
                        CreateProduct();
                    }
                }

                UpdateGrid();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public void UpdateProduct()
        {
            string query = "UPDATE Products " +
                    "SET Price = @price, Discount = @discount " +
                    "WHERE [Name] = @name AND ID = @ID";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@price", int.Parse(priceTxt.Text)),
                new SqlParameter("@discount", int.Parse(discountTxt.Text)),
                new SqlParameter("@name", nameTxt.Text),
                new SqlParameter("@ID", int.Parse(idTxt.Text)),
            };

            dh.AddData(query, parameters.ToArray());

            UpdateGrid();
        }

        public void CreateProduct()
        {
            string query = "INSERT INTO Products ([Name], Price, Discount) " +
                            "VALUES(@name, @price, @discount);";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@name", nameTxt.Text),
                new SqlParameter("@price", int.Parse(priceTxt.Text)),
                new SqlParameter("@discount", int.Parse(discountTxt.Text)),
            };

            dh.AddData(query, parameters.ToArray());

            UpdateGrid();
        }

        private void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            Product selectedProduct = mainGrid.SelectedItem as Product;
            
            try
            {
                string query = "DELETE FROM Products WHERE ID = @ID;";

                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@ID", selectedProduct.ID)
                };
                
                dh.AddData(query, parameters.ToArray());
                
                UpdateGrid();
                
                MessageBox.Show("Produkt slettet.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void mainGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Product selectedProduct = mainGrid.SelectedItem as Product;
                if (selectedProduct != null)
                {
                    nameTxt.Text = selectedProduct.Name;
                    idTxt.Text = selectedProduct.ID.ToString();
                    priceTxt.Text = selectedProduct.Price.ToString();
                    discountTxt.Text = selectedProduct.Discount.ToString();

                    decimal discountedPrice = decimal.Parse(discountTxt.Text);
                    discountedPriceTxt.Text = (selectedProduct.Price * (1 - (discountedPrice / 100))).ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #region Validaton

        private void Validation_Numbers_Only(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        #endregion

        private void ClearSelection_Click(object sender, RoutedEventArgs e)
        {
            nameTxt.Clear();
            idTxt.Text = "0";
            priceTxt.Clear();
            discountTxt.Clear();
            discountedPriceTxt.Clear();
        }
    }
}
