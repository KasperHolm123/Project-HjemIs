﻿using Projekt_HjemIS.Models;
using Projekt_HjemIS.Systems;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Projekt_HjemIS.Views
{
    /// <summary>
    /// Interaction logic for UsersViews.xaml
    /// </summary>
    public partial class UsersViews : UserControl
    {
        //// Contains all available users.
        public ObservableCollection<User> InternalUsers { get; set; }
        public UsersViews()
        {
            InitializeComponent();
            InternalUsers = new ObservableCollection<User>(DatabaseHandler.GetUsers());
            //bind combobox
            comboUsers.ItemsSource = InternalUsers;
            comboUsers.DisplayMemberPath = $"{nameof(User.userUsername)}";
        }
    }
}
