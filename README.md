# VBT-Commerce

**Trendora**, tek mağazalı (single-store) bir e-ticaret platformu. Bu repo üç ayrı uygulamayı bir arada barındırır: bir **.NET backend API**'si, bir **Next.js web arayüzü** (müşteri vitrini + admin paneli) ve bir **Flutter mobil uygulama**. Üçü de aynı backend API'ye karşı çalışır.

```
VBT-Commerce/
├── backend/     .NET 10 / ASP.NET Core Web API — Modular Monolith, DDD, CQRS
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

- **Backend**: Tek deploy edilebilir .NET süreci; içeride Identity, Catalog, Customer, Inventory, Cart, Pricing, Shipping, Payment, Order, Review, Notification olmak üzere 11 modül var. Modüller birbirine yalnızca `Contracts` projeleri üzerinden bağımlı — ayrıntılar için [backend/README.md](backend/README.md).
- **Frontend**: Müşteri vitrini (ana sayfa, ürün listeleme/detay, sepet, checkout, hesabım, sipariş takibi) ve `/admin` altında tam kapsamlı bir yönetim paneli (ürünler, kategoriler, stok, siparişler, ödemeler, kuponlar, kargo, müşteriler, bildirimler) aynı Next.js uygulaması içinde.
- **Mobile**: Flutter ile yazılmış, Clean Architecture (data/domain/presentation) katmanlarına sahip müşteri odaklı alışveriş uygulaması — admin işlevleri mobilde yok.
- **Kimlik doğrulama**: Backend JWT (access + refresh token) üretir. Frontend access token'ı yalnızca bellekte tutar, refresh token'ı HttpOnly cookie üzerinden yönetir. Mobile access/refresh token'ları `flutter_secure_storage` (Keychain/EncryptedSharedPreferences) içinde saklar. Anonim/misafir kullanıcılar için her iki istemci de bir anonim kimlik üretip sepeti buna bağlar.

## Hızlı Başlangıç

Her uygulamanın kendi kurulum adımları kendi README'sinde detaylandırılmıştır; kısaca:

1. **Backend**'i ayağa kaldır (Docker Compose ile SQL Server + API + Seq, veya lokal `dotnet run`) → bkz. [backend/README.md](backend/README.md)
2. **Frontend**'i çalıştır: `frontend/.env.local` içinde `NEXT_PUBLIC_API_URL` backend adresini gösterecek şekilde ayarlanır, `npm install && npm run dev` → bkz. [frontend/README.md](frontend/README.md)
3. **Mobile**'ı çalıştır: `flutter pub get`, gerekirse `--dart-define=API_BASE_URL=...` ile backend adresini geçerek `flutter run` → bkz. [mobile/README.md](mobile/README.md)

## CI

`.github/workflows/backend-ci.yml` — `backend/**` altında değişiklik olduğunda `main`'e push/PR'da tetiklenir; `dotnet restore/build` çalıştırır ve her modülün test projesini (`tests/**/*.Tests.csproj`) ayrı ayrı çalıştırır.

## Dil / Lokalizasyon

Uygulamanın hedef kitlesi Türkiye pazarı — arayüz metinleri, admin panel ve mobil uygulama Türkçe; para birimi Türk Lirası (₺).
