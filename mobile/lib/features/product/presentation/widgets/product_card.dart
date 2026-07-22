import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/utils/currency_formatter.dart';
import '../../../../core/utils/result.dart';
import '../../../../core/widgets/app_network_image.dart';
import '../../domain/entities/product.dart';
import '../providers/product_providers.dart';

class ProductCard extends StatelessWidget {
  const ProductCard({super.key, required this.product, required this.onTap});
  final Product product;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) => Card(
    clipBehavior: Clip.antiAlias,
    child: InkWell(
      onTap: onTap,
      child: Padding(
        padding: const EdgeInsets.all(12),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Expanded(
              child: SizedBox(
                width: double.infinity,
                child: AppNetworkImage(imageUrl: product.imageUrl),
              ),
            ),
            const SizedBox(height: 12),
            Text(
              product.title,
              maxLines: 2,
              overflow: TextOverflow.ellipsis,
              style: Theme.of(context).textTheme.titleSmall,
            ),
            const SizedBox(height: 4),
            Text(
              product.category,
              maxLines: 1,
              overflow: TextOverflow.ellipsis,
            ),
            const SizedBox(height: 8),
            _CardPriceText(product: product),
          ],
        ),
      ),
    ),
  );
}

/// Varyantsız ürünlerde repository'nin zaten doldurduğu `product.price`
/// gösterilir (ek çağrı yok). Varyantlı ürünlerde ise liste yanıtında
/// varyant bilgisi gelmediği için (bkz. `Product.variants`), Detay
/// sayfasının kullandığı AYNI `productDetailProvider` izlenerek ilk
/// varyantın gerçek fiyatı çözülür — Riverpod bu family'yi ürün id'sine göre
/// cache'lediğinden aynı ürün için tekrar tekrar istek atılmaz ve kullanıcı
/// Detay'a geçtiğinde de aynı sonuç hazır olur.
class _CardPriceText extends ConsumerWidget {
  const _CardPriceText({required this.product});
  final Product product;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final style = Theme.of(context).textTheme.titleMedium;
    if (!product.hasVariants) {
      return Text(_label(product.price), style: style);
    }
    final detail = ref.watch(productDetailProvider(product.id));
    return detail.when(
      data: (result) => Text(
        _label(switch (result) {
          Success<Product>(:final value) => value.price,
          ResultFailure<Product>() => null,
        }),
        style: style,
      ),
      loading: () => const SizedBox(
        height: 16,
        width: 16,
        child: CircularProgressIndicator(strokeWidth: 2),
      ),
      error: (_, _) => Text(_label(null), style: style),
    );
  }

  String _label(double? price) =>
      price != null ? price.toTryCurrency() : 'Fiyat yakında';
}
