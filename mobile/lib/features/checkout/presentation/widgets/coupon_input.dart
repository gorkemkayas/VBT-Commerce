import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

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
          ],
        ),
      ),
    );
  }
}
