using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt_HjemIS
{
    public class User
    {
        public string fPassword { get; set; } //"fPassword" er forgot password, og bliver brugt i classen ForgotPass.cs til at store en bestemt usernames password. 
        public static int Admin { get; set; } //skal implementeres
        public static string Username { set; get; }
    }
}
