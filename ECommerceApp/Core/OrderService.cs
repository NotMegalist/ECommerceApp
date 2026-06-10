using System;

namespace ECommerceApp.Core
{
    public class Order
    {
        public string OrderId { get; }
        public Cart Cart { get; }
        public decimal TotalAmount { get; }
        public string Status { get; set; } = "Pending";

        public Order(string orderId, Cart cart, decimal totalAmount)
        {
            OrderId = orderId;
            Cart = cart;
            TotalAmount = totalAmount;
        }
    }

    public class CardDetails
    {
        public string CardNumber { get; set; }
        public decimal Balance { get; set; }

        public CardDetails(string cardNumber, decimal balance)
        {
            CardNumber = cardNumber;
            Balance = balance;
        }
    }

    public class PaymentService
    {
        public void ProcessPayment(CardDetails card, decimal amount)
        {
            if (amount < 0)
            {
                throw new ArgumentException("Payment amount cannot be negative.");
            }

            // BUG 6: Boundary check bug. Fails when balance is exactly equal to the amount due to '<=' operator.
            // Normally: if (card.Balance < amount)
            if (card.Balance <= amount)
            {
                throw new InvalidOperationException("Insufficient funds.");
            }

            card.Balance -= amount;
        }
    }

    public class OrderService
    {
        private readonly PaymentService _paymentService;
        public decimal MinimumOrderLimit { get; set; } = 100.00m; // Default minimum limit

        public OrderService(PaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        public Order PlaceOrder(Cart cart, CardDetails card)
        {
            if (cart.Items.Count == 0)
            {
                throw new InvalidOperationException("Cannot place order with an empty cart.");
            }

            // 1. Stock Control
            foreach (var item in cart.Items)
            {
                // BUG 1: Only checks if stock is exactly 0. Does not check if quantity is greater than remaining stock.
                // Normally: if (item.Product.Stock < item.Quantity)
                if (item.Product.Stock == 0)
                {
                    throw new InvalidOperationException($"Product {item.Product.Name} is out of stock.");
                }
            }

            // 2. Minimum Order Tutarı Control
            decimal totalAmount = cart.GetTotalAmount();
            
            // BUG 3: Boundary limit check bug. Blocks the order if total amount is exactly equal to the limit.
            // Normally: if (totalAmount < MinimumOrderLimit)
            if (totalAmount <= MinimumOrderLimit)
            {
                throw new InvalidOperationException($"Order total must be greater than minimum order limit of {MinimumOrderLimit}.");
            }

            // 3. Payment Processing
            _paymentService.ProcessPayment(card, totalAmount);

            // 4. Stock Deduction
            foreach (var item in cart.Items)
            {
                item.Product.Stock -= item.Quantity;
            }

            var order = new Order(Guid.NewGuid().ToString(), cart, totalAmount)
            {
                Status = "Completed"
            };

            return order;
        }
    }
}
