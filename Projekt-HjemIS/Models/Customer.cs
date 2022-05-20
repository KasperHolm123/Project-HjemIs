using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt_HjemIS.Models
{
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

        public override string ToString()
        {
            return $"{FirstName} {LastName}, {PhoneNumber}; ";
        }
    }
}
