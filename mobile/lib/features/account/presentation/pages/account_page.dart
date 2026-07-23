import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/constants/route_paths.dart';
import '../../../../core/constants/storage_keys.dart';
import '../../../../core/services/secure_storage_service.dart';
import '../../../../core/utils/result.dart';
import '../../../auth/presentation/providers/auth_providers.dart';

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

class AccountPage extends ConsumerWidget {
  const AccountPage({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) => Scaffold(
    appBar: AppBar(title: const Text('Hesabım')),
    body: ListView(
      padding: const EdgeInsets.all(16),
      children: [
        _AccountSectionCard(
          icon: Icons.person_outline,
          title: 'Profil Bilgilerim',
          subtitle: 'Telefon ve doğum tarihi bilgileriniz',
          onTap: () => _openIfLoggedIn(context, ref, RoutePaths.profile),
        ),
        const SizedBox(height: 12),
        _AccountSectionCard(
          icon: Icons.receipt_long_outlined,
          title: 'Siparişlerim',
          subtitle: 'Geçmiş ve devam eden siparişleriniz',
          onTap: () => _openIfLoggedIn(context, ref, RoutePaths.orders),
        ),
        const SizedBox(height: 12),
        _AccountSectionCard(
          icon: Icons.location_on_outlined,
          title: 'Adreslerim',
          subtitle: 'Teslimat adreslerinizi yönetin',
          onTap: () => _openIfLoggedIn(context, ref, RoutePaths.addresses),
        ),
        const SizedBox(height: 12),
        _AccountSectionCard(
          icon: Icons.logout,
          title: 'Çıkış Yap',
          subtitle: 'Hesabınızdan çıkış yapın',
          onTap: () async {
            final result = await ref.read(logoutUseCaseProvider)();
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
    ),
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
