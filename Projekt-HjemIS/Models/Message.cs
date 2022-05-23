using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt_HjemIS.Models
{
    public class Message
    {
        public List<Location> RecipientsLocations { get; set; }

        public int ID { get; set; }
        public string Type { get; set; }
        public string MessageBody { get; set; }
        public List<Product> Offers { get; set; }

        public Message(string body, List<Location> recipients, string type, List<Product> offers = null)
        {
            MessageBody = body;
            RecipientsLocations = recipients;
            Type = type;
            Offers = offers;
        }
    }
}
