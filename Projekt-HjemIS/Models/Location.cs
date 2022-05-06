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

        public Location(string postnr, string bynavn, string postdistrikt, string vejkode, string kommunekode)
        {
            PostalCode = postnr;
            City = bynavn;
            PostalDistrict = postdistrikt;
            StreetCode = vejkode;
            CountyCode = kommunekode;
        }
        public Location(string vejkode, string kommunekode)
        {
            StreetCode = vejkode;
            CountyCode = kommunekode;
        }
    }
}
