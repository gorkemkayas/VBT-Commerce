---
name: gorev-sonu
description: Bir geliştirme görevi tamamlandıktan sonra flutter analyze ve flutter test çalıştırır; git diff üzerinden bir değişiklik özeti ve CLAUDE.md kurallarına göre bir kontrol listesi üretir. Hiçbir kod veya dosya değişikliği yapmaz.
disallowed-tools: Edit, Write
---

# gorev-sonu

## Amaç
Her görev sonunda tekrarlanan doğrulama adımlarını (statik analiz, testler, ne değiştiğinin özeti, kurallara uygunluk kontrolü) tek komutla, tutarlı bir formatta raporlar.

## Kesin Kurallar
- Hiçbir dosyayı değiştirme veya oluşturma. (`Edit`/`Write` bu skill için zaten devre dışı.)
- git commit/push çalıştırma — yalnızca salt-okunur git komutları (`git status`, `git diff`) kullanılabilir, asla `git commit`/`git push`/`git checkout` değil.
- Yalnızca bu Flutter projesinin (mobile) kapsamında çalış; üst dizindeki `backend/`/`frontend/`'e dokunma.

## Akış
1. `flutter analyze` çalıştır, tam çıktıyı özetle (kaç uyarı/hata, varsa dosya:satır).
2. `flutter test` çalıştır, sonucu özetle (kaç test geçti/kaç başarısız).
3. `git status` + `git diff --stat` (salt-okunur) ile "değişiklik özeti" üret: hangi dosyalar değişti/eklendi, kaç satır.
4. `CLAUDE.md`'deki kurallara göre bir **kontrol listesi** üret, ör:
   - [ ] Mevcut kod yeniden yazılmadı mı?
   - [ ] Gereksiz refactor yapılmadı mı?
   - [ ] Mimari (Clean Architecture) korundu mu?
   - [ ] Dosya taşıma yapılmadı mı?
   - [ ] `flutter analyze` temiz mi?
   - [ ] `flutter test` başarılı mı?
5. Tüm bunları tek, okunabilir bir raporda birleştir.

## Çıktı Formatı
```
## Gorev Sonu Raporu
### flutter analyze
...
### flutter test
...
### Değişiklik Özeti
...
### Kontrol Listesi
- [x] ...
- [ ] ...
```

## Not (şeffaflık)
`Bash` aracı bu skill için devre dışı bırakılmadı (flutter analyze/test'i çalıştırmak için gerekli) — bu yüzden "git commit/push yapma" kuralı burada teknik değil, talimat düzeyinde uygulanıyor. Bu, `feature-scaffold`'daki (Bash tamamen kapalı) garantiden daha zayıf bir güvencedir; bilerek böyle, çünkü bu skill'in meşru işi zaten shell komutu çalıştırmayı gerektiriyor.
