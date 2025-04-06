# 8 Taş Bulmacası - A* Algoritması

Bu proje, 8 taş bulmacasını (8-puzzle) çözmek için A* algoritmasının implementasyonunu içerir. 8 taş bulmacası, 3x3'lük bir tahta üzerinde 8 taş ve 1 boş hücre içeren klasik bir bulmaca oyunudur.

## Algoritma Açıklaması

### A* Algoritması
A* algoritması, en kısa yolu bulmak için kullanılan bir arama algoritmasıdır. Her durum için iki maliyet hesaplar:
- **G maliyeti**: Başlangıçtan mevcut duruma kadar olan gerçek maliyet (adım sayısı)
- **H maliyeti**: Mevcut durumdan hedef duruma olan tahmini maliyet (Manhattan mesafesi)
- **F maliyeti**: G + H (toplam maliyet)

### Manhattan Mesafesi
Manhattan mesafesi, iki nokta arasındaki yatay ve dikey uzaklıkların toplamıdır. 8 taş bulmacasında, her taşın hedef konumuna olan uzaklığını hesaplamak için kullanılır:
Manhattan Mesafesi = |x1 - x2| + |y1 - y2|

### Algoritmanın Çalışma Prensibi
1. Başlangıç durumu açık listeye eklenir
2. Açık listeden en düşük F değerine sahip durum seçilir
3. Seçilen durum hedef duruma eşitse çözüm bulunmuştur
4. Değilse, durum kapalı listeye eklenir
5. Seçilen durumdan yapılabilecek tüm hamleler üretilir
6. Her yeni durum için F değeri hesaplanır ve açık listeye eklenir
7. 2. adıma dönülür

### Durum Temsili
8 taş bulmacası 3x3'lük bir matris ile temsil edilir:
1 2 3
4 0 5
6 7 8

- 0: Boş hücre
- 1-8: Taşlar

### Hamle Üretme
Her durumdan yapılabilecek hamleler:
- Yukarı (↑)
- Aşağı (↓)
- Sol (←)
- Sağ (→)

### Maliyet Hesaplama
- Her hamle 50 birim maliyete sahiptir
- Manhattan mesafesi kullanılarak tahmini maliyet hesaplanır
- Toplam maliyet = Gerçek maliyet + Tahmini maliyet

### Örnek Senaryo
Başlangıç Durumu: Hedef Durumu:
1 2 3   1 2 3
4 0 5   4 5 6
6 7 8   7 8 0
Adım:
1 2 3
4 5 0
6 7 8
Adım:
1 2 3
4 5 6
7 8 0 


### Algoritmanın Avantajları
- En kısa yolu garanti eder
- Manhattan mesafesi kullanarak etkili tahmin yapar
- Gereksiz durumları ziyaret etmez
- Bellek kullanımı verimlidir

### Zaman Karmaşıklığı
- En kötü durum: O(b^d)
  - b: Her durumdan üretilebilecek maksimum hamle sayısı
  - d: Çözüm derinliği
- Ortalama durum: O(b^d/2)

### Bellek Karmaşıklığı
- O(b^d): Tüm durumların saklanması
- Kapalı liste kullanılarak tekrar eden durumların önlenmesi

## Kod Yapısı

### Durum Sınıfı (Durum.cs)
```csharp
public class Durum
{
    public int[,] Tahta { get; private set; }  // 3x3'lük bulmaca tahtası
    public int BosX { get; set; }              // Boş hücrenin X koordinatı
    public int BosY { get; set; }              // Boş hücrenin Y koordinatı
    public int G { get; set; }                 // Başlangıçtan mevcut duruma maliyet
    public int H { get; set; }                 // Mevcut durumdan hedefe tahmini maliyet
    public int F => G + H;                     // Toplam maliyet
    public Durum Onceki { get; set; }          // Önceki durum (çözüm yolunu takip etmek için)
}
```

### A* Algoritması (AStar.cs)
```csharp
public class AStar
{
    private Durum baslangicDurum;    // Başlangıç durumu
    private Durum hedefDurum;        // Hedef durum
    private List<DurumMaliyet> acikListe;    // Keşfedilecek durumlar
    private HashSet<string> kapaliListe;     // Ziyaret edilmiş durumlar
}
```

## Algoritmanın Adımları

1. **Başlangıç:**
   - Başlangıç durumu ve hedef durum belirlenir
   - Açık liste ve kapalı liste temizlenir
   - Başlangıç durumu açık listeye eklenir

2. **Arama Döngüsü:**
   - Açık listeden en düşük F değerine sahip durum seçilir
   - Seçilen durum hedef duruma eşitse çözüm bulunmuştur
   - Değilse, durum kapalı listeye eklenir
   - Seçilen durumdan yapılabilecek tüm hamleler üretilir
   - Her yeni durum için F değeri hesaplanır ve açık listeye eklenir

3. **Maliyet Hesaplama:**
   - G değeri: Başlangıçtan mevcut duruma kadar olan adım sayısı
   - H değeri: Manhattan mesafesi (her taşın hedef konumuna olan uzaklığı)
   - F değeri: G + H (toplam maliyet)

4. **Hamle Üretme:**"
   - Boş hücrenin etrafındaki 4 yön kontrol edilir
   - Geçerli her hamle için yeni bir durum oluşturulur
   - Yeni durumlar açık listeye eklenir

5. **Çözüm Bulma:* duruma ulaşıldığında çözüm yolu gösterilir
   - Her adımda tahtanın durumu ve maliyet bilgisi verilir

## Algoritmanın Özellikleri

### Optimalite
A* algoritması, uygun bir sezgisel fonksiyon (heuristic) kullanıldığında optimal çözümü garanti eder. Manhattan mesafesi, 8 taş bulmacası için uygun bir sezgisel fonksiyondur çünkü:
- Her zaman gerçek maliyetten küçük veya eşittir
- Monotoniktir (her adımda artış gösterir)

### Tamlık
A* algoritması, çözüm varsa mutlaka bulur. Çünkü:
- Tüm olası durumları sistematik olarak araştırır
- Sonsuz döngülere girmez (kapalı liste kontrolü sayesinde)
- En umut verici durumları öncelikli olarak araştırır

### Verimlilik
A* algoritması, diğer arama algoritmalarına göre daha verimlidir çünkü:
- En umut verici durumları öncelikli olarak araştırır
- Gereksiz durumları ziyaret etmez
- Bellek kullanımını optimize eder

## Sonuç

Bu proje, A* algoritmasının 8 taş bulmacası problemine uygulanmasını göstermektedir. Algoritma, en kısa yolu bulmak için Manhattan mesafesini kullanarak etkili bir şekilde çalışır. Her adımda en umut verici durumları seçerek, hedef duruma en az hamle sayısıyla ulaşmayı başarır.
