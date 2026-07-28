---
name: gorev-sonu
description: Backend'de bir geliştirme görevi tamamlandıktan sonra dotnet build ve dotnet test çalıştırır; git diff üzerinden bir değişiklik özeti ve modüler monolit/DDD/CQRS kurallarına göre bir kontrol listesi üretir. Hiçbir kod veya dosya değişikliği yapmaz.
disallowed-tools: Edit, Write
---

# gorev-sonu (backend)

## Amaç
Backend'de her görev sonunda tekrarlanan doğrulama adımlarını (derleme, testler, ne değiştiğinin özeti, mimari kurallara uygunluk) tek komutla, tutarlı bir formatta raporlar.

## Kesin Kurallar
- Hiçbir dosyayı değiştirme veya oluşturma. (`Edit`/`Write` bu skill için zaten devre dışı.)
- git commit/push/checkout çalıştırma — yalnızca salt-okunur git komutları (`git status`, `git diff`) kullanılabilir.
- Yalnızca bu backend'in (`backend/`) kapsamında çalış; `frontend/`/`mobile/`'a dokunma.

## Akış
1. `dotnet build backend/ECommerce.slnx` çalıştır, tam çıktıyı özetle (kaç uyarı/hata, varsa dosya:satır).
2. `dotnet test backend/ECommerce.slnx` çalıştır, sonucu özetle (kaç test geçti/kaç başarısız, başarısızsa hangi test).
3. `git status` + `git diff --stat` (salt-okunur) ile "değişiklik özeti" üret: hangi dosyalar değişti/eklendi, kaç satır.
4. `backend/README.md`'deki mimari kurallara göre bir **kontrol listesi** üret, ör:
   - [ ] Değişen/eklenen modül başka bir modülün `Application`/`DbContext`'ine doğrudan referans vermiyor mu — yalnızca hedef modülün `Contracts`'ı üzerinden mi konuşuyor?
   - [ ] Modüller arası kritik senkron çağrı varsa, tüketen modülün kendi `Integrations/` adapter'ı üzerinden mi yapılıyor (`Contracts` doğrudan Application'da kullanılmıyor)?
   - [ ] Yeni bir Command/Query eklendiyse bir `Validator` var mı, rol kısıtı gerekiyorsa `IRequireRole`/`AllowedRoles` tanımlı mı?
   - [ ] Controller action'ları yalnızca `mediator.Send(...)` çağırıyor mu, iş mantığı sızdırmıyor mu?
   - [ ] Yeni bir entity alanı/tablo değişikliği varsa migration eklendi mi (`dotnet ef migrations add`)?
   - [ ] `dotnet build` temiz mi?
   - [ ] `dotnet test` başarılı mı?
5. Tüm bunları tek, okunabilir bir raporda birleştir.

## Çıktı Formatı
```
## Gorev Sonu Raporu (backend)
### dotnet build
...
### dotnet test
...
### Değişiklik Özeti
...
### Kontrol Listesi
- [x] ...
- [ ] ...
```

## Not (şeffaflık)
`Bash` aracı bu skill için devre dışı bırakılmadı (`dotnet build`/`dotnet test`'i çalıştırmak için gerekli) — bu yüzden "git commit/push yapma" kuralı burada teknik değil, talimat düzeyinde uygulanıyor. Bu, `command-scaffold`'daki garantiden daha zayıf bir güvencedir; bilerek böyle, çünkü bu skill'in meşru işi zaten shell komutu çalıştırmayı gerektiriyor.