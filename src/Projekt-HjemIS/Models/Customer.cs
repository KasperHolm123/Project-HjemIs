using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Projekt_HjemIS.Models
{
    /// <summary>
    /// Hovedforfatter: Daniel
    /// </summary>
    public class Customer
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int PhoneNumber { get; set; }
        public string StreetCode { get; set; }
        public string CountyCode { get; set; }
        public List<Message> MsgReceived { get; set; }
        public Location Address { get; set; }
        
        public Customer()
        {
            Address = new Location();
            MsgReceived = new List<Message>();
        }
    }
}
