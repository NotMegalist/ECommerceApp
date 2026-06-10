using System;
using System.Collections.Generic;
using System.Linq;

namespace ECommerceApp.Core
{
    public class CartItem
    {
        public Product Product { get; }
        public int Quantity { get; set; }

        public CartItem(Product product, int quantity)
        {
            Product = product;
            Quantity = quantity;
        }
    }

    public class Cart
    {
        private readonly List<CartItem> _items = new List<CartItem>();
        public IReadOnlyList<CartItem> Items => _items.AsReadOnly();

        public string? CouponCode { get; private set; }
        public decimal DiscountPercentage { get; private set; } = 0m;

        public void AddItem(Product product, int quantity)
        {
            // BUG 5: We do not validate for negative quantities. We allow negative quantity input.
            // Normally, we should check: if (quantity <= 0) throw new ArgumentException("Quantity must be positive.");
            
            var existingItem = _items.FirstOrDefault(i => i.Product.Id == product.Id);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                _items.Add(new CartItem(product, quantity));
            }
        }

        public void RemoveItem(string productId)
        {
            var item = _items.FirstOrDefault(i => i.Product.Id == productId);
            if (item != null)
            {
                _items.Remove(item);
            }
        }

        public void Clear()
        {
            _items.Clear();
            CouponCode = null;
            DiscountPercentage = 0m;
        }

        public void ApplyCoupon(string couponCode)
        {
            if (couponCode == "SAVE10" || couponCode == "SAVE50")
            {
                CouponCode = couponCode;
                // BUG 2: Coupon is validated, but the discount rate is hardcoded to 50% (0.50) for all valid coupons
                // Normally it should be: DiscountPercentage = couponCode == "SAVE10" ? 0.10m : 0.50m;
                DiscountPercentage = 0.50m;
            }
            else
            {
                CouponCode = null;
                DiscountPercentage = 0m;
            }
        }

        public decimal GetSubTotal()
        {
            return _items.Sum(item => item.Product.Price * item.Quantity);
        }

        public decimal GetTotalAmount()
        {
            decimal subtotal = GetSubTotal();
            decimal discountAmount = subtotal * DiscountPercentage;
            return subtotal - discountAmount;
        }
    }
}
