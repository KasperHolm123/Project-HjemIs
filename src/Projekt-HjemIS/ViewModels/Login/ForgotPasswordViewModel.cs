using Projekt_HjemIS.Models;
using Projekt_HjemIS.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Projekt_HjemIS.ViewModels
{
    public class ForgotPasswordViewModel : BaseViewModel
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

        #endregion

        #region Command

        public RelayCommand GoToPreviousPageCommand { get; set; }
        public RelayCommand RequestPasswordCommand { get; set; }

        #endregion

        public ForgotPasswordViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            GoToPreviousPageCommand = new RelayCommand(p => GoToPreviousPage());
            RequestPasswordCommand = new RelayCommand(p => RequestPassword());
        }

        private void GoToPreviousPage()
        {
            NavigationService.ChangeBaseView<LoginViewModel>();
        }

        private async Task RequestPassword()
        {
            var query = $"SELECT 1 FROM Users WHERE Username = '{Username}'";

            var exists = await dh.ExistsAsync(query);

            if (exists)
            {
                query = $"SELECT * FROM Users WHERE Username = '{Username}'";

                var result = await dh.GetEntry<UserNew>(query);

                MessageBox.Show(result.Password);
            }
            else
            {
                MessageBox.Show("No user with that username exists");
            }
        }
    }
}
