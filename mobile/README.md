# VBT-Commerce — Mobile

Trendora e-ticaret platformunun mobil uygulaması. **Flutter** ile yazılmış, iOS ve Android'de çalışan müşteri odaklı bir alışveriş uygulaması. Backend'deki [ECommerce.API](../backend/README.md)'ye REST üzerinden bağlanır. Admin işlevleri bu uygulamada yer almıyor — admin paneli yalnızca [frontend](../frontend/README.md)'de.

---

## 1. Teknoloji Yığını

| Alan | Teknoloji |
|---|---|
| Framework | Flutter (Dart SDK ^3.12.2) |
| Mimari | Clean Architecture (feature bazlı: data / domain / presentation) |
| State yönetimi | flutter_riverpod ^3.0.3 |
| Routing | go_router ^17.0.1 |
| HTTP | dio ^5.9.0 |
| Güvenli depolama | flutter_secure_storage ^10.3.1 (access/refresh token) |
| Basit depolama | shared_preferences ^2.5.4 (kullanıcı önbelleği, anonim kimlik) |
| Model/JSON codegen | freezed + json_serializable + build_runner |
| Diğer | uuid (anonim kart/cart kimliği), intl (tr_TR tarih/para formatı) |

Uygulama içi görünen adı **Trendora**; paket adı `commerce_mobile`. Android/iOS bundle ID'leri hâlâ `com.example.sneaker_store_demo` / `com.example.sneakerStoreDemo` — proje "Sneaker Store" demosu olarak başlamış, marka değişikliği henüz bundle ID'lere yansıtılmamış (gerçek bir yayın öncesi güncellenmesi gerekir).

## 2. Proje Yapısı

```
lib/
├── main.dart              # Giriş noktası: SharedPreferences, ProviderContainer, anonim kimlik bootstrap
├── app.dart                # Kök widget (MaterialApp.router)
├── core/
│   ├── constants/          # API base URL, route path'leri, storage key'leri
│   ├── errors/              # Failure tipleri (Result pattern)
│   ├── navigation/          # Alt gezinme kabuğu (Home/Favoriler/Sepet/Hesap)
│   ├── network/             # dio_client, auth_interceptor (otomatik token yenileme), error mapper
│   ├── router/              # go_router route tanımları, global navigator key
│   ├── services/            # anonymous_id_service, secure_storage_service, storage_service
│   ├── theme/                # Özel tasarım sistemi (colors, typography, spacing, motion)
│   ├── utils/                # currency_formatter, Result<T, Failure>
│   └── widgets/              # LoadingView/ErrorView/EmptyView gibi paylaşılan durum widget'ları
└── features/
    ├── auth/         # login, register, forgot/reset password, session
    ├── home/         # ana sayfa, hero banner
    ├── product/      # ürün listesi, detay, arama, kategori filtresi, sıralama
    ├── cart/         # sepet sayfası + sepet ikon rozeti
    ├── checkout/     # misafir/üye checkout, kupon, kargo firması, ödeme kartı, sipariş özeti
    ├── customer/     # profil, adresler (CRUD)
    ├── orders/        # sipariş listesi/detay, misafir sipariş sorgulama, gönderi takibi
    ├── favorites/     # yerel favoriler listesi
    ├── review/        # ürün yorumları, yorumlarım, yıldız puanlama
    └── account/       # hesap ana sayfası
```

Her feature aynı 3 katmanlı şablonu izler: `data/` (datasource, model, repository impl) → `domain/` (entity, repository arayüzü, usecase) → `presentation/` (sayfa, provider, widget). Bu şablon `mobile/.claude/skills/feature-scaffold/` altında otomatize edilmiştir.

## 3. Ekranlar

- **Auth**: Giriş (`/login`), Kayıt (`/register`), Şifremi Unuttum (`/forgot-password`), Şifre Sıfırlama (`/reset-password`)
- **Ana sayfa** (`/`) — alt gezinmenin 4 sekmesinden biri (Ana Sayfa, Favoriler, Sepet, Hesap)
- **Ürünler**: Ürün listesi (`/products`), ID'ye göre detay (`/product/:id`), slug'a göre detay (`/product/slug/:slug`), Arama (`/search`)
- **Sepet** (`/cart`) — canlı ürün sayısı rozetiyle alt gezinme sekmesi
- **Checkout**: Checkout (`/checkout`), Sipariş Onayı (`/checkout/confirmation`)
- **Hesap**: Hesap ana sayfası (`/account`), Profil (`/profile`), Adresler (`/addresses`)
- **Siparişler**: Sipariş listesi (`/orders`), Sipariş detay (`/orders/:id`), Gönderi Takibi (`/shipment-tracking`), Misafir Sipariş Sorgulama (`/guest-order-lookup`)
- **Yorumlar**: Yorumlarım (`/my-reviews`)
- **Favoriler** — alt gezinme sekmesi (ayrı bir route yok, yerel depolamada tutulur)

