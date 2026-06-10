using System;
using NUnit.Framework;
using ECommerceApp.Core;

namespace ECommerceApp.Tests.IntegrationTests
{
    [TestFixture]
    public class IntegrationTests
    {
        private PaymentService _paymentService = null!;
        private OrderService _orderService = null!;

        [SetUp]
        public void SetUp()
        {
            _paymentService = new PaymentService();
            _orderService = new OrderService(_paymentService)
            {
                MinimumOrderLimit = 50.00m // Set minimum limit to 50 TL for integration tests
            };
        }

        // Test 16: Order_SuccessfulFlow_ShouldDeductStockAndCompletePayment (PASSED)
        [Test]
        public void Order_SuccessfulFlow_ShouldDeductStockAndCompletePayment()
        {
            var laptop = new Product("P1", "Laptop", 100.00m, 10);
            var cart = new Cart();
            cart.AddItem(laptop, 2); // Subtotal: 200.00 TL

            var card = new CardDetails("1111-2222", 300.00m); // Balance: 300 TL

            var order = _orderService.PlaceOrder(cart, card);

            Assert.Multiple(() =>
            {
                Assert.That(order.Status, Is.EqualTo("Completed"));
                Assert.That(card.Balance, Is.EqualTo(100.00m)); // 300 - 200 = 100
                Assert.That(laptop.Stock, Is.EqualTo(8)); // 10 - 2 = 8
            });
        }

        // Test 17: Order_InsufficientStock_ShouldThrowException (FAILED due to Bug 1)
        [Test]
        public void Order_InsufficientStock_ShouldThrowException()
        {
            var laptop = new Product("P1", "Laptop", 100.00m, 2); // Only 2 in stock
            var cart = new Cart();
            cart.AddItem(laptop, 5); // Customer wants 5 (insufficient stock)

            var card = new CardDetails("1111-2222", 1000.00m);

            // Bug 1: OrderService only checks if stock is exactly 0.
            // Since stock is 2, it lets the order pass instead of throwing an out of stock exception.
            // We expect this to throw an InvalidOperationException, but it won't, causing the test to FAIL.
            Assert.Throws<InvalidOperationException>(() =>
            {
                _orderService.PlaceOrder(cart, card);
            }, "Should not allow placing order when quantity exceeds stock.");
        }

        // Test 18: Order_PaymentExactBalance_ShouldSucceed (FAILED due to Bug 6)
        [Test]
        public void Order_PaymentExactBalance_ShouldSucceed()
        {
            var laptop = new Product("P1", "Laptop", 100.00m, 10);
            var cart = new Cart();
            cart.AddItem(laptop, 1); // Total: 100.00 TL

            // BVA: User card balance is EXACTLY equal to the order amount (100.00 TL)
            var card = new CardDetails("1111-2222", 100.00m);

            // Bug 6: PaymentService checks if (balance <= amount) throwing InsufficientFundsException.
            // This blocks the order when balance is exactly 100.00. We expect it to succeed, so this test will FAIL.
            Assert.DoesNotThrow(() =>
            {
                _orderService.PlaceOrder(cart, card);
            }, "Order should succeed if card balance is exactly equal to the order amount.");
        }

        // Test 19: Order_PaymentInsufficientBalance_ShouldThrowException (PASSED)
        [Test]
        public void Order_PaymentInsufficientBalance_ShouldThrowException()
        {
            var laptop = new Product("P1", "Laptop", 100.00m, 10);
            var cart = new Cart();
            cart.AddItem(laptop, 1); // Total: 100.00 TL

            // EP: User card balance is 99.99 TL, which is less than 100.00 TL
            var card = new CardDetails("1111-2222", 99.99m);

            // Should correctly reject payment due to insufficient funds (99.99 < 100)
            Assert.Throws<InvalidOperationException>(() =>
            {
                _orderService.PlaceOrder(cart, card);
            });
        }

        // Test 20: Order_ExactStockRemaining_ShouldSucceedAndSetStockToZero (PASSED)
        [Test]
        public void Order_ExactStockRemaining_ShouldSucceedAndSetStockToZero()
        {
            var laptop = new Product("P1", "Laptop", 100.00m, 5); // Stock is 5
            var cart = new Cart();
            cart.AddItem(laptop, 5); // BVA: Ordering exactly the remaining stock of 5

            var card = new CardDetails("1111-2222", 1000.00m);

            var order = _orderService.PlaceOrder(cart, card);

            Assert.Multiple(() =>
            {
                Assert.That(order.Status, Is.EqualTo("Completed"));
                Assert.That(laptop.Stock, Is.EqualTo(0)); // Remaining stock should be exactly 0
            });
        }
    }
}
