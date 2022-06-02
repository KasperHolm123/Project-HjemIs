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

        public Customer(string firstName, string lastName, int phoneNumber, string streetCode, string countyCode)
        {
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
            StreetCode = streetCode;
            CountyCode = countyCode;
        }

        public Customer(object[] values)
        {
            FirstName = values[0].ToString();
            LastName = values[1].ToString();
            PhoneNumber = (int)values[2];
            StreetCode = values[3].ToString();
            CountyCode = values[4].ToString();
        }

        public override string ToString()
        {
            return $"{FirstName} {LastName}, {PhoneNumber}; ";
        }
    }
}
