using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
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
using System.Windows.Shapes;

namespace Projekt_HjemIS
{
    /// <summary>
    /// Interaction logic for ForgotPass.xaml
    /// </summary>
    public partial class ForgotPass : Window
    {
        public ForgotPass()
        {
            InitializeComponent();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            //emulator.
            string test = rUsername.Text;
            MessageBox.Show(Database(test));
        }
        public string Database(string fUsername)
        {
            SqlConnection connString = new SqlConnection(ConfigurationManager.ConnectionStrings["path"].ConnectionString);
            User pUser = new User();
            try
            {
                if (connString.State == ConnectionState.Closed)
                {
                    connString.Open();

                    string query = "SELECT [password] FROM Users WHERE username=@fUsername";
                    SqlCommand sqlCommand = new SqlCommand(query, connString);
                    sqlCommand.Parameters.AddWithValue("@fUsername", fUsername);
                    using (SqlDataReader sqlReader = sqlCommand.ExecuteReader())
                    {
                        while(sqlReader.Read())
                        {
                            pUser.fPassword = sqlReader["password"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                connString.Close();
            }
            return pUser.fPassword;
        }
    }
}
