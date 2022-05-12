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
        public string MessageBody { get; set; }
        public string Subject { get; set; }
        public List<Product> Offers { get; set; }

        public Message(string subject, string body, List<Location> recipients, List<Product> offers = null)
        {
            Subject = subject;
            MessageBody = body;
            RecipientsLocations = recipients;
            Offers = offers;
        }
    }
}
