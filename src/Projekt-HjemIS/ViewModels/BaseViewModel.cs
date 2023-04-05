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

namespace Projekt_HjemIS.ViewModels
{
    public class BaseViewModel : ObservableObject, INotifyPropertyChanged
    {
        public static readonly DatabaseHandler dh = new DatabaseHandler();
    }
}
