---
name: command-scaffold
description: Var olan bir modülün (Catalog, Order, Pricing vb.) Application katmanına, projenin CQRS/DDD şablonuna uygun yeni bir Command veya Query (+Handler+Validator) ekler ve gerekirse ilgili Controller'a endpoint bağlar. Önce plan sunar, kullanıcı onayı olmadan hiçbir dosya yazmaz.
---

# command-scaffold

## Amaç
Bu backend'de her yeni Command/Query eklemede tekrar eden 3 dosyayı (`<İsim>Command.cs`/`<İsim>Query.cs`, `<İsim>Handler.cs`, `<İsim>Validator.cs`) — gerekirse Controller'a bağlanan bir action ile birlikte — projenin gözlemlenen şablonuna birebir uyumlu şekilde oluşturur (bkz. `Catalog.Application/Commands/Products/UpdateProductVariant/*`).

## Kesin Kurallar (istisnasız)
- **Yeni bir modül oluşturmaz.** İstenen modül `backend/src/Modules/` altındaki 11 modülden biri değilse dur, kullanıcıya modül oluşturmanın bu skill'in kapsamı dışında olduğunu söyle.
- Var olan Command/Query/Handler/Validator dosyalarını **değiştirmez** — tek istisna, Controller'a yeni bir action eklerken o dosyaya `Edit` ile dokunmaktır.
- Migration oluşturmaz/uygulamaz, `dotnet build`/`dotnet test` çalıştırmaz (bu iş `gorev-sonu` skill'inin).
- git commit/push çalıştırmaz.
- Aynı isimde klasör (`<Modül>.Application/Commands|Queries/<Alan>/<İsim>/`) zaten varsa hiçbir dosya oluşturmaz, sadece durumu bildirir.
- Kullanıcı açıkça onaylamadan `Write`/`Edit` çağırmaz.

## Şablon (bu kod tabanında gözlenen desen)
- `<Modül>.Application/Commands|Queries/<Alan>/<İsim>/<İsim>Command.cs`
  `public record <İsim>Command(...) : ICommand<TResponse>` — rol kısıtı gerekiyorsa `, IRequireRole` ekleyip `AllowedRoles => ["Admin"]` gibi tanımla.
- `<İsim>CommandHandler.cs`
  `IRequestHandler<<İsim>Command, TResponse>` — constructor'da yalnızca kendi modülünün `I<Modül>DbContext`'ini (ve varsa `Integrations/` altındaki adapter arayüzlerini) alır; iş kuralını mümkün olduğunca domain entity metoduna devreder (ör. `product.UpdateVariant(...)`), handler ince kalır.
- `<İsim>CommandValidator.cs`
  `AbstractValidator<<İsim>Command>` — `RuleFor`/`RuleForEach` ile `NotEmpty`/`MaximumLength` gibi temel kontroller.
- (Yalnızca yeni bir HTTP endpoint gerekiyorsa) `ECommerce.API/Controllers/<Modül>/<Modül>Controller.cs`'e action ekle — action yalnızca `await mediator.Send(new <İsim>Command(...))` çağırır, iş mantığı içermez.

## Akış
1. Kullanıcıdan netleştir: hangi modül, Command mı Query mi, hangi alan (klasör) altında, hangi parametreler/dönüş tipi, rol kısıtı var mı, yeni bir controller action'ı gerekiyor mu.
2. `backend/src/Modules/<Modül>` var mı `Glob`/`Read` ile doğrula — yoksa dur.
3. Hedef klasör zaten varsa dur, mevcut içeriği raporla.
4. Oluşturulacak dosyaların tam yollarını ve her birinin taslak içeriğini kullanıcıya göster, onay bekle.
5. Onay sonrası yalnızca adım 4'te listelenen dosyaları `Write` ile oluştur; controller değişikliği varsa `Edit` ile ekle.
6. Sonuç raporu + "sırada ne var" notu (yeni bir entity alanı/DB kolonu gerekiyorsa migration, test yazımı — bunlar bilerek bu skill'in kapsamı dışında, `gorev-sonu` veya insan kararı gerektirir).

## Bilinçli Olarak Yapılmayanlar
Yeni modül oluşturma, EF Core migration oluşturma/uygulama, test yazma, `dotnet build`/`dotnet test` çalıştırma, git işlemleri.