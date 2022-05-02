using System;
using System.Collections.Generic;
using System.Configuration;
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
using System.Windows.Shapes;

namespace Projekt_HjemIS
{
    /// <summary>
    /// Interaction logic for Create.xaml
    /// </summary>
    public partial class Create : Window
    {
        public Create()
        {
            InitializeComponent();
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection connString = new SqlConnection(ConfigurationManager.ConnectionStrings["path"].ConnectionString);
            connString.Open();
            try
            {
                if (Username.Text == "" || Username.Text == "Username" || Password.Password == "" || Password2.Password == "")
                {
                    MessageBox.Show("Please Fill Out The Required Fields");
                    if (Username.Text == "" || Username.Text == "Username")
                    {
                        Username.BorderBrush = Brushes.Red;
                    }
                    if (Password.Password == "")
                    {
                        Password.BorderBrush = Brushes.Red;
                    }
                    if (Password2.Password == "")
                    {
                        Password2.BorderBrush = Brushes.Red;
                    }
                    
                }
                else if (Password.Password != Password2.Password)
                {
                    MessageBox.Show("Your Password Does Not Match");
                }
                else
                {
                    bool exists = false;
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) from Users WHERE [username] = @username", connString))
                    {
                        cmd.Parameters.AddWithValue("username", Username.Text);
                        exists = (int)cmd.ExecuteScalar() > 0;
                    }

                    if (exists)
                    {
                        MessageBox.Show($"This Username {Username.Text} Already Exists");
                    }
                    else
                    {
                        string b = string.Empty;
                        
                        SqlCommand command = new SqlCommand($"INSERT INTO Users (username, [Password]) VALUES ('{Username.Text}', '{Password.Password}')", connString);
                        command.ExecuteNonQuery();
                        MessageBoxResult result = MessageBox.Show("User Has Been Created Succesfully", "User Record", MessageBoxButton.OK, MessageBoxImage.Information);
                        if (result == MessageBoxResult.OK)
                        {
                            this.Close();
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            finally
            {
                connString.Close();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
