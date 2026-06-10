using System;

namespace ECommerceApp.Core
{
    public class Product
    {
        public string Id { get; set; }
        public string Name { get; set; }
        private decimal _price;
        public decimal Price
        {
            get => _price;
            set
            {
                // BUG 4: Price is not validated for negative values, allowing invalid product pricing.
                // Normally, we should check: if (value < 0) throw new ArgumentException("Price cannot be negative.");
                _price = value;
            }
        }

        public int Stock { get; set; }

        public Product(string id, string name, decimal price, int stock)
        {
            Id = id;
            Name = name;
            Price = price; // Sets via property which has Bug 4
            Stock = stock;
        }
    }
}
