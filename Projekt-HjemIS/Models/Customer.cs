using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt_HjemIS.Models
{
    public class Customer
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public Location Address { get; set; }
        public List<Message> MsgReceived { get; set; }


        public Customer()
        {

        }
    }
}
