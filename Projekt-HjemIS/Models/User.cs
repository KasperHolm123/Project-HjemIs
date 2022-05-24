using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt_HjemIS.Models
{
    public class User
    {
        public User(string username)
        {
            userUsername = username;
        }

        public string fPassword { get; set; } //"fPassword" er forgot password, og bliver brugt i classen ForgotPass.cs til at store en bestemt usernames password. 
        public static bool Admin { get; set; } //skal implementeres.
        public static string Username { set; get; } //bliver brugt til at se, hvilken user der er logget ind på applikationen. 
        public string userUsername { get; set; } //bliver brugt i UsersView til at importere alle unikke Usernames.
        public bool adminAdmin
        {
            get
            {
                return Admin;
            }
            set
            {
                Admin = value;
            }
        }

        public User(object[] values)
        {

        }

    }
}
