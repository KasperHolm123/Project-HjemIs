using Projekt_HjemIS.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Projekt_HjemIS.ViewModels
{
    public class DashboardViewModel : BaseViewModel
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

        #region Commands

        public RelayCommand ChangePageCommand { get; set; }

        #endregion

        public DashboardViewModel(INavigationService navService)
        {
            _navigationService = navService;

            // Bind relay commands
            ChangePageCommand = new RelayCommand(p => ChangePage((string)p));
        }

        private void ChangePage(string viewModel)
        {
            switch (viewModel)
            {
                case "customer":
                    NavigationService.NavigateTo<EditCustomerViewModel>();
                    break;
                default:
                    break;
            }
        }
    }
}
