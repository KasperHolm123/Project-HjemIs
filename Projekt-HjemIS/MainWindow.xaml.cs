using Projekt_HjemIS.Models;
using Projekt_HjemIS.Systems;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
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

namespace Projekt_HjemIS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            RecordHandler record = new RecordHandler();

        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection connString = new SqlConnection(ConfigurationManager.ConnectionStrings["path"].ConnectionString);

            try
            {
                if (connString.State == ConnectionState.Closed)
                {
                    connString.Open();

                    string query = "SELECT COUNT(1) FROM Users WHERE username=@username AND [password]=@password";
                    SqlCommand sqlCommand = new SqlCommand(query, connString);
                    sqlCommand.CommandType = CommandType.Text;
                    sqlCommand.Parameters.AddWithValue("@username", username.Text);
                    sqlCommand.Parameters.AddWithValue("@password", password.Password);
                    int count = Convert.ToInt32(sqlCommand.ExecuteScalar());

                    if (count == 1)
                    {
                        User.Username = username.Text.ToString();
                        dashboard dashbord = new dashboard();
                        dashbord.Show();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Username Or Password Is Incorrect, Please Try Again");
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
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            Create create = new Create();
            create.Show();
            this.Close();
        }

        private void btnForgot_Click(object sender, RoutedEventArgs e)
        {
            ForgotPass forgotPass = new ForgotPass();
            forgotPass.Show();
            this.Close();
        }
    }
}
