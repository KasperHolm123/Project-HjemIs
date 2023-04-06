using Projekt_HjemIS.Models;
using Projekt_HjemIS.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Projekt_HjemIS.ViewModels
{
    public class EmailViewModel : BaseViewModel
    {

        #region Fields

        private string _searchQuery;
        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                _searchQuery = value;
                FilteredLocations.View.Refresh();
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Location> _locations;
        public ObservableCollection<Location> Locations
        {
            get => _locations;
            set
            {
                _locations = value;
                OnPropertyChanged();
            }
        }

        private CollectionViewSource _filteredLocations;
        public CollectionViewSource FilteredLocations
        {
            get => _filteredLocations;
            set
            {
                _filteredLocations = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Commands

        

        #endregion

        public EmailViewModel()
        {
            Locations = new ObservableCollection<Location>(dh.GetTable<Location>("SELECT * FROM Locations"));

            FilteredLocations = new CollectionViewSource
            {
                Source = Locations
            };
            FilteredLocations.Filter += Search;
        }

        private void Search(object sender, FilterEventArgs e)
        {
            if (string.IsNullOrEmpty(SearchQuery))
            {
                e.Accepted = true;
            }
            else
            {
                e.Accepted = e.Item is Location item && item.Street.ToUpper().Contains(SearchQuery.ToUpper());
            }
        }
    }
}
