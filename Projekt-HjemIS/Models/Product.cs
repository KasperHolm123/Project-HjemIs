using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt_HjemIS.Models
{
    /// <summary>
    /// Hovedforfatter: Christian
    /// </summary>
    public class Product
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public int Discount { get; set; }

        public Product() { }

        public Product(int id, string name, int price, int discount)
        {
            ID = id;
            Name = name;
            Price = price;
            Discount = discount;
        }

        public Product(object[] values)
        {
            ID = (int)values[0];
            Name = values[1].ToString();
            Price = (int)values[2];
            Discount = (int)values[3];
        }

        public override string ToString()
        {
            return $"Name: {Name} | Price: {Price} | Discount: {Discount}";
        }
    }
}
