﻿using System;
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
    public class Message : INotifyPropertyChanged
    {
        private DateTime _date;
        public event PropertyChangedEventHandler PropertyChanged;
        public List<Location> RecipientsLocations { get; set; }
        public int ID { get; set; }
        public string Body { get; set; }
        public string Type { get; set; }
        public List<Product> Offers { get; set; }
        public DateTime Date
        {
            get { return _date; }
            set
            {
                _date = value;
                OnPropertyChanged("Date");
            }
        }
        public List<Customer> Recipients { get; set; }
        public Message()
        {
            Recipients = new List<Customer>();
        }
        public Message(string body, List<Location> recipients, string type, List<Product> offers = null)
        {
            Body = body;
            Recipients = new List<Customer>();
            Type = type;
        }


        public void OnPropertyChanged(string prop)
        {
            if(PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs(prop)); 
        }

        public override bool Equals(Object obj)
        {
            //Check for null and compare run-time types.
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                Message msg = obj as Message;
                return msg.ID == this.ID;
            }
        }

    }
}
