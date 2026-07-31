# VBT-Commerce — Trendora

**Trendora**, tek mağazalı (single-store) bir e-ticaret platformu. Bu repo üç ayrı uygulamayı bir arada barındırır: bir **.NET backend API**'si, bir **Next.js web arayüzü** (müşteri vitrini + admin paneli) ve bir **Flutter mobil uygulama**. Üçü de aynı backend API'ye karşı çalışır — mock veri kullanılmaz.

**Canlı ortam:** [trendora.kayas.dev](https://trendora.kayas.dev) · [API + Scalar dokümantasyonu](https://intern-api.kayas.dev/scalar)

**Videolar:**
- 🎬 [Ürün demosu](https://www.youtube.com/watch?v=uAGmsGNiLcU) — web ve mobil uygulamanın uçtan uca kullanımı
- ⚙️ [Backend anlatımı](https://youtu.be/rOB8YGsTJ_g) — mimari, modüller ve backend'de neler yaptığımız

```
VBT-Commerce/
├── backend/     .NET 10 / ASP.NET Core Web API — Modular Monolith, DDD, CQRS (11 modül, ~104 endpoint)
├── frontend/    Next.js 16 (App Router) + React 19 — mağaza vitrini ve admin paneli
├── mobile/      Flutter — iOS/Android mobil alışveriş uygulaması
└── .github/     CI workflow'ları (backend build & test)
```

Her alt proje kendi README'sinde ayrıntılı olarak anlatılıyor:

- [backend/README.md](backend/README.md) — mimari, modüller, veritabanı, çalıştırma, CI
- [frontend/README.md](frontend/README.md) — sayfalar, admin paneli, API entegrasyonu, çalıştırma
- [mobile/README.md](mobile/README.md) — ekranlar, mimari, çalıştırma/build

---

## Mimari Genel Bakış

```
┌────────────────┐        ┌──────────────────┐
│  frontend/      │        │  mobile/          │
│  (Next.js)      │        │  (Flutter)        │
│  Web + Admin    │        │  iOS/Android      │
└────────┬────────┘        └─────────┬─────────┘
         │        HTTPS (REST + JWT)  │
         └───────────────┬────────────┘
                          ▼
                ┌───────────────────────┐
                │   backend/            │
                │   ASP.NET Core API    │
                │   Modular Monolith    │
                │   (11 modül, CQRS)    │
                └───────────┬───────────┘
                            ▼
                  SQL Server (tek DB,
                  modül başına schema)
```

- **Backend**: Tek deploy edilebilir .NET süreci; içeride Identity, Catalog, Customer, Inventory, Cart, Pricing, Shipping, Payment, Order, Review, Notification olmak üzere 11 modül var. Her modül `Domain → Application → Contracts → Infrastructure` katmanlarına sahip ve modüller birbirine **yalnızca `Contracts` projeleri üzerinden** bağımlı — ayrıntılar için [backend/README.md](backend/README.md).
- **Frontend**: Müşteri vitrini (ana sayfa, ürün listeleme/detay, sepet, checkout, hesabım, sipariş takibi) ve `/admin` altında tam kapsamlı bir yönetim paneli (ürünler, kategoriler, stok, siparişler, ödemeler, kuponlar, kargo, müşteriler, bildirimler) aynı Next.js uygulaması içinde.
- **Mobile**: Flutter ile yazılmış, Clean Architecture (data/domain/presentation) katmanlarına sahip müşteri odaklı alışveriş uygulaması — admin işlevleri mobilde yok.
- **Kimlik doğrulama**: Backend JWT (access + refresh token) üretir. Frontend access token'ı yalnızca bellekte tutar, refresh token'ı HttpOnly cookie üzerinden yönetir. Mobile access/refresh token'ları `flutter_secure_storage` (Keychain/EncryptedSharedPreferences) içinde saklar. Anonim/misafir kullanıcılar için her iki istemci de bir anonim kimlik üretip sepeti buna bağlar.

## Öne Çıkan Özellikler

- **Uçtan uca checkout**: Order modülü, checkout sırasında Inventory / Shipping / Pricing / Payment / Cart modüllerini senkron orkestre eder. Bir adım başarısız olursa önceki adımlar geri alınır (telafi edici aksiyon); rollback çağrısı da düşerse stok rezervasyonlarındaki `ExpiresAt` güvenlik ağı devreye girer.
- **Üye + misafir (guest) akışı**: Kayıt zorunlu değil; misafir checkout, misafir sipariş sorgulama ve giriş anında sepet birleştirme destekleniyor.
- **Katalog**: Kategori ağacı, ürün attribute'ları, renk/beden varyantları, çoklu görsel.
- **Stok yönetimi**: Rezervasyon tabanlı, checkout anında senkron kontrol.
- **Kupon & vergi**: Yüzde/sabit tutar, kategori/ürün/sepet kapsamlı, yığılabilir kuponlar; vergi ve toplam hesaplama motoru.
- **Ödeme**: iyzico (sandbox) entegrasyonu; ödeme sağlayıcısı `IPaymentGateway` arkasında soyutlanmış, Order modülü sağlayıcının kim olduğunu bilmiyor.
- **Bildirim**: Sipariş onay e-postası, sipariş kaydıyla aynı transaction'da outbox'a yazılıp arka planda (BackgroundService) işlenir.
- **Yorum/puan**: Yalnızca ürünü satın almış kullanıcılar yorum yazabilir.
- **Güvenlik**: BCrypt parola hash'leme, refresh token rotation + platform bazlı oturum zinciri, per-IP rate limiting (auth endpoint'lerinde daha sıkı), RFC 7807 ProblemDetails hata formatı.

## Hızlı Başlangıç

Her uygulamanın kendi kurulum adımları kendi README'sinde detaylandırılmıştır; kısaca:

```bash
# 1) Backend (Docker: api + mssql + seq + frontend)
cp backend/.env.example backend/.env     # gerçek değerleri gir
cd backend && docker compose up -d --build
# veya lokal: dotnet run --project backend/src/ECommerce.API

# 2) Frontend
cd frontend
echo "NEXT_PUBLIC_API_URL=https://intern-api.kayas.dev" > .env.local
npm install && npm run dev

# 3) Mobile
cd mobile
flutter pub get
dart run build_runner build --delete-conflicting-outputs
flutter run --dart-define=API_BASE_URL=https://intern-api.kayas.dev
```

Migration'lar API açılışında otomatik uygulanır; Development ortamında örnek Admin/Customer hesabı seed edilir ve `/scalar` üzerinden interaktif API dokümantasyonu açılır.

Seed hesaplarımız işe şu şekilde:

         Admin Kullanıcısı(mail/password) : admin@vbt-commerce.com/Admin123!
         
         Müşteri Kullanıcısı(mail/password) : customer@vbt*commerce.com/Customer123!

## Test & CI

- **Backend**: 11 modülün tamamı için ayrı test projesi — **781 unit test / 210 dosya** (handler + validator seviyesinde).
- **Mobile**: Checkout feature'ı için birkaç unit/widget testi (`flutter test`).
- **Frontend**: Henüz otomatik test altyapısı yok.
- **CI**: [`.github/workflows/backend-ci.yml`](.github/workflows/backend-ci.yml) — `backend/**` altında değişiklik olduğunda `main`'e push/PR'da tetiklenir; `dotnet restore/build (Release)` çalıştırır ve her modülün test projesini (`tests/**/*.Tests.csproj`) ayrı ayrı koşar.

## Geliştirme Akışı (Claude Code skill/agent'ları)

Tekrar eden işler projeye özel skill ve agent'lara dönüştürüldü:

| Konum | Ad | Ne yapar |
|---|---|---|
| `backend/.claude/skills/` | `command-scaffold` | Var olan bir modüle, projenin CQRS/DDD şablonuna uygun Command/Query + Handler + Validator (+ controller action) ekler |
| `backend/.claude/skills/` | `gorev-sonu` | `dotnet build` + `dotnet test` çalıştırır, değişiklik özeti ve mimari kontrol listesi üretir (salt-okunur) |
| `mobile/.claude/skills/` | `feature-scaffold` | Yeni Flutter feature'ını `data/domain/presentation` şablonuyla iskeletler |
| `mobile/.claude/skills/` | `gorev-sonu` | `flutter analyze` + `flutter test` ile görev sonu kontrolü |
| `mobile/.claude/agents/` | `flutter-reviewer` | Clean Architecture, Riverpod deseni ve test kapsamı için salt-okunur kod incelemesi |

Detay: [mobile/SKILLS.md](mobile/SKILLS.md).

## Dağıtım

Sunucuda `backend/docker-compose.yml` ile dört container çalışır: `api`, `frontend`, `mssql` ve `seq` (log görüntüleme). Loglar Serilog ile Console + rolling file + Seq'e akar; nginx reverse proxy `trendora.kayas.dev` → frontend, `intern-api.kayas.dev` → API yönlendirmesini yapar. Ayrıntılar: [backend/README.md §13](backend/README.md).


## Dil / Lokalizasyon

Uygulamanın hedef kitlesi Türkiye pazarı — arayüz metinleri, admin panel ve mobil uygulama Türkçe; para birimi Türk Lirası (₺).
