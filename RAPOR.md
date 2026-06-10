# YAZILIM TEST VE KALİTESİ FİNAL PROJE RAPORU

**Ders:** MTH2005 - Yazılım Test ve Kalitesi (1. Section)  
**Öğrenci:** Ahmet Sarpkaya (NotMegalist)  
**Tarih:** 10 Haziran 2026  

---

## 📊 Test Summary

| Durum | Adet |
| :--- | :--- |
| **Total Tests** | 20 |
| **Passed Tests** | 14 |
| **Failed Tests** | 6 |

---

## ❌ Failed Tests & Nedenleri

Sistem içerisine kasıtlı olarak bırakılan 6 mantıksal hata (bug) nedeniyle aşağıdaki 6 test başarısız (Failed) olmuştur:

1. **`Product_NegativePrice_ShouldThrowArgumentException`**
   - *Neden:* `Product` sınıfının constructor ve property setter bloklarında fiyatın negatif (`< 0`) olmasını engelleyen bir validasyon bulunmamaktadır. Bu nedenle negatif fiyatlı ürün oluşturulabilmiş ve fırlatılması beklenen `ArgumentException` fırlatılmamıştır.
2. **`Cart_AddItem_NegativeQuantity_ShouldThrowArgumentException`**
   - *Neden:* `Cart.AddItem` metodunda sepete eklenecek ürün adedinin pozitif olması gerektiği doğrulanmamıştır. Negatif adet (-2) sepete eklenebilmiş ve sepet tutarı düşmüştür. Beklenen `ArgumentException` fırlatılmadığı için test başarısız olmuştur.
3. **`Discount_Valid10PercentCoupon_ShouldApplyExactly10Percent`**
   - *Neden:* E-ticaret sepetinde indirim kuponu uygulayan mantıkta, kupon doğruluğu kontrol edilse de indirim oranı tüm geçerli kuponlar için sabit `%50` olarak hardcoded yazılmıştır. Test, `%10` indirim beklerken sistem `%50` indirim uyguladığı için test kalmıştır.
4. **`MinimumOrder_ExactlyAtLimit_ShouldSucceed`**
   - *Neden:* `OrderService.PlaceOrder` metodunda minimum sipariş kontrolü `if (totalAmount <= MinimumOrderLimit)` şeklinde yazılmıştır. Sınır karşılaştırmasında `<` yerine `<=` kullanıldığı için sipariş tutarı tam minimum limite eşit olduğunda (100 TL) siparişe izin verilmesi gerekirken hatalı bir şekilde hata fırlatılmıştır.
5. **`Order_InsufficientStock_ShouldThrowException`**
   - *Neden:* `OrderService` içindeki stok kontrolü yalnızca ürün stoku tam `0` ise siparişi engellemektedir (`if (product.Stock == 0)`). Ürünün stoku 0'dan büyük ancak sipariş miktarından küçük olduğunda (örneğin stok: 2, talep: 5) kontrol atlanmakta ve siparişe izin verilmektedir.
6. **`Order_PaymentExactBalance_ShouldSucceed`**
   - *Neden:* `PaymentService` sınıfında bakiye kontrolü yapılırken `if (card.Balance <= amount)` operatörü kullanılmıştır. Kart bakiyesi çekilecek tutara kuruşu kuruşuna tam eşit olduğunda `<=` kontrolü `true` dönmekte ve sistem yetersiz bakiye hatası fırlatmaktadır.

---

## 🐞 Tespit Edilen Bug Listesi (Bug Registry)

