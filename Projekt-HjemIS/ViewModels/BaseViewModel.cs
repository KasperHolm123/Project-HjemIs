using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Projekt_HjemIS.Systems.Utility.Database_handling;
using System.Windows.Input;
using System.Configuration;
using System.ComponentModel;

namespace Projekt_HjemIS.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public static readonly DatabaseHandler dh = new DatabaseHandler();
        public static readonly SqlConnection connection = new SqlConnection(
            ConfigurationManager.ConnectionStrings["post"].ConnectionString);

        public event PropertyChangedEventHandler PropertyChanged;

        public static SqlParameter CreateParameter(string paramName, object value, SqlDbType type)
        {
            SqlParameter param = new SqlParameter
            {
                ParameterName = paramName,
                Value = value,
                SqlDbType = type
            };

            return param;
        }

        public void OnPropertyChanged(string property = null)
        {
            if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }

    public class RelayCommand : ICommand
    {
        private Action<object> execute;
        private Func<object, bool> canExecute;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return this.canExecute == null || this.canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            this.execute(parameter);
        }
    }
}
