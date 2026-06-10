using System;
using NUnit.Framework;
using ECommerceApp.Core;

namespace ECommerceApp.Tests.BlackBoxTests
{
    [TestFixture]
    public class BlackBoxTests
    {
        // Test 6: Discount_Valid10PercentCoupon_ShouldApplyExactly10Percent (FAILED due to Bug 2)
        [Test]
        public void Discount_Valid10PercentCoupon_ShouldApplyExactly10Percent()
        {
            var cart = new Cart();
            var product = new Product("P1", "Laptop", 100.00m, 10);
            cart.AddItem(product, 1);

            // EP: Applying a valid 10% coupon should deduct 10% (10.00 TL)
            cart.ApplyCoupon("SAVE10");

            // Bug 2: The system hardcodes the discount to 50% for any valid coupon.
            // Expected discount: 10.00m (Total: 90.00m)
            // Actual discount applied: 50.00m (Total: 50.00m)
            Assert.That(cart.GetTotalAmount(), Is.EqualTo(90.00m), "10% coupon should deduct exactly 10%.");
        }

        // Test 7: Discount_Valid50PercentCoupon_ShouldApplyExactly50Percent (PASSED by coincidence)
        [Test]
        public void Discount_Valid50PercentCoupon_ShouldApplyExactly50Percent()
        {
            var cart = new Cart();
            var product = new Product("P1", "Laptop", 100.00m, 10);
            cart.AddItem(product, 1);

            // EP: Applying a valid 50% coupon should deduct 50% (50.00 TL)
            cart.ApplyCoupon("SAVE50");

            // Coincidentally passes because Bug 2 hardcodes all valid coupons to 50%
            Assert.That(cart.GetTotalAmount(), Is.EqualTo(50.00m));
        }

        // Test 8: Discount_InvalidCoupon_ShouldNotApplyDiscount (PASSED)
        [Test]
        public void Discount_InvalidCoupon_ShouldNotApplyDiscount()
        {
            var cart = new Cart();
            var product = new Product("P1", "Laptop", 100.00m, 10);
            cart.AddItem(product, 1);

            // EP: Invalid coupon should not modify the total amount
            cart.ApplyCoupon("INVALID_CODE");

            Assert.Multiple(() =>
            {
                Assert.That(cart.CouponCode, Is.Null);
                Assert.That(cart.DiscountPercentage, Is.EqualTo(0m));
                Assert.That(cart.GetTotalAmount(), Is.EqualTo(100.00m));
            });
        }

        // Test 9: Product_PriceBoundary_MinimumValidPrice_ShouldBeAccepted (PASSED)
        [Test]
        public void Product_PriceBoundary_MinimumValidPrice_ShouldBeAccepted()
        {
            // BVA: The minimum valid price for a product is 0.01 TL
            var product = new Product("P2", "Cheap Item", 0.01m, 10);
            
            Assert.That(product.Price, Is.EqualTo(0.01m));
        }

        // Test 10: Cart_AddItem_QuantityBoundary_ExactlyOneItem_ShouldPass (PASSED)
        [Test]
        public void Cart_AddItem_QuantityBoundary_ExactlyOneItem_ShouldPass()
        {
            var cart = new Cart();
            var product = new Product("P1", "Laptop", 100.00m, 10);

            // BVA: Adding exactly 1 item to the cart is the boundary condition for a valid order quantity
            cart.AddItem(product, 1);

            Assert.Multiple(() =>
            {
                Assert.That(cart.Items, Has.Count.EqualTo(1));
                Assert.That(cart.Items[0].Quantity, Is.EqualTo(1));
            });
        }
    }
}
