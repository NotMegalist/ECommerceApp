using System;
using NUnit.Framework;
using ECommerceApp.Core;

namespace ECommerceApp.Tests.GrayBoxTests
{
    [TestFixture]
    public class GrayBoxTests
    {
        private PaymentService _paymentService = null!;
        private OrderService _orderService = null!;

        [SetUp]
        public void SetUp()
        {
            _paymentService = new PaymentService();
            _orderService = new OrderService(_paymentService)
            {
                MinimumOrderLimit = 100.00m // Set limit to 100 TL
            };
        }

        // Test 11: MinimumOrder_BelowLimit_ShouldThrowException (PASSED)
        [Test]
        public void MinimumOrder_BelowLimit_ShouldThrowException()
        {
            var cart = new Cart();
            // Total order is 99.00 TL, which is below the 100 TL limit
            var product = new Product("P1", "Low Cost Item", 99.00m, 10);
            cart.AddItem(product, 1);
            var card = new CardDetails("1111", 500.00m);

            // BVA: Below minimum limit (99.00m) should throw InvalidOperationException.
            // Since it throws, this test passes.
            Assert.Throws<InvalidOperationException>(() =>
            {
                _orderService.PlaceOrder(cart, card);
            });
        }

        // Test 12: MinimumOrder_AboveLimit_ShouldSucceed (PASSED)
        [Test]
        public void MinimumOrder_AboveLimit_ShouldSucceed()
        {
            var cart = new Cart();
            // Total order is 101.00 TL, which is above the 100 TL limit
            var product = new Product("P1", "Item", 101.00m, 10);
            cart.AddItem(product, 1);
            var card = new CardDetails("1111", 500.00m);

            // EP: Above minimum limit (101.00m) should succeed.
            var order = _orderService.PlaceOrder(cart, card);
            Assert.That(order.Status, Is.EqualTo("Completed"));
        }

        // Test 13: MinimumOrder_ExactlyAtLimit_ShouldSucceed (FAILED due to Bug 3)
        [Test]
        public void MinimumOrder_ExactlyAtLimit_ShouldSucceed()
        {
            var cart = new Cart();
            // Total order is EXACTLY 100.00 TL
            var product = new Product("P1", "Exactly Limit Item", 100.00m, 10);
            cart.AddItem(product, 1);
            var card = new CardDetails("1111", 500.00m);

            // BVA: Exactly at the limit (100.00m) should succeed.
            // Bug 3: The OrderService uses '<=' instead of '<', so it incorrectly throws an exception here.
            // Since we expect it to succeed, this test will FAIL.
            Assert.DoesNotThrow(() =>
            {
                _orderService.PlaceOrder(cart, card);
            }, "An order exactly at the minimum limit should be allowed.");
        }

        // Test 14: CartState_EmptyCartOrder_ShouldThrowException (PASSED)
        [Test]
        public void CartState_EmptyCartOrder_ShouldThrowException()
        {
            var cart = new Cart(); // Empty cart
            var card = new CardDetails("1111", 500.00m);

            // State: Placing an order with zero items in cart is an invalid transition and should throw
            Assert.Throws<InvalidOperationException>(() =>
            {
                _orderService.PlaceOrder(cart, card);
            });
        }

        // Test 15: Order_CheckStatusAfterSuccessfulPlacement_ShouldBeCompleted (PASSED)
        [Test]
        public void Order_CheckStatusAfterSuccessfulPlacement_ShouldBeCompleted()
        {
            var cart = new Cart();
            var product = new Product("P1", "Valid Item", 150.00m, 10);
            cart.AddItem(product, 1);
            var card = new CardDetails("1111", 500.00m);

            // State Transition: After a successful payment, the order status changes to "Completed"
            var order = _orderService.PlaceOrder(cart, card);
            Assert.That(order.Status, Is.EqualTo("Completed"));
        }
    }
}
