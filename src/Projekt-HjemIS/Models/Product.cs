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
        public decimal Discount { get; set; }
        private decimal _discountedPrice;
        public decimal DiscountedPrice
        {
            get => _discountedPrice;
            set => _discountedPrice = value;
        }
    }
}
