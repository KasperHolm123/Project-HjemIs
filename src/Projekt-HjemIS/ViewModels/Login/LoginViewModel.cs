using Projekt_HjemIS.Models;
using Projekt_HjemIS.Services;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
        public RelayCommand ChangePageCommand { get; set; }

        #endregion

        public LoginViewModel(INavigationService navService)
        {
            _navigationService = navService;

            // Bind RelayCommands
            LoginCommand = new RelayCommand(p => Login());
            ChangePageCommand = new RelayCommand(p => ChangePage((string)p));
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
                    //LoginService.CurrentUser = await dh.GetEntry<Product>($"SELECT 1 FROM Products WHERE ID = {id}");
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

        private void ChangePage(string viewModel)
        {
            switch (viewModel)
            {
                case "create":
                    NavigationService.ChangeBaseView<CreateUserViewModel>();
                    break;
                case "forgot":
                    NavigationService.ChangeBaseView<ForgotPasswordViewModel>();
                    break;
                default:
                    break;
            }
        }
    }
}
