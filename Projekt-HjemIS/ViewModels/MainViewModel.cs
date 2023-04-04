using Projekt_HjemIS.Services;
using Projekt_HjemIS.Views;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace Projekt_HjemIS.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        #region Fields
        
        public RelayCommand ChangePageCmd { get; set; }

        private BaseViewModel _currentPage { get; set; }
        public BaseViewModel CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                OnPropertyChanged();
            }
        }

        #endregion

        public MainViewModel()
        {
            ChangePageCmd = new RelayCommand(p => ChangePage());
        }

        private void ChangePage()
        {
            CurrentPage = new EditCustomerViewModel();
        }
    }
}
