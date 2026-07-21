# Commerce Mobile — Geliştirici Araçları

Bu proje, Clean Architecture (data/domain/presentation)
katmanlarıyla geliştirilen bir Flutter uygulamasıdır.

Uygulama canlı backend ile entegre çalışmaktadır.
Mimari, backend değişikliklerinde mümkün olduğunca yalnızca
Data katmanının etkilenmesini hedefleyecek şekilde tasarlanmıştır.

Bu dosya, projede tekrar eden geliştirme işlerini otomatikleştiren proje-özel skill'leri belgeler. Genel amaçlı araçlar değildir; her biri bu projenin mimarisine ve geliştirme kurallarına özel olarak tasarlanmıştır.

Skill'ler önem sırasına göre listelenmiştir: `feature-scaffold` projenin ana otomasyon aracıdır; `gorev-sonu` ise geliştirme sürecini destekleyen yardımcı bir araçtır.

---

## `feature-scaffold`

**Konum:** `.claude/skills/feature-scaffold/SKILL.md`

**Ne işe yarar:** Yeni bir feature adı verildiğinde, Clean Architecture klasör yapısını ve 6 temel boilerplate dosyasını (entity, repository interface, use case, mock repository implementation, provider+state+controller, page) otomatik oluşturur.

**Neden var:** Auth, Product, Cart ve Checkout feature'larının her biri elle, aynı 6 dosyayı, aynı katmanlara, aynı isimlendirme kalıbıyla kurularak oluşturuldu. Bu tekrar, projede zaman alan, hataya açık olabilen (özellikle Result/Failure/Notifier deseninin elle her seferinde doğru tekrarlanması) bir iş. Skill bu iskeleti hızlıca, aynı proje standardına uygun şekilde üretir.

**Mevcut proje yapısına uyumu:** Şablon, projedeki iki geçerli desenden **daha basit ve daha genel olanına** (Checkout'un mock-repository deseni) dayanıyor — Model/DataSource/Freezed içermiyor; backend entegrasyonu feature ihtiyaçlarına göre sonradan eklenebilir. Use case, Cart'ın `GetCartItemsUseCase`'i gibi tek repository'ye bağımlı, minimal bir iskelet — Checkout'un çapraz-feature bağımlılığı (`CartRepository`) ve entity-seviyesi validasyonu gibi feature'a özel detaylar şablona kopyalanmadı. Provider dosyası, Cart'ın `cart_providers.dart`'ındaki DI zinciri + `Notifier<State>` controller desenini yakından izliyor. Sayfa, `LoadingView`/`ErrorView`/`EmptyView` ile switch-üzerinden state render eden ProductListPage/CartPage desenini kullanıyor.

**Bilinçli olarak yapmadıkları:** Router/route ekleme, test dosyası, Maestro dosyası, Freezed model, datasource, backend entegrasyonu — hiçbiri skill'in kapsamında değil (insan kararı gerektirir veya henüz erken). Skill sadece yeni dosya oluşturur, hiçbir mevcut dosyayı düzenlemez — bu sayede mevcut feature'ları (Auth/Product/Cart/Checkout) etkilemeyecek şekilde tasarlanmıştır.

**Kullanım örneği:**

```
/feature-scaffold wishlist
```

Bu çağrı şu dosyaları oluşturur:

```
lib/features/wishlist/domain/entities/wishlist.dart
lib/features/wishlist/domain/repositories/wishlist_repository.dart
lib/features/wishlist/domain/usecases/get_wishlists_use_case.dart
lib/features/wishlist/data/repositories/wishlist_repository_impl.dart
lib/features/wishlist/presentation/providers/wishlist_providers.dart
lib/features/wishlist/presentation/pages/wishlist_page.dart
```

Ardından geliştiricinin entity'ye gerçek alanları eklemesi, sayfayı router'a bağlaması ve `WishlistRepositoryImpl`'i ilgili backend endpoint'lerine bağlaması beklenir.


## `gorev-sonu`

**Konum:** `.claude/skills/gorev-sonu/SKILL.md`

**Ne işe yarar:** `dart format`, `flutter analyze`, `flutter test` üçlüsünü çalıştırır ve sonucu tek, tutarlı bir formatta raporlar.

**Neden var:** Proje geliştirme sürecinde her görev tamamlandığında kodun biçimlendirilmiş, statik analizden geçmiş ve testlerin başarılı olması bekleniyor. Bu üç adım her görevde tekrarlandığı için tek komutla çalıştırılabilir hale getirildi.

**Nasıl kullanılır:**

```
/gorev-sonu
```

Kod değiştiren görevlerin sonunda çağrılması önerilir.