| Bug ID | Bileşen | Hata Açıklaması | Etkisi |
| :--- | :--- | :--- | :--- |
| **BUG-01** | `OrderService.cs` | Yetersiz stok durumunda sipariş geçilebiliyor. | Stok değerlerinin eksiye düşmesine ve teslim edilemeyecek siparişlerin oluşmasına sebep olur. |
| **BUG-02** | `Cart.cs` | Geçerli her kuponda sabit %50 indirim uygulanıyor. | İşletme için ciddi finansal kayıplara yol açar. |
| **BUG-03** | `OrderService.cs` | Tam sınır değerdeki (100 TL) siparişler engelleniyor. | Müşteri memnuniyetsizliğine ve sepet terkine neden olur. |
| **BUG-04** | `Product.cs` | Negatif fiyat girilmesine izin veriliyor. | Sepet tutarının sıfır veya negatif olmasına imkan tanır. |
| **BUG-05** | `Cart.cs` | Sepete eksi miktarda ürün eklenebiliyor. | Müşterilerin sepet fiyatını manipüle etmesine olanak tanır. |
| **BUG-06** | `PaymentService.cs` | Tam bakiye ile ödeme yapılması engelleniyor. | Tam bakiyesi olan müşterilerin alışveriş yapamamasına sebep olur. |

---

## ⚠️ Hata Kavramları Açıklamaları (Yazılım Test Terminolojisi)

Yazılım kalitesinde sıklıkla karıştırılan dört temel kavram ve bu projeden somut örnekleri aşağıdadır:

### 1. Error (Hata / İnsan Hatası)
Geliştiricinin veya tasarımcının analiz, tasarım ya da kodlama sırasında yaptığı bilişsel hatalardır.
- *Projeden Örnek:* Geliştiricinin bakiye kontrolü kodlarken, zihninde "bakiye tutardan küçük olmalı" mantığı yerine yanlışlıkla `<=` yazması bir **Error**'dır.

### 2. Fault / Defect / Bug (Kusur / Hata Kodu)
Yazılımın kaynak koduna, dokümanına veya tasarımına yansıyan, insan hatasının ürünü olan fiziksel kusurdur. Kodun içine yerleşmiş yanlış karşılaştırma ifadeleridir.
- *Projeden Örnek:* `PaymentService.cs` dosyasının 30. satırında yer alan `if (card.Balance <= amount)` kod satırı bir **Fault / Bug**'dır.

### 3. Failure (Başarısızlık / Çalışma Zamanı Hatası)
Yazılım çalışırken bir kusurun (fault) tetiklenmesi sonucu sistemin beklenen davranıştan sapması ve dış dünyaya hatalı çıktı vermesidir.
- *Projeden Örnek:* Kartında tam 100 TL olan ve 100 TL'lik alışveriş yapan bir müşterinin ekranında ödeme onaylanması gerekirken "Yetersiz Bakiye" uyarısının çıkması sistemin çalışma zamanındaki **Failure** durumudur.

---

## 📈 Test Stratejileri Açıklamaları

### 1. Agile Testing (Çevik Test)
Yazılım geliştirme süreciyle (yazılım kodlamasıyla) test süreçlerinin paralel ve iş birliği içinde yürütüldüğü, test uzmanlarının sürecin başından itibaren yer aldığı dinamik bir stratejidir. Geri bildirim döngüleri çok kısadır ve gereksinim değişikliklerine hızlı uyum sağlanır.

### 2. Risk-Based Testing (Risk Tabanlı Test)
Yazılımın en kritik, en sık kullanılan veya hata olasılığı en yüksek olan kısımlarına öncelik verilerek test kaynaklarının (zaman, insan gücü) yönetilmesidir. 
- *Örneğin bu projede:* E-ticaret sisteminde ödeme ve stok kontrolü bileşenleri yüksek riskli alanlar olduğu için entegrasyon testlerinde bu kısımlar yoğun olarak test edilmiştir.

### 3. Regression Testing (Regresyon Testi)
Sistemde yapılan kod değişikliklerinin (yeni özellik eklenmesi, bug fix vb.) mevcut ve çalışan diğer özelliklere zarar vermediğini doğrulamak amacıyla mevcut test setinin yeniden çalıştırılmasıdır. Bizim senaryomuzda, hataları giderdikten sonra 20 testin tamamının hala doğru çalışıp çalışmadığını kontrol etmek regresyon testidir.

