using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt_HjemIS.Models
{
    public class Message_Mail : Message
    {
        public string Subject { get; set; }

        public Message_Mail(string subject, string body, List<Location> recipients, List<Product> offers = null)
            : base(body, recipients, offers)
        {
            Subject = subject;
        }
    }
}
