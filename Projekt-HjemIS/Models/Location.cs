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
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string PostalDistrict { get; set; }
        public string StreetCode { get; set; }
        public string CountyCode { get; set; }
        private string _street;

        public string Street
        {
            get { return _street; }
            set
            {
                _street = CleanInput(value);
            }
        }


        public Location()
        {

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
