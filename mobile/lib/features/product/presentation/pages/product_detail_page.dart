import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/utils/currency_formatter.dart';
import '../../../../core/utils/result.dart';
import '../../../../core/widgets/app_network_image.dart';
import '../../../../core/widgets/async_state_views.dart';
import '../../../cart/presentation/providers/cart_providers.dart';
import '../../../cart/presentation/widgets/cart_icon_button.dart';
import '../../../favorites/domain/entities/favorite_item.dart';
import '../../../favorites/presentation/widgets/favorite_button.dart';
import '../../domain/entities/product.dart';
import '../../domain/entities/product_variant.dart';
import '../providers/product_providers.dart';

class ProductDetailPage extends ConsumerWidget {
  const ProductDetailPage({super.key, required this.productId});
  final String productId;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final product = ref.watch(productDetailProvider(productId));
    return Scaffold(
      appBar: AppBar(
        title: const Text('Ürün detayı'),
        actions: const [CartIconButton()],
      ),
      body: product.when(
        loading: () => const LoadingView(message: 'Ürün detayı yükleniyor...'),
        error: (_, _) => ErrorView(
          message: 'Ürün detayı yüklenemedi.',
          onRetry: () => ref.invalidate(productDetailProvider(productId)),
        ),
        data: (result) => switch (result) {
          Success<Product>(:final value) => _ProductDetail(product: value),
          ResultFailure<Product>(:final failure) => ErrorView(
            message: failure.message,
            onRetry: () => ref.invalidate(productDetailProvider(productId)),
          ),
        },
      ),
    );
  }
}

class _ProductDetail extends ConsumerStatefulWidget {
  const _ProductDetail({required this.product});
  final Product product;

  @override
  ConsumerState<_ProductDetail> createState() => _ProductDetailState();
}

class _ProductDetailState extends ConsumerState<_ProductDetail> {
  bool _isAddingToCart = false;
  String? _selectedVariantId;

  @override
  void initState() {
    super.initState();
    if (widget.product.variants.length == 1) {
      _selectedVariantId = widget.product.variants.first.id;
    }
  }

  bool get _hasVariants => widget.product.variants.isNotEmpty;

  /// Varyantlar yalnızca gerçekten "Beden" özniteliğine sahipse (bkz.
  /// `ProductDetailModel._variantsFromJson` — yalnızca `attributeName ==
  /// 'Beden'` olan seçenek `size`'a yazılır) beden seçimi anlamlıdır. Kıyafet
  /// olmayan varyantlı ürünlerde (ör. renk seçenekli ürünler) `size` boş
  /// string olarak gelir — bu durumda "Beden" bölümü hiç gösterilmez.
  bool get _hasSizeVariants =>
      widget.product.variants.any((variant) => variant.size.isNotEmpty);

  bool get _canAddToCart => !_hasVariants || _selectedVariantId != null;

  /// `product.price`, repository tarafından bu varyant üzerinden zaten
  /// doldurulmuştur (bkz. `ProductRepositoryImpl._priceReferenceFor`).
  String? get _defaultVariantId =>
      _hasVariants ? widget.product.variants.first.id : null;

