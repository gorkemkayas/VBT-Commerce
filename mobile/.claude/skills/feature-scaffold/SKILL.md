---
name: feature-scaffold
description: Yeni bir mobile feature için Clean Architecture (data/domain/presentation) boilerplate'ini standart şablonla oluşturur. Önce plan sunar, kullanıcı onayı olmadan hiçbir dosya yazmaz; mevcut dosyalara dokunmaz, git komutu çalıştırmaz.
disallowed-tools: Edit, Bash
---

# feature-scaffold

## Amaç
`lib/features/<isim>/` altında, projenin mevcut Clean Architecture desenine uygun 6 temel dosyayı (entity, repository interface, use case, mock repository implementation, provider+state+controller, page) standart bir şablonla oluşturur.

## Kesin Kurallar (istisnasız)
- Mevcut hiçbir dosyayı düzenleme. (`Edit` aracı bu skill için zaten devre dışı.)
- git commit/push çalıştırma, branch değiştirme. (`Bash` aracı bu skill için zaten devre dışı — teknik olarak mümkün değil.)
- `lib/features/<isim>/` zaten varsa hiçbir dosya oluşturma, sadece durumu bildir.
- Kullanıcı onayı olmadan `Write` çağırma.

## Akış
1. **İsim doğrulama:** Kullanıcının verdiği isim yalnızca küçük harf/rakam/alt çizgi içermeli (`^[a-z][a-z0-9_]*$`). Uymuyorsa dur, sebebini açıkla.
2. **Çakışma kontrolü (salt-okunur, `Read`/`Glob` ile):** `lib/features/<isim>/` var mı bak. Varsa dur ve mevcut içeriği raporla — hiçbir şey oluşturma.
3. **Plan sunumu:** Oluşturulacak 6 dosyanın tam yolunu ve her birinin ne içereceğini (tek satır özet) kullanıcıya göster.
4. **Onay bekle.** Kullanıcı açıkça onaylamadan hiçbir `Write` çağrısı yapılmaz.
5. **Onay sonrası:** Yalnızca `Write` ile, yalnızca adım 3'te listelenen 6 dosyayı sırayla oluştur.
6. **Sonuç raporu:** Oluşturulan dosyaların listesi + "şimdi ne yapılmalı" notu (router'a ekleme, backend entegrasyonu, test dosyası — bunlar bilerek bu skill'in kapsamı dışında, insan kararı gerektirir).

## Şablon Dosyalar
- `lib/features/<isim>/domain/entities/<isim>.dart`
- `lib/features/<isim>/domain/repositories/<isim>_repository.dart`
- `lib/features/<isim>/domain/usecases/get_<isim>s_use_case.dart`
- `lib/features/<isim>/data/repositories/<isim>_repository_impl.dart` (mock implementasyon — gerçek backend entegrasyonu sonradan eklenir)
- `lib/features/<isim>/presentation/providers/<isim>_providers.dart` (`Notifier`/`Provider` DI zinciri, Cart/Checkout feature'larındaki desenle aynı)
- `lib/features/<isim>/presentation/pages/<isim>_page.dart` (`LoadingView`/`ErrorView`/`EmptyView` ile state render eden mevcut sayfa deseniyle aynı)

## Bilinçli Olarak Yapılmayanlar
Router/route ekleme, test dosyası, Freezed model, gerçek datasource/backend entegrasyonu, `flutter analyze`/`test` çalıştırma (bu `gorev-sonu`'nun işi).