## 4. Backend ile Entegrasyon

Base URL, `lib/core/constants/app_constants.dart` içinde build-time ortam değişkeni olarak tanımlı:

```dart
static const apiBaseUrl = String.fromEnvironment('API_BASE_URL', defaultValue: 'https://intern-api.kayas.dev');
static const productApiBaseUrl = String.fromEnvironment('PRODUCT_API_BASE_URL', defaultValue: 'https://intern-api.kayas.dev');
```

Farklı bir backend'e karşı çalıştırmak için `--dart-define` ile geçilebilir (bkz. §6). `.env` mekanizması kullanılmıyor.

`AuthInterceptor` (`lib/core/network/auth_interceptor.dart`) her isteğe `Authorization: Bearer <accessToken>` ekler; 401 aldığında otomatik olarak `POST /api/auth/refresh` çağırıp isteği bir kez tekrar dener, yenileme başarısız olursa kullanıcıyı çıkışa zorlayıp `/login`'e yönlendirir.

Her feature'ın kendi `*_remote_data_source.dart` dosyası (dio ile gerçek HTTP çağrısı) → `*_repository_impl.dart` → domain `usecase`'leri → Riverpod provider/controller katmanı şeklinde akar. Hata yönetimi `Result<T, Failure>` pattern'i ile yapılır (ham exception'lar UI'a sızmaz).

## 5. Kimlik Doğrulama

- **Access/refresh token**: `flutter_secure_storage` ile saklanır (iOS'ta Keychain, Android'de şifreli SharedPreferences).
- **Anonim kimlik**: İlk açılışta `uuid` ile üretilip kalıcı hale getirilir (`anonymous_id_service.dart`); giriş yapmamış kullanıcının sepetini backend'e bağlamak için kullanılır.
- **Kullanıcı önbelleği**: Hassas olmayan veriler (mevcut kullanıcı bilgisi vb.) `shared_preferences` ile saklanır.
- Token yenileme akışının tamamı `AuthInterceptor` içinde, feature kodundan bağımsız şekilde yönetilir.

## 6. Çalıştırma / Build

```bash
flutter pub get
dart run build_runner build --delete-conflicting-outputs   # freezed/json_serializable codegen

flutter run                                                  # varsayılan backend ile
flutter run --dart-define=API_BASE_URL=https://your-api      # farklı bir backend ile

flutter build apk        # Android
flutter build ios        # iOS
flutter build web         # Web (deneysel)
```

**Gereksinimler**: Flutter SDK (Dart ^3.12.2 uyumlu), Android build'leri için Android Studio + Android SDK, iOS/macOS build'leri için Xcode.

## 7. Test

```bash
flutter test
```

Şu an test kapsamı sınırlı: `test/widget_test.dart` (varsayılan iskelet) ve yalnızca Checkout feature'ı için birkaç unit/widget testi (`complete_order_use_case_test.dart`, `coupon_input_test.dart`). Diğer feature'larda henüz test yok.

## 8. Notlar

- Arayüz metinleri ve kod içi yorumlar Türkçe; `intl` açılışta `tr_TR` locale'ini set eder.
- `core/theme/` altında Material varsayılanları yerine özel bir tasarım sistemi (renk, tipografi, spacing, motion) kullanılıyor; `LoadingView`/`ErrorView`/`EmptyView` gibi paylaşılan widget'lar web tarafındaki (`frontend/`) karşılıklarıyla bilinçli olarak benzer tutuluyor.
- Bazı modeller (`user_model`, `customer_address_model`, `product_list_item_model`) freezed/json_serializable ile code-gen edilirken diğerleri elle yazılmış plain Dart sınıfları — bu tutarsızlık bilinçli bir tercih (yeni feature scaffold'unda daha basit yol tercih ediliyor).
- Proje özel Claude Code skill'leri içeriyor: `feature-scaffold` (yeni feature iskeleti) ve `gorev-sonu` (görev sonu `flutter analyze` + `flutter test` kontrolü) — detaylar için `mobile/SKILLS.md`.
