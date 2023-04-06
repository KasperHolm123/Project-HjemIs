using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt_HjemIS.Models
{
    /// <summary>
    /// Hovedforfatter: Kasper
    /// </summary>
    public class Message
    {
        public int ID { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
