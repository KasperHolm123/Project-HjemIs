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
            InternalProducts = new ObservableCollection<Product>();//dh.GetTable<Product>("SELECT * FROM Products"));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
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
                    CreateParameter("@name", int.Parse(nameTxt.Text), SqlDbType.NVarChar),
                    CreateParameter("@ID", int.Parse(idTxt.Text), SqlDbType.NVarChar),
                };
                dh.AddData(query, sp);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
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
    }
}
