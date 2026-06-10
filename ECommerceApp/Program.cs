using System;
using ECommerceApp.Core;

namespace ECommerceApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=================================================================");
            Console.WriteLine("        FINAL PROJESI - YAZILIM TEST VE KALITESI (MTH2005)      ");
            Console.WriteLine("                 E-Commerce App & Bug Showcase                  ");
            Console.WriteLine("=================================================================");
            Console.WriteLine();
            Console.WriteLine("Sistem içinde testler tarafından tespit edilmek üzere bırakılmış");
            Console.WriteLine("6 adet kasıtlı hata (bug) bulunmaktadır:");
            Console.WriteLine();
            Console.WriteLine("1. [STOCK BUG]: Stok miktarı 0 değilse (örneğin 2) ama sipariş miktarı");
            Console.WriteLine("   stoktan fazlaysa (örneğin 5) siparişe izin veriyor.");
            Console.WriteLine("2. [DISCOUNT BUG]: SAVE10 (%10) kuponu uygulansa bile arka planda sabit");
            Console.WriteLine("   %50 indirim uygulanıyor.");
            Console.WriteLine("3. [MIN ORDER BUG]: Sipariş tutarı minimum limit değerine tam eşitse");
            Console.WriteLine("   (örneğin 100 TL) siparişi hata vererek engelliyor (<= yerine < olmalı).");
            Console.WriteLine("4. [PRICE BUG]: Ürün fiyatının negatif (-10 TL gibi) belirlenmesine izin veriliyor.");
            Console.WriteLine("5. [QTY BUG]: Sepete negatif adet (-2 gibi) ürün eklenerek sepet toplamı düşürülebiliyor.");
            Console.WriteLine("6. [PAYMENT BUG]: Kart bakiyesi sipariş tutarına tam eşitse ödemeyi reddediyor.");
            Console.WriteLine();
            Console.WriteLine("-----------------------------------------------------------------");
            Console.WriteLine("                   ÖRNEK SIMÜLASYON BAŞLATILIYOR                  ");
            Console.WriteLine("-----------------------------------------------------------------");

            try
            {
                // 1. Ürünler ve Servisler Tanımlanıyor
                var paymentService = new PaymentService();
                var orderService = new OrderService(paymentService) { MinimumOrderLimit = 100m };

                var laptop = new Product("P1", "Gaming Laptop", 150.00m, 5); // Fiyat: 150 TL, Stok: 5
                var mouse = new Product("P2", "Kablosuz Mouse", 40.00m, 10);  // Fiyat: 40 TL, Stok: 10

                Console.WriteLine($"Ürün 1: {laptop.Name} | Fiyat: {laptop.Price} TL | Stok: {laptop.Stock}");
                Console.WriteLine($"Ürün 2: {mouse.Name} | Fiyat: {mouse.Price} TL | Stok: {mouse.Stock}");

                // 2. Sepete ekleme yapılıyor
                var cart = new Cart();
                cart.AddItem(laptop, 1);
                cart.AddItem(mouse, 1);
                Console.WriteLine($"Sepete {laptop.Name} (1 adet) ve {mouse.Name} (1 adet) eklendi.");
                Console.WriteLine($"Ara Toplam: {cart.GetSubTotal()} TL");

                // 3. Kupon Uygulanıyor
                Console.WriteLine("SAVE10 kuponu uygulanıyor...");
                cart.ApplyCoupon("SAVE10");
                Console.WriteLine($"Uygulanan Kupon: {cart.CouponCode}");
                Console.WriteLine($"İndirim Oranı: %{cart.DiscountPercentage * 100}");
                Console.WriteLine($"İndirimli Toplam: {cart.GetTotalAmount()} TL (Hata: %10 yerine %50 düştü)");

                // 4. Ödeme ve Sipariş
                var userCard = new CardDetails("1234-5678-9012-3456", 200m);
                Console.WriteLine($"Müşteri Kart Bakiyesi: {userCard.Balance} TL");

                Console.WriteLine("Sipariş veriliyor...");
                var order = orderService.PlaceOrder(cart, userCard);

                Console.WriteLine("Sipariş başarıyla tamamlandı!");
                Console.WriteLine($"Sipariş ID: {order.OrderId}");
                Console.WriteLine($"Kalan Kart Bakiyesi: {userCard.Balance} TL");
                Console.WriteLine($"Yeni Laptop Stoku: {laptop.Stock} (Kalan)");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"HATA OLUŞTU: {ex.Message}");
            }

            Console.WriteLine();
            Console.WriteLine("=================================================================");
            Console.WriteLine("NUnit Test projesini çalıştırmak için:");
            Console.WriteLine("  dotnet test");
            Console.WriteLine("=================================================================");
        }
    }
}
