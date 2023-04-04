using Projekt_HjemIS.Models;
using Projekt_HjemIS.Systems.Utility.Database_handling;
using Projekt_HjemIS.Views;
using Projekt_HjemIS.Views.Login;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Input;


namespace Projekt_HjemIS
{
    /// <summary>
    /// Hovedforfatter: Christian
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        /*
        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            CreateUserView create = new CreateUserView();
            create.Show();
            this.Close();
        }

        private void btnForgot_Click(object sender, RoutedEventArgs e)
        {
            ForgotPasswordView forgotPass = new ForgotPasswordView();
            forgotPass.Show();
            this.Close();
        }

        private void password_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                btnLogin_Click(sender, e);
            }
        }
        */
    }
}
