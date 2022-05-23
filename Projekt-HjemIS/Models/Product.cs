using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt_HjemIS.Models
{
    public class Product
    {
        public int Price { get; set; }
        public int DiscountedPrice { get; set; }
        public int Discount { get; set; }
        public string Name { get; set; }
        
        public Product(string name, int price, int discount, int discountedPrice)
        {
            Name = name;
            Price = price;
            Discount = discount;
            DiscountedPrice = discountedPrice;
        }

        public Product(object[] values)
        {

        }

        public override string ToString()
        {
            return $"Name: {Name} | Price: {Price} | Discount: {Discount} | Discounted Price: {DiscountedPrice} ";
        }
    }
}
