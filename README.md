# ECommerceApp - Yazılım Test ve Kalitesi Final Projesi

Bu proje, **MTH2005 Yazılım Test ve Kalitesi** dersi Final Projesi kapsamında geliştirilmiş bir e-ticaret simülasyonu ve test otomasyonu çalışmasıdır.

---

## 🎯 Proje Amacı ve Kapsamı
Projede, temel bir e-ticaret akışı (ürün seçimi, sepete ekleme, sipariş oluşturma ve ödeme) simüle edilmiştir. Sisteme ek olarak aşağıdaki isterler entegre edilmiştir:
- **Stok Kontrolü** (yetersiz stok durumunda siparişin engellenmesi)
- **İndirim Uygulaması** (yüzdelik kupon indirimlerinin sepet toplamına yansıtılması)
- **Minimum Sipariş Tutarı Kontrolü** (belirlenen limitin altındaki siparişlerin reddedilmesi)

Gereksinimler doğrultusunda, sisteme kasıtlı olarak **6 adet mantıksal hata (bug)** enjekte edilmiş ve C# / NUnit test kütüphanesiyle hazırlanan **20 test senaryosu** yardımıyla bu hataların yakalanması sağlanmıştır.

---

## 🏗️ Proje Dosya Yapısı
Hocanın final ödevinde talep ettiği dosya ağacı birebir uygulanmıştır:

```
ECommerceApp/
 ├── Core/
 │    ├── Product.cs         # Ürün modeli (Kayıt, fiyat ve stok verileri)
 │    ├── Cart.cs            # Sepet işlemleri, ara toplam ve indirim hesaplamaları
 │    └── OrderService.cs    # Sipariş yönetimi, minimum limit kontrolü ve ödeme simülasyonu
 ├── Tests/
 │    ├── UnitTests/         # Unit (White Box) ve Black Box (EP & BVA) Testleri
 │    └── IntegrationTests/  # Integration ve Gray Box Testleri
 ├── Program.cs              # Konsol demo simülasyonu
 └── ECommerceApp.slnx       # .NET 10 XML Solution dosyası
```

---

## 🧪 Test Stratejisi ve Tasarım Teknikleri
Yazılan **exactly 20** adet test senaryosu, test türlerine göre dengeli bir şekilde (her türe 5'er adet) dağıtılmıştır:

1. **UNIT TEST (WHITE BOX):** Kod içindeki kontrol blokları, nesne inşaları ve sepet boşaltma durumları test edilmiştir.
2. **BLACK BOX TEST:** Kodun iç yapısına bakılmaksızın girdilere göre üretilen çıktılar doğrulanmıştır. İndirim kuponları ve geçerli/geçersiz değer aralıkları test edilmiştir.
3. **GRAY BOX TEST:** Sistem eşikleri ve durum geçişleri (örneğin sepet durumu değiştikçe sipariş sürecine geçiş, minimum sipariş limiti sınırları) doğrulanmıştır.
4. **INTEGRATION TEST:** Ürün stoku, sepet, sipariş servisi ve ödeme simülatörünün bir bütün olarak entegrasyonu test edilmiştir.

### 📌 EP & BVA Tekniklerinin Kullanımı
- **Equivalence Partitioning (EP):** Geçerli ve geçersiz kupon kodları (SAVE10, SAVE50, INVALID_CODE), yetersiz kart bakiyesi gibi girdiler eşdeğer sınıflara ayrılarak test edilmiştir.
- **Boundary Value Analysis (BVA):** Minimum ürün fiyatı sınırı (`0.01 TL`), sepete eklenebilecek minimum miktar (`1`), tam minimum sipariş tutarı (`100.00 TL`), tam kalan stok sınırı ve kart bakiyesinin sipariş tutarına tam eşit olması sınırları test edilmiştir.

---

## 🐞 Kasıtlı Bırakılan Hatalar (Bug Listesi)
Yazılan testlerden **14'ü başarılı (Passed)** olurken, enjekte edilen **6 hata nedeniyle tam 6 test başarısız (Failed)** olmaktadır:

| Hata Tipi | Bileşen | Hata Açıklaması | Başarısız Olan Test |
| :--- | :--- | :--- | :--- |
| **Bug 1 (Stok)** | `OrderService` | Stok 0 değilse (örneğin 2) ama sipariş miktarı stoktan fazlaysa (örneğin 5) siparişe hatalı izin verir. | `Order_InsufficientStock_ShouldThrowException` |
| **Bug 2 (İndirim)** | `Cart` | `SAVE10` (%10) kuponu girilse dahi sisteme sabit `%50` indirim uygulanır. | `Discount_Valid10PercentCoupon_ShouldApplyExactly10Percent` |
| **Bug 3 (Minimum Tutar)** | `OrderService` | Sipariş tutarı tam sınır limitine eşitse (`total == limit`), `<=` karşılaştırma hatasından ötürü siparişi engeller. | `MinimumOrder_ExactlyAtLimit_ShouldSucceed` |
| **Bug 4 (Fiyat)** | `Product` | Ürün fiyatının negatif (örneğin `-5.00 TL`) tanımlanmasına izin verir, hata fırlatmaz. | `Product_NegativePrice_ShouldThrowArgumentException` |
| **Bug 5 (Miktar)** | `Cart` | Sepete eklenecek ürün adedinin negatif olmasına (`quantity < 0`) izin verir, sepet toplamını düşürür. | `Cart_AddItem_NegativeQuantity_ShouldThrowArgumentException` |
| **Bug 6 (Ödeme)** | `PaymentService` | Kullanıcı bakiyesi sipariş tutarına kuruşu kuruşuna eşitse, `<=` mantıksal hatasından dolayı ödemeyi reddeder. | `Order_PaymentExactBalance_ShouldSucceed` |

---

## 🚀 Projeyi Çalıştırma

### 1. Konsol Uygulamasını Çalıştırma
Simülasyon akışını ve bug gösterimlerini konsol ekranında görmek için projenin ana dizininde:
```bash
dotnet run --project ECommerceApp/ECommerceApp.csproj
```

### 2. Testleri Çalıştırma
NUnit test suitini başlatmak ve **14 Passed / 6 Failed** sonuçlarını doğrulamak için:
```bash
dotnet test
```
