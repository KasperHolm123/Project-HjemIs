using Projekt_HjemIS.Models;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows;


namespace Projekt_HjemIS
{
    /// <summary>
    /// Hovedforfatter: Christian
    /// Interaction logic for ForgotPass.xaml
    /// </summary>
    public partial class ForgotPasswordView : Window
    {
        public ForgotPasswordView()
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
            MessageBox.Show(RequestPass(test));
        }
        public string RequestPass(string fUsername)
        {
            SqlConnection connString = new SqlConnection(ConfigurationManager.ConnectionStrings["post"].ConnectionString);
            User fUser = new User(fUsername);
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
                            fUser.fPassword = sqlReader["password"].ToString();
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
            return fUser.fPassword;
        }

        public string ToFile(string f)
        {
            string fileName = @"Emulator\test.txt";
            try
            {
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
            }
            catch
            {
                
            }
            finally
            {

            }
            return f;
        }
    }
}