---

## 🔁 STLC (Yazılım Test Yaşam Döngüsü) Aşamaları

Projede uygulanan STLC süreci aşağıdaki aşamalardan oluşmuştur:

### 1. Requirement Analysis (Gereksinim Analizi)
Ödev senaryosundaki e-ticaret akışları ve yeni eklenecek özellikler (Stok kontrolü, discount kuponu, minimum sipariş limiti) analiz edilmiştir. Test edilebilecek iş kuralları listelenmiştir.

### 2. Test Planning (Test Planlaması)
Hangi test türlerinin uygulanacağı planlanmıştır. NUnit kütüphanesinin ve C# dilinin kullanılacağı belirlenmiş, testlerin Unit, Integration, Black Box ve Gray Box türleri arasında tam 5'er adet olacak şekilde eşit dağıtılması kararlaştırılmıştır.

### 3. Test Design (Test Tasarımı)
Test case'ler tasarlanırken **Equivalence Partitioning (Eşdeğerlik Bölümleme)** ve **Boundary Value Analysis (Sınır Değer Analizi)** yöntemleri kullanılmıştır. 
- *EP Örneği:* Kupon kodlarında geçerli sınıflar (SAVE10, SAVE50) ve geçersiz sınıflar (INVALID_CODE) şeklinde girdiler gruplanmıştır.
- *BVA Örneği:* Minimum sipariş limitinin 100 TL olması durumunda; 99 TL (limit - 1), 100 TL (limit) ve 101 TL (limit + 1) test girdisi olarak seçilmiştir.

### 4. Test Execution (Test Koşumu)
Test kodları NUnit test projesinde kodlanmış ve terminal üzerinde `dotnet test` komutu yardımıyla çalıştırılmıştır. Koşum sonrasında test çıktısı analiz edilmiş ve beklenen 6 hatanın tamamı ilgili testler tarafından yakalanmıştır.

### 5. Test Result & Reporting (Test Sonuçları & Raporlama)
Test koşum sonuçları, geçen/kalan test sayıları, enjekte edilen bug'ların etkileri ve test teorisi kavramları bu rapor (`RAPOR.md`) dosyası içerisinde belgelenmiştir.

---

## 📝 Yazılan Test Senaryolarının Detaylı Listesi

### 1. Unit Tests (White Box) - 5 Test
- **Test 1:** `Product_Constructor_ValidParameters_ShouldSetProperties` (**PASSED**)  
  *Girdi:* Fiyat=10.50, Stok=10. *Beklenen:* Nesne özelliklerinin doğru set edilmesi.
- **Test 2:** `Product_NegativePrice_ShouldThrowArgumentException` (**FAILED - BUG-04**)  
  *Girdi:* Fiyat=-5.00. *Beklenen:* ArgumentException fırlatılması.
- **Test 3:** `Cart_AddItem_ValidQuantity_ShouldIncreaseCartTotal` (**PASSED**)  
  *Girdi:* Laptop (Fiyat: 50.00 TL), Miktar=2. *Beklenen:* Sepet alt toplamının 100.00 TL olması.
- **Test 4:** `Cart_AddItem_NegativeQuantity_ShouldThrowArgumentException` (**FAILED - BUG-05**)  
  *Girdi:* Miktar=-2. *Beklenen:* ArgumentException fırlatılması.
- **Test 5:** `Cart_Clear_ShouldResetCartToEmpty` (**PASSED**)  
  *Girdi:* Sepet temizleme tetiklemesi. *Beklenen:* Sepet listesinin ve kupon oranının sıfırlanması.

### 2. Black Box Tests - 5 Test
- **Test 6:** `Discount_Valid10PercentCoupon_ShouldApplyExactly10Percent` (**FAILED - BUG-02**)  
  *Girdi:* Kupon="SAVE10", Sepet=100.00 TL. *Beklenen:* Toplamın 90.00 TL olması.
