using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt_HjemIS.Models
{
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool Admin { get; set; }
    }
}
