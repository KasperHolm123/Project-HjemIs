using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt_HjemIS.Models
{
    /// <summary>
    /// Hovedforfatter: Kasper
    /// </summary>
    public class Message_Mail : Message
    {
        public string Subject { get; set; }

        public Message_Mail(string subject, string body, List<Location> recipients, string type, List<Product> offers = null)
        {
            Subject = subject;
        }
    }
}