- **Test 7:** `Discount_Valid50PercentCoupon_ShouldApplyExactly50Percent` (**PASSED**)  
  *Girdi:* Kupon="SAVE50", Sepet=100.00 TL. *Beklenen:* Toplamın 50.00 TL olması.
- **Test 8:** `Discount_InvalidCoupon_ShouldNotApplyDiscount` (**PASSED**)  
  *Girdi:* Kupon="INVALID_CODE", Sepet=100.00 TL. *Beklenen:* Toplamın 100.00 TL kalması.
- **Test 9:** `Product_PriceBoundary_MinimumValidPrice_ShouldBeAccepted` (**PASSED**)  
  *Girdi:* Fiyat=0.01 (BVA). *Beklenen:* Sınır değerin başarıyla kabul edilmesi.
- **Test 10:** `Cart_AddItem_QuantityBoundary_ExactlyOneItem_ShouldPass` (**PASSED**)  
  *Girdi:* Adet=1 (BVA). *Beklenen:* Sepete 1 adet eklenebilmesi.

### 3. Gray Box Tests - 5 Test
- **Test 11:** `MinimumOrder_BelowLimit_ShouldThrowException` (**PASSED**)  
  *Girdi:* Sepet=99.00 TL, Limit=100.00 TL (BVA). *Beklenen:* InvalidOperationException fırlatılması.
- **Test 12:** `MinimumOrder_AboveLimit_ShouldSucceed` (**PASSED**)  
  *Girdi:* Sepet=101.00 TL, Limit=100.00 TL (BVA). *Beklenen:* Siparişin başarılı olması.
- **Test 13:** `MinimumOrder_ExactlyAtLimit_ShouldSucceed` (**FAILED - BUG-03**)  
  *Girdi:* Sepet=100.00 TL, Limit=100.00 TL (BVA). *Beklenen:* Siparişin engellenmeden geçmesi.
- **Test 14:** `CartState_EmptyCartOrder_ShouldThrowException` (**PASSED**)  
  *Girdi:* Boş sepet ile checkout. *Beklenen:* Durum geçiş hatası fırlatılması.
- **Test 15:** `Order_CheckStatusAfterSuccessfulPlacement_ShouldBeCompleted` (**PASSED**)  
  *Girdi:* Başarılı sipariş akışı. *Beklenen:* Sipariş durumunun "Completed" olarak güncellenmesi.

### 4. Integration Tests - 5 Test
- **Test 16:** `Order_SuccessfulFlow_ShouldDeductStockAndCompletePayment` (**PASSED**)  
  *Girdi:* Laptop (Stok:10, Fiyat:100), Miktar=2, Bakiye=300. *Beklenen:* Stok=8, Bakiye=100 olması.
- **Test 17:** `Order_InsufficientStock_ShouldThrowException` (**FAILED - BUG-01**)  
  *Girdi:* Laptop (Stok:2), Sipariş Miktarı=5. *Beklenen:* Stok hatası fırlatılması.
- **Test 18:** `Order_PaymentExactBalance_ShouldSucceed` (**FAILED - BUG-06**)  
  *Girdi:* Sipariş=100.00 TL, Kart Bakiyesi=100.00 TL (BVA). *Beklenen:* Ödemenin tamamlanması.
- **Test 19:** `Order_PaymentInsufficientBalance_ShouldThrowException` (**PASSED**)  
  *Girdi:* Sipariş=100.00 TL, Kart Bakiyesi=99.99 TL (BVA). *Beklenen:* Bakiye yetersiz hatası fırlatılması.
- **Test 20:** `Order_ExactStockRemaining_ShouldSucceedAndSetStockToZero` (**PASSED**)  
  *Girdi:* Stok=5, Sipariş=5 (BVA). *Beklenen:* Sipariş sonrasında kalan stok değerinin tam 0 olması.
