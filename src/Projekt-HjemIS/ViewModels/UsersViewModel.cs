using Projekt_HjemIS.Models;
using Projekt_HjemIS.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Projekt_HjemIS.ViewModels
{
    public class UsersViewModel : BaseViewModel
    {
        #region Fields

        private ObservableCollection<User> _users;
        public ObservableCollection<User> Users
        {
            get => _users;
            set
            {
                _users = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Commands

        public RelayCommand ChangeAdminStatusCommand { get; set; }

        #endregion

        public UsersViewModel()
        {
            ChangeAdminStatusCommand = new RelayCommand(p => ChangeAdminStatus((User)p));


            Refresh();
        }

        private async void ChangeAdminStatus(User user)
        {
            var query = $"SELECT 1 FROM Users WHERE Username = '{user.Username}'";

            var exists = await dh.ExistsAsync(query);

            if (exists)
            {
                MessageBoxResult result;
                if (!user.Admin)
                {
                    result = MessageBox.Show(
                        "Are you sure you want to make this user an admin?", "Caution", MessageBoxButton.YesNo);
                }
                else
                {
                    result = MessageBox.Show(
                        "Are you sure you want to remove this user's admin status?", "Caution", MessageBoxButton.YesNo);
                }

                if (result == MessageBoxResult.Yes)
                {
                    if (!user.Admin)
                    {
                        query = $"UPDATE Users SET [Admin] = 1 WHERE Username = '{user.Username}'";

                        dh.AddData(query);

                        MessageBox.Show($"{user.Username} is now an administrator");
                    }
                    else
                    {
                        query = $"UPDATE Users SET [Admin] = 0 WHERE Username = '{user.Username}'";

                        dh.AddData(query);

                        MessageBox.Show($"{user.Username} is no longer an administrator");
                    }

                    await Refresh();
                }
            }
        }

        private async Task Refresh()
        {
            Users = new ObservableCollection<User>(dh.GetTable<User>("SELECT * FROM Users"));
        }
    }
}
