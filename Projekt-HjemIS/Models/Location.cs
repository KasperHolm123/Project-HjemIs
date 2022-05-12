using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt_HjemIS.Models
{
    public class Location
    {
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string PostalDistrict { get; set; }
        public string StreetCode { get; set; }
        public string CountyCode { get; set; }
        public string Street { get; set; }
        
        public Location()
        {

        }

        public Location(string streetCode, string countyCode, string street, string postalCode, string city, string postalDistrict)
        {
            Street = streetCode;
            CountyCode = countyCode;
            Street = street;
            PostalCode = postalCode;
            City = city;
            PostalDistrict = postalDistrict;
        }

    }
}
