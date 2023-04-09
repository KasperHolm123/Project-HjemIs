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
            SubmitProductChangesCommand = new RelayCommand(p => SubmitProductChangesAsync((Product)p));
            DeleteProductCommand = new RelayCommand(p => DeleteProductAsync());
            ClearSelectedProductCommand = new RelayCommand(p => ClearSelectedProduct());

            SelectedProduct = new Product();

            Task.Run(() => RefreshAsync());
        }

        private async Task SubmitProductChangesAsync(Product product)
        {
            // temp setup of product
            product.DiscountedPrice = product.Price * (1 - (product.Discount / 100));

            var query = $"SELECT 1 FROM product WHERE ID = {product.ID}";

            var exists = await dh.ExistsAsync(query);

            if (exists)
            {
                await UpdateProductAsync(product);
            }
            else
            {
                var result = MessageBox.Show(
                    "Produkt ikke fundet. Ønsker du at oprette et nyt?", "Error", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    await CreateProductAsync(product);
                }
            }

            await RefreshAsync();
        }

        private async Task UpdateProductAsync(Product product)
        {
            string query = "UPDATE product " +
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

            await dh.AddDataAsync(query, parameters.ToArray());

            MessageBox.Show("Product updated!");
        }

        private async Task CreateProductAsync(Product product)
        {
            string query = "INSERT INTO product ([Name], Price, Discount, DiscountedPrice) " +
                            "VALUES(@Name, @Price, @Discount, @DiscountedPrice);";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Name", product.Name),
                new SqlParameter("@Price", product.Price),
                new SqlParameter("@Discount", product.Discount),
                new SqlParameter("@DiscountedPrice", product.DiscountedPrice)
            };

            await dh.AddDataAsync(query, parameters.ToArray());
        }

        private async Task DeleteProductAsync()
        {
            string query = $"DELETE FROM product WHERE ID = {SelectedProduct.ID}";

            await dh.AddDataAsync(query);

            await RefreshAsync();

            MessageBox.Show("Produkt slettet.");
        }

        private void SelectProduct(Product product)
        {
            SelectedProduct = product;
        }

        private async Task RefreshAsync()
        {
            try
            {
                var products = await dh.GetTable<Product>("SELECT * FROM product");
                Products = new ObservableCollection<Product>(products);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ClearSelectedProduct()
        {
            SelectedProduct = new Product();
        }
    }
}
