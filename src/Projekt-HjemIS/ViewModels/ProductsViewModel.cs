using Projekt_HjemIS.Models;
using Projekt_HjemIS.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Projekt_HjemIS.ViewModels
{
    public class ProductsViewModel : BaseViewModel
    {
        #region Fields

        private ObservableCollection<Product> _products;
        public ObservableCollection<Product> Products
        {
            get => _products;
            set
            {
                _products = value;
                OnPropertyChanged();
            }
        }

        private Product _selectedProduct;
        public Product SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                _selectedProduct = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Commands

        public RelayCommand SelectProductCommand { get; set; }
        public RelayCommand SubmitProductChangesCommand { get; set; }
        public RelayCommand DeleteProductCommand { get; set; }
        public RelayCommand ClearSelectedProductCommand { get; set; }

        #endregion

        public ProductsViewModel()
        {
            SelectProductCommand = new RelayCommand(p => SelectProduct((Product)p));
            SubmitProductChangesCommand = new RelayCommand(p => SubmitProductChanges((Product)p));
            DeleteProductCommand = new RelayCommand(p => DeleteProduct());
            ClearSelectedProductCommand = new RelayCommand(p => ClearSelectedProduct());

            SelectedProduct = new Product();

            Refresh();
        }

        private async Task SubmitProductChanges(Product product)
        {
            // temp setup of product
            product.DiscountedPrice = product.Price * (1 - (product.Discount / 100));

            var query = $"SELECT 1 FROM Products WHERE ID = {product.ID}";

            var exists = await dh.ExistsAsync(query);

            if (exists)
            {
                UpdateProduct(product);
            }
            else
            {
                var result = MessageBox.Show(
                    "Produkt ikke fundet. Ønsker du at oprette et nyt?", "Error", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    CreateProduct(product);
                }
            }

            Refresh();
        }

        private void UpdateProduct(Product product)
        {
            string query = "UPDATE Products " +
                    "SET [Name] = @Name, Price = @Price, Discount = @Discount, DiscountedPrice = @DiscountedPrice " +
                    "WHERE ID = @ID";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Name", product.Name),
                new SqlParameter("@Price", product.Price),
                new SqlParameter("@Discount", product.Discount),
                new SqlParameter("@DiscountedPrice", product.DiscountedPrice),
                new SqlParameter("@ID", product.ID)
            };

            dh.AddData(query, parameters.ToArray());

            MessageBox.Show("Product updated!");
        }

        private void CreateProduct(Product product)
        {
            string query = "INSERT INTO Products ([Name], Price, Discount, DiscountedPrice) " +
                            "VALUES(@Name, @Price, @Discount, @DiscountedPrice);";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Name", product.Name),
                new SqlParameter("@Price", product.Price),
                new SqlParameter("@Discount", product.Discount),
                new SqlParameter("@DiscountedPrice", product.DiscountedPrice)
            };

            dh.AddData(query, parameters.ToArray());
        }

        private async Task DeleteProduct()
        {
            string query = $"DELETE FROM Products WHERE ID = {SelectedProduct.ID}";

            dh.AddData(query);

            Refresh();

            MessageBox.Show("Produkt slettet.");
        }

        private void SelectProduct(Product product)
        {
            SelectedProduct = product;
        }

        private void Refresh()
        {
            try
            {
                Products = new ObservableCollection<Product>(dh.GetTable<Product>("SELECT * FROM Products"));
            }
            catch (Exception ex)
            {
                MessageBox.Show("No products found");
            }
        }

        private void ClearSelectedProduct()
        {
            SelectedProduct = new Product();
        }
    }
}
