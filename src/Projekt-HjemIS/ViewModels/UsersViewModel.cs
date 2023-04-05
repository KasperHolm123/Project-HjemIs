using Projekt_HjemIS.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt_HjemIS.ViewModels
{
    public class UsersViewModel : BaseViewModel
    {
        #region Fields

        private ObservableCollection<UserNew> _users;
        public ObservableCollection<UserNew> Users
        {
            get => _users;
            set
            {
                _users = value;
                OnPropertyChanged();
            }
        }

        public UsersViewModel()
        {
            Users = new ObservableCollection<UserNew>(dh.GetTable<UserNew>("SELECT * FROM Users"));
        }

        private async Task Refresh()
        {

        }

        #endregion


    }
}
