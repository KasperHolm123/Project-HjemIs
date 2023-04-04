using Projekt_HjemIS.Models;
using Projekt_HjemIS.Services;
using Projekt_HjemIS.Views;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.PeerToPeer;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Projekt_HjemIS.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        #region Fields
        
        private INavigationService _navigationService;
        public INavigationService NavigationService
        {
            get => _navigationService;
            set
            {
                _navigationService = value;
                OnPropertyChanged();
            }
        }

        private string _username = "";
        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }

        private string _password = "";
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Commands

        public RelayCommand LoginCommand { get; set; }

        #endregion

        public LoginViewModel(INavigationService navService)
        {
            _navigationService = navService;

            // Bind RelayCommands
            LoginCommand = new RelayCommand(p => Login());
        }

        private async Task Login()
        {
            try
            {
                var query = "SELECT 1 " +
                            "FROM Users " +
                            "WHERE Username = @Username " +
                "AND [Password] = @Password";

                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@Username", Username),
                    new SqlParameter("@Password", Password)
                };

                var exists = await dh.ExistsAsync(query, parameters);

                if (exists)
                {
                    NavigationService.ChangeBaseView<DashboardViewModel>();
                }
                else
                {
                    MessageBox.Show("Incorrect credentials");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
