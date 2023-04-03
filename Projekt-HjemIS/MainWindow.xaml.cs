using Projekt_HjemIS.Models;
using Projekt_HjemIS.Systems;
using Projekt_HjemIS.Systems.Utility.Database_handling;
using Projekt_HjemIS.ViewModels;
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
    /// Hovedforfatter: Christian
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DatabaseHandler dbHandler = new DatabaseHandler();

        MainViewModel model = new MainViewModel();

        public MainWindow()
        {
            DataContext = model;

            InitializeComponent();
        }
        
        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var query = "SELECT CAST(CASE WHEN COUNT(*) > 0 THEN 1 ELSE 0 END AS bit) " +
                            "FROM Users " +
                            "WHERE Username = @Username " +
                            "AND [Password] = @Password";

                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@Username", username.Text),
                    //new SqlParameter("@Password", password.Password)
                };

                var exists = await dbHandler.ExistsAsync(query, parameters);

                if (exists)
                {
                    dashboard dashbord = new dashboard();
                    dashbord.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Username Or Password Is Incorrect, Please Try Again");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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

        private void password_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                btnLogin_Click(sender, e);
            }
        }
    }
}
