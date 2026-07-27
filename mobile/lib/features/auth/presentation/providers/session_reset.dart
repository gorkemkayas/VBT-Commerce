import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/constants/storage_keys.dart';
import '../../../../core/services/storage_service.dart';
import '../../../cart/presentation/providers/cart_providers.dart';
import '../../../checkout/presentation/providers/checkout_providers.dart'
    show isLoggedInProvider, savedGuestContactProvider;
import '../../../customer/presentation/providers/customer_providers.dart';
import '../../../favorites/presentation/providers/favorites_providers.dart';
import '../../../orders/presentation/providers/order_providers.dart';
import 'auth_providers.dart';

/// Oturum değiştiğinde (giriş, kayıt, çıkış) kullanıcıya bağlı her şeyi
/// sıfırlar. Bu olmadan bir hesaptan çıkıp başka bir hesaba girildiğinde
/// önceki kullanıcının sepeti, favorileri ve profil bilgisi ekranda kalıyordu:
///
/// * `cartControllerProvider`, `favoritesControllerProvider` ve
///   `ordersControllerProvider` uygulama ömrü boyunca yaşayan
///   `NotifierProvider`'lar — `build()` yalnızca bir kez çalışıp veriyi
///   yüklüyor, oturum değişince kendiliğinden yeniden yüklenmiyorlar.
/// * `currentUserProvider` `autoDispose` değil; yereldeki eski kullanıcıyı
///   (profildeki e-posta) süresiz önbellekliyor.
/// * Sepet snapshot'ları, favoriler ve misafir müşteri kaydı tek bir cihaz
///   anahtarında tutuluyor; kullanıcıya göre ayrılmıyor.
///
/// Çağrı sırası önemlidir: **önce** token'lar yazılmış (giriş) ya da
/// silinmiş (çıkış) olmalı, sonra bu fonksiyon çağrılmalı — yeniden kurulan
/// controller'lar sepeti doğru uçtan (`/api/carts/me` ya da
/// `/api/carts/anonymous/{id}`) çeksin diye.
Future<void> resetUserScopedState(Ref ref) async {
  // Yereldeki kullanıcıya özel veriler. Sepet snapshot'ları backend'in kalem
  // id'lerine göre saklandığından başka bir hesapta zaten eşleşmez; favoriler
  // ve misafir iletişim bilgisi ise doğrudan bir önceki kullanıcıya aitti.
  final storage = ref.read(storageServiceProvider);
  await storage.remove(StorageKeys.cartItemSnapshots);
  await storage.remove(StorageKeys.favoriteItems);
  await storage.remove(StorageKeys.guestCustomerId);

  // Sunucudan yeniden çekilecek olanlar.
  ref.invalidate(cartControllerProvider);
  ref.invalidate(favoritesControllerProvider);
  ref.invalidate(ordersControllerProvider);
  ref.invalidate(currentUserProvider);
  ref.invalidate(currentCustomerProvider);
  ref.invalidate(isLoggedInProvider);
  ref.invalidate(savedGuestContactProvider);
}
