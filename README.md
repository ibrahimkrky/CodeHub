# CodeHub — Kod Parçacığı ve Geliştirici Kaynak Merkezi

CodeHub, yazılım geliştiricilerin kod parçacıklarını (snippet) paylaştığı, kategorize ettiği, beğendiği ve birbirlerinin kodlarını inceleyebildiği web tabanlı bir topluluk platformudur. ASP.NET Core MVC mimarisi üzerine kurulmuştur.

---

## Kullanılan Teknolojiler

| Teknoloji | Amaç |
|-----------|------|
| ASP.NET Core MVC (.NET 10) | Sunucu tarafı mimari (Controller-View-Model) |
| Entity Framework Core 10 | Code-First ORM, veritabanı modelleme |
| SQL Server (LocalDB) | İlişkisel veritabanı |
| ASP.NET Core Identity | Kullanıcı kayıt, giriş ve kimlik doğrulama |
| Bootstrap 5 | Duyarlı arayüz, grid, kart, modal bileşenleri |
| JavaScript (Vanilla) | İstemci tarafı filtreleme, AJAX, etkileşimler |
| Chart.js | Analiz panelinde grafikler |
| highlight.js | Kod bloklarında sözdizimi renklendirmesi |

---

## Kurulum ve Çalıştırma

### Gereksinimler
- .NET 10 SDK
- SQL Server LocalDB (Visual Studio ile birlikte gelir)

### Adımlar

1. Proje klasörüne girin:
   ```
   cd CodeHub
   ```

2. Bağımlılıkları yükleyin:
   ```
   dotnet restore
   ```

3. Veritabanını oluşturun (migration'ları uygular):
   ```
   dotnet ef database update
   ```

4. Uygulamayı çalıştırın:
   ```
   dotnet run
   ```

5. Tarayıcıda açın: `https://localhost:xxxx` (terminalde yazan port)

> Uygulama ilk açılışta veritabanını örnek verilerle (demo kullanıcılar, snippet'ler, koleksiyonlar) otomatik doldurur.

> **Not:** `dotnet ef` komutu tanınmıyorsa şu komutla EF araçlarını kurun:
> ```
> dotnet tool install --global dotnet-ef --version 10.0.8
> ```

---

## Demo Hesaplar

Örnek verilerle birlikte gelen test kullanıcıları:

| E-posta | Parola |
|---------|--------|
| ayse@codehub.dev | Parola123! |
| mehmet@codehub.dev | Parola123! |
| zeynep@codehub.dev | Parola123! |

İstediğiniz gibi yeni bir hesap da kaydedebilirsiniz.

---

## Özellikler

- **Keşfet paneli:** Kod parçacıkları kartlar halinde; anlık arama, dile göre filtreleme ve sıralama (en yeni, en popüler, en çok görüntülenen).
- **Snippet detayı:** Sözdizimi renklendirmesi, tek tıkla kopyalama, AJAX ile beğeni, koleksiyona ekleme.
- **Snippet ekleme/düzenleme:** Çift taraflı doğrulama (istemci + sunucu), dinamik etiketleme, sahiplik kontrolü.
- **Koleksiyonlar:** Kişisel kod klasörleri; oluşturma, yeniden adlandırma, silme ve içerik yönetimi.
- **Etiketler dizini:** Etiketlere göre snippet listeleme, canlı arama.
- **Kod inceleme odası:** Soru paylaşma, AJAX ile oylama, çözüm işaretleme, yorum düzenleme/silme.
- **Liderlik tablosu:** Geliştiricilerin puana göre sıralanması, sütun bazlı anlık sıralama.
- **Analiz paneli:** Chart.js ile kişisel istatistik grafikleri.
- **Profil:** Bilgi ve parola güncelleme; kullanıcının kendi snippet ve incelemeleri.
- **Açık / koyu tema:** Çerez tabanlı, tüm sayfalarda kalıcı tema geçişi.
- **Kimlik doğrulama:** Kayıt, giriş, "beni hatırla" ve şifremi unuttum akışı.

---

## Proje Yapısı

```
CodeHub/
├── Controllers/        # İstekleri karşılayan denetleyiciler
├── Models/
│   ├── Entities/       # Veritabanı varlıkları (Snippet, Tag, Review, ...)
│   └── ViewModels/     # Görünümlere özel veri modelleri
├── Views/              # Razor (.cshtml) arayüz şablonları
├── Data/
│   ├── ApplicationDbContext.cs   # EF Core veritabanı bağlamı
│   └── SeedData.cs               # Örnek veri ekleme
├── Migrations/         # EF Core veritabanı migration'ları
├── wwwroot/
│   ├── css/            # Stil dosyaları (tema sistemi dahil)
│   └── js/             # İstemci tarafı betikler
├── Program.cs          # Uygulama başlangıç yapılandırması
└── appsettings.json    # Bağlantı dizesi ve ayarlar
```

---

## Veritabanı

Veritabanı EF Core Code-First yaklaşımıyla oluşturulur. Temel varlıklar: `ApplicationUser`, `Snippet`, `Tag`, `Collection`, `Review`, `Comment`, `Vote`, `Star`. Snippet–Tag ve Collection–Snippet arasında çoka-çok ilişkiler ara tablolarla (`SnippetTag`, `CollectionSnippet`) modellenmiştir.

Verileri sıfırlamak isterseniz:
```
dotnet ef database drop --force
dotnet ef database update
dotnet run
```

---

İbrahim Karakaya — 230309015
