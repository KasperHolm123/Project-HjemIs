using Projekt_HjemIS.Models;
using Projekt_HjemIS.Systems;
using Projekt_HjemIS.Systems.Utility.Database_handling;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
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
    /// Hovedforfatter: Christian
    /// Interaction logic for UsersViews.xaml
    /// </summary>
    public partial class UsersView : UserControl
    {
        //// Contains all available users.
        public ObservableCollection<UserNew> InternalUsers { get; set; }

        private DatabaseHandler dh = new DatabaseHandler();

        public UsersView()
        {
            InitializeComponent();
            InternalUsers = new ObservableCollection<UserNew>(dh.GetTable<UserNew>("SELECT * FROM Users"));
            //bind combobox
            comboUsers.ItemsSource = InternalUsers;
            comboUsers.DisplayMemberPath = $"{nameof(User.userUsername)}";
            FillDataGrid();
        }
        private void FillDataGrid()
        {
            SqlConnection connString = new SqlConnection(ConfigurationManager.ConnectionStrings["post"].ConnectionString);
            try
            {
                if (connString.State == ConnectionState.Closed)
                {
                    connString.Open();

                    string query = "SELECT [username], [admin] FROM Users";
                    SqlCommand cmd = new SqlCommand(query, connString);
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable("Users");
                    sda.Fill(dt);
                    dataUsers.ItemsSource = dt.DefaultView;
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
        private string MakeAdmin(string test)
        {
            SqlConnection connString = new SqlConnection(ConfigurationManager.ConnectionStrings["post"].ConnectionString);
            User user = comboUsers.SelectedItem as User;
            try
            {
                if (connString.State == ConnectionState.Closed)
                {
                    connString.Open();

                    SqlCommand sqlCommand = new SqlCommand("SELECT [admin] FROM Users WHERE username=@username", connString);
                    sqlCommand.Parameters.AddWithValue("@username", user.userUsername);

                    if ((bool)sqlCommand.ExecuteScalar() == false)
                    {
                        string query = "UPDATE Users SET [Admin] = 1 WHERE [Username]=@username";
                        SqlCommand sqlCommand1 = new SqlCommand(query, connString);
                        sqlCommand1.Parameters.AddWithValue("@username", test);
                        sqlCommand1.ExecuteNonQuery();
                    }
                    else if ((bool)sqlCommand.ExecuteScalar() == true)
                    {
                        string query = "UPDATE Users SET [Admin] = 0 WHERE [Username]=@username";
                        SqlCommand sqlCommand1 = new SqlCommand(query, connString);
                        sqlCommand1.Parameters.AddWithValue("@username", test);
                        sqlCommand1.ExecuteNonQuery();
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
            return test;
        }

        private void btnAdmin_Click(object sender, RoutedEventArgs e)
        {
            User test = comboUsers.SelectedItem as User;

            //string name = comboUsers.SelectionBoxItem.ToString();
            MakeAdmin(test.userUsername);
            FillDataGrid();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}