  Future<void> _addToCart() async {
    final hasVariants = _hasVariants;
    setState(() => _isAddingToCart = true);
    await ref
        .read(cartControllerProvider.notifier)
        .add(
          sellableItemId: hasVariants ? _selectedVariantId! : widget.product.id,
          isVariant: hasVariants,
          title: widget.product.title,
          imageUrl: widget.product.imageUrl,
        );
    if (!mounted) return;
    setState(() => _isAddingToCart = false);
    final failure = ref.read(cartControllerProvider).failure;
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(content: Text(failure?.message ?? 'Ürün sepete eklendi.')),
    );
  }

  @override
  Widget build(BuildContext context) {
    final product = widget.product;
    return SingleChildScrollView(
      padding: const EdgeInsets.all(24),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          SizedBox(
            height: 300,
            child: AppNetworkImage(imageUrl: product.imageUrl),
          ),
          const SizedBox(height: 24),
          Row(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Expanded(
                child: Text(
                  product.title,
                  style: Theme.of(context).textTheme.headlineSmall,
                ),
              ),
              FavoriteButton(
                item: FavoriteItem(
                  productId: product.id,
                  title: product.title,
                  imageUrl: product.imageUrl,
                ),
              ),
            ],
          ),
          const SizedBox(height: 12),
          _PriceText(
            defaultPrice: product.price,
            selectedVariantId: _selectedVariantId,
            defaultVariantId: _defaultVariantId,
          ),
          const SizedBox(height: 20),
          Text(
            product.description,
            style: Theme.of(context).textTheme.bodyLarge,
          ),
          if (_hasSizeVariants) ...[
            const SizedBox(height: 20),
            Text('Beden', style: Theme.of(context).textTheme.titleSmall),
            const SizedBox(height: 8),
            Wrap(
              spacing: 8,
              runSpacing: 8,
              children: [
                for (final variant in _sortedBySizeOrder(product.variants))
                  _SizeBox(
                    label: variant.size,
                    selected: _selectedVariantId == variant.id,
                    onTap: () =>
                        setState(() => _selectedVariantId = variant.id),
                  ),
              ],
            ),
          ],
          const SizedBox(height: 32),
          FilledButton.icon(
            onPressed: (_isAddingToCart || !_canAddToCart) ? null : _addToCart,
            icon: _isAddingToCart
                ? const SizedBox(
                    height: 18,
                    width: 18,
                    child: CircularProgressIndicator(strokeWidth: 2),
                  )
                : const Icon(Icons.add_shopping_cart),
            label: const Text('Sepete Ekle'),
          ),
        ],
      ),
    );
  }
}

/// Beden seçeneklerini mantıksal sırada gösterir; tanınmayan bir etiket
/// gelirse (beklenmedik bir değer) listenin sonuna eklenir, mevcut sırası
/// korunur.
const _sizeOrder = ['XS', 'S', 'M', 'L', 'XL', 'XXL'];

List<ProductVariant> _sortedBySizeOrder(List<ProductVariant> variants) {
  final sorted = [...variants];
  sorted.sort((first, second) {
    final firstIndex = _sizeOrder.indexOf(first.size);
    final secondIndex = _sizeOrder.indexOf(second.size);
    return (firstIndex == -1 ? _sizeOrder.length : firstIndex).compareTo(
      secondIndex == -1 ? _sizeOrder.length : secondIndex,
    );
  });
  return sorted;
}

class _SizeBox extends StatelessWidget {
  const _SizeBox({
    required this.label,
    required this.selected,
    required this.onTap,
  });

  final String label;
  final bool selected;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return InkWell(
      onTap: onTap,
      borderRadius: BorderRadius.circular(4),
      child: Container(
        width: 48,
        height: 48,
        alignment: Alignment.center,
        decoration: BoxDecoration(
          color: selected ? theme.colorScheme.primaryContainer : null,
          border: Border.all(
            color: selected
                ? theme.colorScheme.primary
                : theme.colorScheme.outline,
            width: selected ? 2 : 1,
          ),
          borderRadius: BorderRadius.circular(4),
        ),
        child: Text(
          label,
          style: TextStyle(
            fontWeight: selected ? FontWeight.bold : FontWeight.normal,
            color: selected
                ? theme.colorScheme.primary
                : theme.colorScheme.onSurface,
          ),
        ),
      ),
    );
  }
}

class _PriceText extends ConsumerWidget {
  const _PriceText({
    required this.defaultPrice,
    required this.selectedVariantId,
    required this.defaultVariantId,
  });

  /// Repository tarafından varsayılan (ilk) varyant/ürün üzerinden
  /// doldurulan fiyat.
  final double? defaultPrice;
  final String? selectedVariantId;
  final String? defaultVariantId;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final style = Theme.of(context).textTheme.headlineMedium;
    final variantId = selectedVariantId;
    if (variantId == null || variantId == defaultVariantId) {
      return Text(_label(defaultPrice), style: style);
    }
    final variantPrice = ref.watch(variantPriceProvider(variantId));
    return variantPrice.when(
      data: (result) => Text(
        _label(switch (result) {
          Success<double?>(:final value) => value,
          ResultFailure<double?>() => null,
        }),
        style: style,
      ),
      loading: () => Text(_label(defaultPrice), style: style),
      error: (_, _) => Text(_label(null), style: style),
    );
  }

  String _label(double? price) =>
      price != null ? price.toTryCurrency() : 'Fiyat bilgisi yakında eklenecek';
}
