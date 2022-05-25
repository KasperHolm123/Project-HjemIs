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
                mainGrid.ItemsSource= InternalProducts;
            }
            catch (Exception ex) { MessageBox.Show("No products found"); }
        }

        private void UpdateProduct(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "UPDATE Products " +
                    "SET Price = @price, Discount = @discount " +
                    "WHERE [Name] = @name AND ID = @ID";
                SqlParameter[] sp = new SqlParameter[]
                {
                    CreateParameter("@price", int.Parse(priceTxt.Text), SqlDbType.Int),
                    CreateParameter("@discount", int.Parse(discountTxt.Text), SqlDbType.Int),
                    CreateParameter("@name", nameTxt.Text, SqlDbType.NVarChar),
                    CreateParameter("@ID", int.Parse(idTxt.Text), SqlDbType.NVarChar),
                };
                PromptProductHandling(dh.AddData(query, sp));
                UpdateGrid();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void DeleteProduct(object sender, RoutedEventArgs e)
        {
            Product selectedProduct = mainGrid.SelectedItem as Product;
            try
            {
                string query = "DELETE FROM Products WHERE ID = @ID;";
                SqlParameter[] sp = new SqlParameter[]
                {
                    CreateParameter("@ID", selectedProduct.ID, SqlDbType.Int)
                };
                dh.AddData(query, sp);
                UpdateGrid();
                MessageBox.Show("Produkt slettet.");
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

            private void PromptProductHandling(int notFound)
        {
            if (notFound == 0)
            {
                MessageBoxResult msgResult = MessageBox.Show("Produkt ikke fundet. Ønsker du at oprette et nyt?", "Error", MessageBoxButton.YesNo);
                switch (msgResult)
                {
                    case MessageBoxResult.Yes:
                        string insertQuery = "INSERT INTO Products ([Name], Price, Discount) " +
                            "VALUES(@name, @price, @discount);";
                        SqlParameter[] insertSp = new SqlParameter[]
                        {
                            CreateParameter("@name", nameTxt.Text, SqlDbType.NVarChar),
                            CreateParameter("@price", int.Parse(priceTxt.Text), SqlDbType.Int),
                            CreateParameter("@discount", int.Parse(discountTxt.Text), SqlDbType.Int),
                        };
                        dh.AddData(insertQuery, insertSp);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                MessageBoxResult confirmBox = MessageBox.Show("Produkt opdateret.", "Success", MessageBoxButton.OK);
            }
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
                    discountedPriceTxt.Text = (selectedProduct.Price * (discountedPrice / 100)).ToString();
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        #region Validaton

        private void Validation_Numbers_Only(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        #endregion
    }
}
