using System;
using NUnit.Framework;
using ECommerceApp.Core;

namespace ECommerceApp.Tests.UnitTests
{
    [TestFixture]
    public class UnitTests
    {
        // Test 1: Product_Constructor_ValidParameters_ShouldSetProperties (PASSED)
        [Test]
        public void Product_Constructor_ValidParameters_ShouldSetProperties()
        {
            var product = new Product("P1", "Test Product", 10.50m, 10);
            
            Assert.Multiple(() =>
            {
                Assert.That(product.Id, Is.EqualTo("P1"));
                Assert.That(product.Name, Is.EqualTo("Test Product"));
                Assert.That(product.Price, Is.EqualTo(10.50m));
                Assert.That(product.Stock, Is.EqualTo(10));
            });
        }

        // Test 2: Product_NegativePrice_ShouldThrowArgumentException (FAILED due to Bug 4)
        [Test]
        public void Product_NegativePrice_ShouldThrowArgumentException()
        {
            // Bug 4: Negative price validation is omitted in Product class.
            // We expect this to throw an ArgumentException, but it won't, causing this test to FAIL.
            Assert.Throws<ArgumentException>(() =>
            {
                var product = new Product("P2", "Buggy Product", -5.00m, 10);
            }, "Negative price should not be allowed.");
        }

        // Test 3: Cart_AddItem_ValidQuantity_ShouldIncreaseCartTotal (PASSED)
        [Test]
        public void Cart_AddItem_ValidQuantity_ShouldIncreaseCartTotal()
        {
            var cart = new Cart();
            var product = new Product("P1", "Test Product", 50.00m, 10);

            cart.AddItem(product, 2);

            Assert.Multiple(() =>
            {
                Assert.That(cart.Items, Has.Count.EqualTo(1));
                Assert.That(cart.Items[0].Quantity, Is.EqualTo(2));
                Assert.That(cart.GetSubTotal(), Is.EqualTo(100.00m));
            });
        }

        // Test 4: Cart_AddItem_NegativeQuantity_ShouldThrowArgumentException (FAILED due to Bug 5)
        [Test]
        public void Cart_AddItem_NegativeQuantity_ShouldThrowArgumentException()
        {
            var cart = new Cart();
            var product = new Product("P1", "Test Product", 50.00m, 10);

            // Bug 5: Cart allows adding negative quantity (which decreases total amount), so it won't throw.
            // We expect it to throw an ArgumentException, but it won't, causing this test to FAIL.
            Assert.Throws<ArgumentException>(() =>
            {
                cart.AddItem(product, -2);
            }, "Negative quantity should throw ArgumentException.");
        }

        // Test 5: Cart_Clear_ShouldResetCartToEmpty (PASSED)
        [Test]
        public void Cart_Clear_ShouldResetCartToEmpty()
        {
            var cart = new Cart();
            var product = new Product("P1", "Test Product", 50.00m, 10);
            cart.AddItem(product, 2);
            cart.ApplyCoupon("SAVE10");

            cart.Clear();

            Assert.Multiple(() =>
            {
                Assert.That(cart.Items, Is.Empty);
                Assert.That(cart.CouponCode, Is.Null);
                Assert.That(cart.DiscountPercentage, Is.EqualTo(0m));
            });
        }
    }
}
