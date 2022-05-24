using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        private string _street;
        public string Street
        {
            get { return _street; }
            set
            {
                _street = CleanInput(value);
            }
        }
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
        public override string ToString()
        {
            if (Street != null) return City + "-" + Street;
            return City + "-" + PostalCode;
        }
        static string CleanInput(string strIn)
        {
            // Replace invalid characters with empty strings.
            try
            {
                return Regex.Replace(strIn, @"[^\w\.@-]", "",
                                     RegexOptions.None, TimeSpan.FromSeconds(1.5));
            }
            // If we timeout when replacing invalid characters,
            // we should return Empty.
            catch (RegexMatchTimeoutException)
            {
                return String.Empty;
            }
        }
    }
}
