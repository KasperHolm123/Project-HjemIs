using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Projekt_HjemIS.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        #region Fields
        public RelayCommand LoginCmd { get; set; }

        private string _username;
        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }

        #endregion

        public MainViewModel()
        {
            LoginCmd = new RelayCommand(p => Login());
        }

        private async Task Login()
        {
            var query = "SELECT CAST(CASE WHEN COUNT(*) > 0 THEN 1 ELSE 0 END AS bit) " +
                            "FROM Users " +
                            "WHERE Username = 'admin'" +
                            "AND [Password] = 'admin'";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Username", Username),
                new SqlParameter("@Password", Password)
            };

            var exists = await dh.ExistsAsync(query);

            if (exists)
            {
                Debug.WriteLine("yeet");
            }
        }
    }
}
