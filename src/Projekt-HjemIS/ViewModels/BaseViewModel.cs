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
using Projekt_HjemIS.Services;
using System.IO;
using System.Security.Cryptography.Xml;

namespace Projekt_HjemIS.ViewModels
{
    public class BaseViewModel : ObservableObject, INotifyPropertyChanged
    {
        
        private bool _isBusy;
        /// <summary>
        /// used to keep track of whether or not any actions that might
        /// prevent the user from interacting with the UI, or do any
        /// database operations are currently active.
        /// </summary>
        public bool IsBusy
        {
            get => _isBusy;
            protected set
            {
                _isBusy = value;
                OnPropertyChanged();
            }
        }

        public static readonly DatabaseHandler dh = new DatabaseHandler();
        public static readonly DataProcessorService dataService = new DataProcessorService();
        public static LoginService LoginService = new LoginService();

        /// <summary>
        /// Returns current directory.
        /// </summary>
        /// <returns></returns>
        protected static string GetCurrentDirectory()
        {
            var rootPathChild = Directory.GetCurrentDirectory();
            var rootPathParent = Directory.GetParent($"{rootPathChild}");
            var rootPathFolder = Directory.GetParent($"{rootPathParent}");
            var rootPath = rootPathFolder.ToString();
            return rootPath;
        }
    }
}
