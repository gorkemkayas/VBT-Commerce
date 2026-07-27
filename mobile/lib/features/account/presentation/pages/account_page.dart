import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/constants/route_paths.dart';
import '../../../../core/constants/storage_keys.dart';
import '../../../../core/services/secure_storage_service.dart';
import '../../../../core/utils/result.dart';
import '../../../auth/presentation/providers/auth_providers.dart';
import '../../../checkout/presentation/providers/checkout_providers.dart'
    show isLoggedInProvider;

/// Oturum gerektiren bir hesap sayfasına gitmeden önce token kontrolü yapar.
/// Token yoksa sayfaya hiç gidilmez (aksi halde sayfanın kendi provider'ı
/// `/me` ucuna gidip 401 alır ve `AuthInterceptor` kullanıcıyı login'e
/// yönlendirir) — bunun yerine kapatılabilir bir bilgi dialogu gösterilir.
Future<void> _openIfLoggedIn(
  BuildContext context,
  WidgetRef ref,
  String route,
) async {
  final token = await ref
      .read(secureStorageServiceProvider)
      .getString(StorageKeys.accessToken);
  if (token == null) {
    if (context.mounted) await _showLoginRequiredDialog(context);
    return;
  }
  if (context.mounted) context.push(route);
}

Future<void> _showLoginRequiredDialog(BuildContext context) => showDialog<void>(
  context: context,
  builder: (dialogContext) => AlertDialog(
    title: Row(
      mainAxisAlignment: MainAxisAlignment.spaceBetween,
      children: [
        const Text('Giriş Gerekli'),
        IconButton(
          icon: const Icon(Icons.close),
          tooltip: 'Kapat',
          onPressed: () => Navigator.of(dialogContext).pop(),
        ),
      ],
    ),
    content: const Text('Bu özelliği kullanmak için giriş yapmanız gerekiyor.'),
  ),
);

/// Oturum durumuna göre iki farklı hesap ekranı gösterir. Misafirken üyeye
/// özel kartlar (profil, adresler, yorumlar, kargo takibi, çıkış) hiç
/// çizilmez — bunlar giriş olmadan zaten çalışmıyordu ve dokunulduğunda
/// yalnızca "giriş gerekli" dialogu açıyordu.
class AccountPage extends ConsumerWidget {
  const AccountPage({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    // `null` iken oturum durumu henüz okunmadı (kısa süreli secure storage
    // erişimi) — checkout'takiyle aynı desen.
    final loggedIn = ref.watch(isLoggedInProvider).value;
    return Scaffold(
      appBar: AppBar(title: const Text('Hesabım')),
      body: switch (loggedIn) {
        null => const Center(child: CircularProgressIndicator()),
        false => const _GuestAccountView(),
        true => const _MemberAccountView(),
      },
    );
  }
}

/// Misafir görünümü: yalnızca giriş/kayıt ve sipariş no + müşteri no ile
/// sipariş sorgulama. Sorgulama ekranı (`GuestOrderLookupPage`) zaten var,
/// buradan doğrudan ona gidilir.
class _GuestAccountView extends StatelessWidget {
  const _GuestAccountView();

  @override
  Widget build(BuildContext context) => ListView(
    padding: const EdgeInsets.all(16),
    children: [
      Card(
        child: Padding(
          padding: const EdgeInsets.all(16),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(
                'Misafir olarak geziniyorsunuz',
                style: Theme.of(context).textTheme.titleMedium,
              ),
              const SizedBox(height: 4),
              Text(
                'Giriş yaptığınızda siparişleriniz, adresleriniz ve '
                'yorumlarınız hesabınızda saklanır.',
                style: Theme.of(context).textTheme.bodyMedium,
              ),
            ],
          ),
        ),
      ),
      const SizedBox(height: 12),
      _AccountSectionCard(
        icon: Icons.login,
        title: 'Giriş Yap / Kayıt Ol',
        subtitle: 'Hesabınıza girin ya da yeni hesap oluşturun',
        onTap: () => context.push(RoutePaths.login),
      ),
      const SizedBox(height: 12),
      _AccountSectionCard(
        icon: Icons.receipt_long_outlined,
        title: 'Siparişimi Bul',
        subtitle: 'Sipariş no ve müşteri no ile siparişinizi sorgulayın',
        onTap: () => context.push(RoutePaths.guestOrderLookup),
      ),
    ],
  );
}

