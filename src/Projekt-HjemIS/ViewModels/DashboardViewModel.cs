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
                case "home":
                    NavigationService.NavigateTo<HomeViewModel>();
                    break;
                case "send":
                    NavigationService.NavigateTo<EmailViewModel>();
                    break;
                case "customer":
                    NavigationService.NavigateTo<EditCustomerViewModel>();
                    break;
                case "users":
                    NavigationService.NavigateTo<UsersViewModel>();
                    break;
                case "products":
                    NavigationService.NavigateTo<ProductsViewModel>();
                    break;
                case "emulator":
                    NavigationService.NavigateTo<EmulatorViewModel>();
                    break;
                case "log":
                    // TODO: fix

                    //NavigationService.NavigateTo<LogViewModel>();
                    break;
                default:
                    break;
            }
        }
    }
}
