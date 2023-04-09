using Projekt_HjemIS.Models;
using Projekt_HjemIS.Services;
using Projekt_HjemIS.Views;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Projekt_HjemIS.ViewModels
{
    public class MainViewModel : BaseViewModel
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

        #endregion

        public MainViewModel(INavigationService navService)
        {
            _navigationService = navService;
            NavigationService.ChangeBaseView<LoginViewModel>();
            var result = MessageBox.Show("Data detected", "Notice!", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                dataService.ProcessDataAsync();
            }
        }
    }
}
