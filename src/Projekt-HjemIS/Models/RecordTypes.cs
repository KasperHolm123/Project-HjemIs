using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt_HjemIS.Models
{

    public class Record
    {

    }
    
    public class RecordTypeLocation : Record
    {
        public int ID { get; set; }
        public string StreetName { get; set; }
	    public string CountyCode { get; set; }
	    public string StreetCode { get; set; }
	    public string HouseNumberFrom { get; set; }
	    public string HouseNumberTo { get; set; }
	    public string EvenOdd { get; set; }
	    public string PostalCode { get; set; }
        public string CityName { get; set; }
    }

    public class RecordTypeOther : Record
    {
        public string CountyCode { get; set; }
        public string StreetCode { get; set; }
        public string HouseNumberFrom { get; set; }
        public string HouseNumberTo { get; set; }
        public string EvenOdd { get; set; }
        public string PostalCode { get; set; }

        public RecordTypeOther(string[] record)
        {
            CountyCode = record[1];
            StreetCode = record[2];
            HouseNumberFrom = record[3];
            HouseNumberTo = record[4];
            EvenOdd = record[5];
            if (record[0] == "004")
            {
                PostalCode = record[7];
            }
            else
            {
                PostalCode = "";
            }
        }
    }

    public class RecordTypeAKTVEJ : Record
    {
        public string CountyCode { get; set; }
        public string StreetCode { get; set; }
        public string ToCountyCode { get; set; }
        public string ToStreetCode { get; set; }
        public string FromCountyCode { get; set; }
        public string FromStreetCode { get; set; }
        public string ThereStart { get; set; }
        public string StreetAddrName { get; set; }
        public string StreetName { get; set; }

        public RecordTypeAKTVEJ(string[] record)
        {
            CountyCode = record[1];
            StreetCode = record[2];
            ToCountyCode = record[4];
            ToStreetCode = record[5];
            FromCountyCode = record[6];
            FromStreetCode = record[7];
            ThereStart = record[8];
            StreetAddrName = record[9];
            StreetName = record[10];
        }
    }

    public class RecordTypeBOLIG : Record
    {
        public string CountyCode { get; set; }
        public string StreetCode { get; set; }
        public string HouseNumber { get; set; }
        public string Floor { get; set; }
        public string NextDoorNumber { get; set; }
        public string Timestamp { get; set; }
        public string Filler1a { get; set; }
        public string ThereStart { get; set; }
        public string Filler12N { get; set; }
        public string Locality { get; set; }
    }

    public class RecordTypeNOTATVEJ : Record
    {
        public string CountyCode { get; set; }
        public string StreetCode { get; set; }
        public string NoteNumber { get; set; }
        public string NoteLine { get; set; }
        public string Timestamp { get; set; }
        public string ThereStart { get; set; }
    }
}
