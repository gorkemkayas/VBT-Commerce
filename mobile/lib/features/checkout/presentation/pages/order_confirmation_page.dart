import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/constants/route_paths.dart';
import '../../../../core/utils/currency_formatter.dart';

/// `CheckoutPage`'in başarılı sipariş sonrası bu ekrana geçerken taşıdığı
/// veri — hiçbiri yeniden sorgulanmaz, tamamı zaten `CheckoutState.order`'da
/// (ve misafir dalında `guestCustomerId`'de) mevcuttur.
class OrderConfirmationArgs {
  const OrderConfirmationArgs({
    required this.orderId,
    required this.total,
    this.guestCustomerId,
  });

  final String orderId;

  /// Sepet kalemlerinin toplamı (`Order.total`) — vergi/kargo hesaba
  /// katılmadan önceki yaklaşık tutar; `0` ise gösterilmez.
  final double total;

  /// Misafir siparişinde dolu, üyeli siparişte `null`.
  final String? guestCustomerId;
}

/// Web'deki `/checkout/[orderId]` onay sayfasının mobil karşılığı — ancak
/// web'in aksine (orayı ayrıca `GET /api/orders/.../{id}` ile sorgular) hiç
/// ek istek atmaz; yalnızca checkout akışının zaten ürettiği bilgiyi gösterir.
class OrderConfirmationPage extends StatelessWidget {
  const OrderConfirmationPage({super.key, required this.args});
  final OrderConfirmationArgs args;

  static String _shortId(String id) =>
      id.isEmpty ? '' : '#${id.split('-').first.toUpperCase()}';

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final isGuest = args.guestCustomerId != null;
    return Scaffold(
      appBar: AppBar(title: const Text('Sipariş Onayı'), automaticallyImplyLeading: false),
      body: SafeArea(
        child: SingleChildScrollView(
          padding: const EdgeInsets.all(24),
          child: ConstrainedBox(
            constraints: const BoxConstraints(maxWidth: 420),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.stretch,
              children: [
                Center(
                  child: Column(
                    children: [
                      Container(
                        width: 56,
                        height: 56,
                        decoration: BoxDecoration(
                          shape: BoxShape.circle,
                          color: theme.colorScheme.primaryContainer,
                        ),
                        child: Icon(
                          Icons.check,
                          color: theme.colorScheme.onPrimaryContainer,
                          size: 32,
                        ),
                      ),
                      const SizedBox(height: 16),
                      Text(
                        'Siparişiniz başarıyla oluşturuldu',
                        style: theme.textTheme.titleLarge,
                        textAlign: TextAlign.center,
                      ),
                    ],
                  ),
                ),
                const SizedBox(height: 24),
                Card(
                  child: Padding(
                    padding: const EdgeInsets.all(16),
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        _InfoRow(label: 'Sipariş No', value: _shortId(args.orderId)),
                        if (isGuest) ...[
                          const SizedBox(height: 8),
                          _InfoRow(
                            label: 'Müşteri No',
                            value: _shortId(args.guestCustomerId!),
                          ),
                        ],
                        if (!isGuest && args.total > 0) ...[
                          const SizedBox(height: 8),
                          _InfoRow(
                            label: 'Toplam Tutar',
                            value: args.total.toTryCurrency(),
                          ),
                        ],
                      ],
                    ),
                  ),
                ),
                if (isGuest) ...[
                  const SizedBox(height: 16),
                  Text(
                    'Misafir siparişi olduğu için "Siparişlerim" listenizde '
                    'görünmeyecek. Sipariş No ve Müşteri No bilgilerini, '
                    'daha sonra "Sipariş Sorgula" ile tekrar '
                    'görüntüleyebilmek için saklayın.',
                    style: theme.textTheme.bodyMedium,
                  ),
                ],
                const SizedBox(height: 32),
                if (isGuest)
                  FilledButton(
                    onPressed: () =>
                        context.push(RoutePaths.guestOrderLookup),
                    child: const Text('Sipariş Sorgula'),
                  )
                else
                  FilledButton(
                    onPressed: () => context.go(RoutePaths.orders),
                    child: const Text('Siparişlerime Git'),
                  ),
                const SizedBox(height: 12),
                OutlinedButton(
                  onPressed: () => context.go(RoutePaths.home),
                  child: const Text('Alışverişe Devam Et'),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}

class _InfoRow extends StatelessWidget {
  const _InfoRow({required this.label, required this.value});
  final String label;
  final String value;

  @override
  Widget build(BuildContext context) => Row(
    mainAxisAlignment: MainAxisAlignment.spaceBetween,
    children: [
      Text(label, style: Theme.of(context).textTheme.bodyMedium),
      Text(
        value,
        style: Theme.of(context).textTheme.bodyMedium?.copyWith(
          fontWeight: FontWeight.w600,
        ),
      ),
    ],
  );
}