class _MemberAccountView extends ConsumerWidget {
  const _MemberAccountView();

  @override
  Widget build(BuildContext context, WidgetRef ref) => ListView(
    padding: const EdgeInsets.all(16),
    children: [
      _AccountSectionCard(
        icon: Icons.person_outline,
        title: 'Profil Bilgilerim',
        subtitle: 'Telefon ve doğum tarihi bilgileriniz',
        onTap: () => _openIfLoggedIn(context, ref,RoutePaths.profile),
      ),
      const SizedBox(height: 12),
      _AccountSectionCard(
        icon: Icons.receipt_long_outlined,
        title: 'Siparişlerim',
        subtitle: 'Geçmiş ve devam eden siparişleriniz',
        onTap: () => context.push(RoutePaths.orders),
      ),
      const SizedBox(height: 12),
      _AccountSectionCard(
        icon: Icons.local_shipping_outlined,
        title: 'Kargo Takibi',
        subtitle: 'Aktif siparişlerinizin kargo durumunu görün',
        onTap: () => _openIfLoggedIn(context, ref,RoutePaths.shipmentTracking),
      ),
      const SizedBox(height: 12),
      _AccountSectionCard(
        icon: Icons.location_on_outlined,
        title: 'Adreslerim',
        subtitle: 'Teslimat adreslerinizi yönetin',
        onTap: () => _openIfLoggedIn(context, ref,RoutePaths.addresses),
      ),
      const SizedBox(height: 12),
      _AccountSectionCard(
        icon: Icons.rate_review_outlined,
        title: 'Yorumlarım',
        subtitle: 'Yazdığınız değerlendirmeleri yönetin',
        onTap: () => _openIfLoggedIn(context, ref,RoutePaths.myReviews),
      ),
      const SizedBox(height: 12),
      _AccountSectionCard(
        icon: Icons.logout,
        title: 'Çıkış Yap',
        subtitle: 'Hesabınızdan çıkış yapın',
        onTap: () async {
          // Çıkış + kullanıcıya bağlı durumun sıfırlanması tek yerde:
          // sepet, favoriler, profil ve oturum bayrağı burada tazelenir
          // (bkz. `resetUserScopedState`) — hesap sekmesi `IndexedStack`
          // içinde ayakta kaldığından kendiliğinden yenilenmiyorlar.
          final result = await ref
              .read(sessionControllerProvider.notifier)
              .logout();
          if (!context.mounted) return;
          switch (result) {
            case Success<bool>():
              context.go(RoutePaths.login);
            case ResultFailure<bool>(:final failure):
              ScaffoldMessenger.of(
                context,
              ).showSnackBar(SnackBar(content: Text(failure.message)));
          }
        },
      ),
    ],
  );
}

class _AccountSectionCard extends StatelessWidget {
  const _AccountSectionCard({
    required this.icon,
    required this.title,
    required this.subtitle,
    this.onTap,
  });

  final IconData icon;
  final String title;
  final String subtitle;
  final VoidCallback? onTap;

  @override
  Widget build(BuildContext context) => Card(
    child: ListTile(
      leading: Icon(icon),
      title: Text(title),
      subtitle: Text(subtitle),
      trailing: const Icon(Icons.chevron_right),
      onTap:
          onTap ??
          () => ScaffoldMessenger.of(context).showSnackBar(
            const SnackBar(content: Text('Yakında kullanıma açılacak.')),
          ),
    ),
  );
}
