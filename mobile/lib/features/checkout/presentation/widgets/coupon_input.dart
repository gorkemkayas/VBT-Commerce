import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/utils/currency_formatter.dart';
import '../../domain/entities/coupon.dart';
import '../providers/checkout_providers.dart';

/// Kupon kodu girişi: kullanıcı bir kod yazıp "Uygula"ya basar. Kod backend'de
/// doğrulanır (bkz. `CheckoutController.applyCoupon`); geçerliyse aşağıda
/// kaldırılabilir bir çip olarak listelenir ve indirim ödeme özetine yansır,
/// geçersizse alanın altında hata mesajı gösterilir.
///
/// Alan ve buton bilerek dikey (Row/Expanded'sız) yerleştirilir: Flutter'ın
/// yeni semantics motorunda (3.4x) `Row` içindeki `Expanded(TextField)`,
/// `parentDataDirty` doğrulamasını tetikleyip checkout'ta beyaz ekrana yol
/// açıyordu. Uygulamanın çalışan diğer alanları da (arama çubuğu, misafir
/// formu) düz `TextField`'dır.
class CouponInput extends ConsumerStatefulWidget {
  const CouponInput({super.key});

  @override
  ConsumerState<CouponInput> createState() => _CouponInputState();
}

class _CouponInputState extends ConsumerState<CouponInput> {
  final _controller = TextEditingController();

  @override
  void dispose() {
    _controller.dispose();
    super.dispose();
  }

  Future<void> _apply() async {
    final code = _controller.text.trim();
    if (code.isEmpty) return;
    await ref.read(checkoutControllerProvider.notifier).applyCoupon(code);
    if (!mounted) return;
    // Kod başarıyla eklendiyse alanı temizle; hata varsa kullanıcı düzeltsin
    // diye metni bırak.
    final applied = ref
        .read(checkoutControllerProvider)
        .couponCodes
        .any((c) => c.toLowerCase() == code.toLowerCase());
    if (applied) _controller.clear();
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final couponCodes = ref.watch(
      checkoutControllerProvider.select((state) => state.couponCodes),
    );
    final isApplying = ref.watch(
      checkoutControllerProvider.select((state) => state.isApplyingCoupon),
    );
    final couponError = ref.watch(
      checkoutControllerProvider.select((state) => state.couponError),
    );

    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            Text('Kupon Kodu', style: theme.textTheme.titleMedium),
            const SizedBox(height: 12),
            TextField(
              controller: _controller,
              textInputAction: TextInputAction.done,
              textCapitalization: TextCapitalization.characters,
              enabled: !isApplying,
              decoration: InputDecoration(
                labelText: 'Kupon kodu girin',
                errorText: couponError,
                border: const OutlineInputBorder(),
              ),
              onSubmitted: (_) => _apply(),
            ),
            const SizedBox(height: 12),
            FilledButton(
              onPressed: isApplying ? null : _apply,
              child: isApplying
                  ? const SizedBox(
                      height: 18,
                      width: 18,
                      child: CircularProgressIndicator(strokeWidth: 2),
                    )
                  : const Text('Uygula'),
            ),
            if (couponCodes.isNotEmpty) ...[
              const SizedBox(height: 12),
              Wrap(
                spacing: 8,
                runSpacing: 4,
                children: [
                  for (final code in couponCodes)
                    InputChip(
                      label: Text(code),
                      onDeleted: () => ref
                          .read(checkoutControllerProvider.notifier)
                          .removeCoupon(code),
                    ),
                ],
              ),
            ],
            _AvailableCoupons(
              appliedCodes: couponCodes,
              isApplying: isApplying,
              onSelected: (code) => ref
                  .read(checkoutControllerProvider.notifier)
                  .applyCoupon(code),
            ),
          ],
        ),
      ),
    );
  }
}

/// `GET /api/coupons/active` ile gelen, herkese açık kuponları öneri olarak
/// listeler — kullanıcının kodu ezbere bilmesi gerekmesin diye. Dokunulan
/// kupon doğrudan uygulanır; geçerlilik kontrolünü (min. sepet tutarı,
/// kullanım limiti vb.) yine backend yapar ve hata mesajı alanın altında
/// görünür (bkz. `CheckoutController.applyCoupon`).
///
/// Zaten uygulanmış kodlar listeden çıkarılır. Aktif kupon yoksa ya da uç
/// hata verirse bölüm hiç görünmez.
class _AvailableCoupons extends ConsumerWidget {
  const _AvailableCoupons({
    required this.appliedCodes,
    required this.isApplying,
    required this.onSelected,
  });

  final List<String> appliedCodes;
  final bool isApplying;
  final ValueChanged<String> onSelected;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final coupons = ref.watch(activeCouponsProvider).value ?? const <Coupon>[];
    final suggestions = coupons
        .where(
          (coupon) => !appliedCodes.any(
            (applied) => applied.toLowerCase() == coupon.code.toLowerCase(),
          ),
        )
        .toList(growable: false);
    if (suggestions.isEmpty) return const SizedBox.shrink();

    final theme = Theme.of(context);
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        const SizedBox(height: 16),
        Text(
          'Uygulanabilir kuponlar',
          style: theme.textTheme.labelMedium?.copyWith(
            color: theme.colorScheme.onSurfaceVariant,
          ),
        ),
        const SizedBox(height: 8),
        Wrap(
          spacing: 8,
          runSpacing: 4,
          children: [
            for (final coupon in suggestions)
              ActionChip(
                label: Text(_chipLabel(coupon)),
                onPressed: isApplying ? null : () => onSelected(coupon.code),
              ),
          ],
        ),
      ],
    );
  }

  /// "KOD · %10" / "KOD · 50,00 ₺". İndirim tipi tanınmıyorsa yalnızca kod
  /// gösterilir — yanlış bir indirim vaadi vermektense sessiz kalmak yeğdir.
  String _chipLabel(Coupon coupon) => switch (coupon.discountType) {
    CouponDiscountType.percentage =>
      '${coupon.code} · %${_trimZeros(coupon.discountValue)}',
    CouponDiscountType.fixedAmount =>
      '${coupon.code} · ${coupon.discountValue.toTryCurrency()}',
    CouponDiscountType.unknown => coupon.code,
  };

  /// Yüzde değerleri tam sayıysa küsuratsız ("%10"), değilse virgüllü
  /// ("%12,5") gösterilir — para biçimlendirmesiyle aynı ayraç.
  static String _trimZeros(double value) => value == value.roundToDouble()
      ? value.toStringAsFixed(0)
      : value.toString().replaceAll('.', ',');
}
