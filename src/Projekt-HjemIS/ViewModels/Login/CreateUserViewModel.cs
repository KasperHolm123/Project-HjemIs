using Projekt_HjemIS.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Projekt_HjemIS.ViewModels
{
    public class CreateUserViewModel : BaseViewModel
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

        private string _userName;
        public string Username
        {
            get => _userName;
            set
            {
                _userName = value;
                OnPropertyChanged();
            }
        }

        private string _password;
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

        #region Command

        public RelayCommand GoToPreviousPageCommand { get; set; }
        public RelayCommand CreateUserCommand { get; set; }

        #endregion

        public CreateUserViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            GoToPreviousPageCommand = new RelayCommand(p => GoToPreviousPage());
            CreateUserCommand = new RelayCommand(p => CreateUser());
        }

        private void GoToPreviousPage()
        {
            NavigationService.ChangeBaseView<LoginViewModel>();
        }

        private async Task CreateUser()
        {
            var query = $"SELECT 1 from Users WHERE [username] = '{Username}'";

            var exists = await dh.ExistsAsync(query);

            if (!exists)
            {
                query = $"INSERT INTO Users (Username, [Password], [Admin]) VALUES ('{Username}', '{Password}', '0')";

                dh.AddData(query);

                MessageBox.Show("User created successfully");
                NavigationService.ChangeBaseView<LoginViewModel>();
            }
            else
            {
                MessageBox.Show("User with that username already exists");
            }
        }
    }
}
