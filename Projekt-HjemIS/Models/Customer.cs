using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        

        public Customer()
        {

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
            List<object> internalProperties = GetInternalProperties();
            
            // Sæt alle properties = værdierne af values array'et.
            for (int i = 0; i < values.Length - 1; i++)
            {
                if ((Type)values[i] == typeof(int))
                    internalProperties[2] = values[i];
                else 
                    internalProperties[i] = values[i];
            }
        }

        private List<object> GetInternalProperties()
        {
            List<object> properties = new List<object>();
            properties.Add(FirstName);
            properties.Add(LastName);
            properties.Add(PhoneNumber);
            properties.Add(StreetCode);
            properties.Add(CountyCode);
            return properties;
        }

        public override string ToString()
        {
            return $"{FirstName} {LastName}, {PhoneNumber}; ";
        }
    }
}
