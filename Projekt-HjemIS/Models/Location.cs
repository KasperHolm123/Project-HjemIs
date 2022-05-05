using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt_HjemIS.Models
{
    public class Location
    {
        public string PostNr { get; set; }
        public string Bynavn { get; set; }
        public string Postdistrikt { get; set; }
        public string Vejkode { get; set; }
        public string Kommunekode { get; set; }
        public string VejNavn { get; set; }
        
        public Location()
        {

        }

        public Location(string postnr, string bynavn, string postdistrikt, string vejkode, string kommunekode)
        {
            PostNr = postnr;
            Bynavn = bynavn;
            Postdistrikt = postdistrikt;
            Vejkode = vejkode;
            Kommunekode = kommunekode;
        }
        public Location(string vejkode, string kommunekode)
        {
            Vejkode = vejkode;
            Kommunekode = kommunekode;
        }
    }
}
