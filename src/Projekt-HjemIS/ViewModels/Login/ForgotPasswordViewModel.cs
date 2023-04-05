using Projekt_HjemIS.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        #endregion

        #region Command

        public RelayCommand GoToPreviousPageCommand { get; set; }

        #endregion

        public ForgotPasswordViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            GoToPreviousPageCommand = new RelayCommand(p => GoToPreviousPage());
        }

        private void GoToPreviousPage()
        {
            NavigationService.ChangeBaseView<LoginViewModel>();
        }
    }
}
