using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt_HjemIS.Models
{

    public class RecordTypeOther
    {
        public string RecordType { get; set; }
        public string CountyCode { get; set; }
        public string StreetCode { get; set; }
        public string HouseNumberFrom { get; set; }
        public string HouseNumberTo { get; set; }
        public string EvenOdd { get; set; }
        public string TimeStamp { get; set; }
    }

    public class AKTVEJ
    {
    }

    public class BOLIG
    {

    }

    public class BYNAVN : RecordTypeOther
    {

    }

    public class POSTDIST : RecordTypeOther
    {

    }

    public class NOTATVEJ
    {

    }

    public class BYFORNYDIST : RecordTypeOther
    {

    }

    public class DIVDIST : RecordTypeOther
    {

    }

    public class EVAKUERDIST : RecordTypeOther
    {

    }

    public class KIRKEDIST : RecordTypeOther
    {

    }

    public class SKOLEDIST : RecordTypeOther
    {

    }

    public class BEFOLKDIST : RecordTypeOther
    {

    }

    public class SOCIALDIST : RecordTypeOther
    {

    }

    public class SOGNEDIST : RecordTypeOther
    {

    }

    public class VALGDIST : RecordTypeOther
    {

    }

    public class VARMEDIST : RecordTypeOther
    {

    }

}
