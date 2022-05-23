using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt_HjemIS.Models
{
    public class Location
    {
        public string StreetCode { get; set; }
        public string CountyCode { get; set; }
        public string Street { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string PostalDistrict { get; set; }
        public bool IsRecipient { get; set; }

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

        public Location(object[] values)
        {
            StreetCode = values[0].ToString();
            CountyCode = values[1].ToString();
            Street = values[2].ToString();
            PostalCode = values[3].ToString();
            City = values[4].ToString();
            PostalDistrict = values[5].ToString();
        }

    }
}
