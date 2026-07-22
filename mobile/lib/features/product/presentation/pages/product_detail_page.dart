import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/utils/result.dart';
import '../../../../core/widgets/app_network_image.dart';
import '../../../../core/widgets/async_state_views.dart';
import '../../../cart/presentation/providers/cart_providers.dart';
import '../../../cart/presentation/widgets/cart_icon_button.dart';
import '../../../favorites/domain/entities/favorite_item.dart';
import '../../../favorites/presentation/widgets/favorite_button.dart';
import '../../domain/entities/category.dart';
import '../../domain/entities/product.dart';
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

  bool get _canAddToCart => !_hasVariants || _selectedVariantId != null;

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
    // Ürün detayı yanıtı yalnızca `categoryId` taşır; adı kategori listesinden
    // çözeriz. Çözülemezse (liste yüklenmemiş ya da eşleşme yok) çip yerine
    // hiçbir şey göstermeyiz — kullanıcıya asla ham GUID gösterilmez.
    final categoryName = _resolveCategoryName(
      ref.watch(categoriesProvider).asData?.value,
      product.category,
    );
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
          if (categoryName != null) ...[
            Align(
              alignment: Alignment.centerLeft,
              child: Chip(label: Text(categoryName)),
            ),
            const SizedBox(height: 12),
          ],
          Text(
            product.price != null
                ? '\$${product.price!.toStringAsFixed(2)}'
                : 'Fiyat bilgisi yakında eklenecek',
            style: Theme.of(context).textTheme.headlineMedium,
          ),
          const SizedBox(height: 20),
          Text(
            product.description,
            style: Theme.of(context).textTheme.bodyLarge,
          ),
          if (_hasVariants) ...[
            const SizedBox(height: 20),
            Text('Beden', style: Theme.of(context).textTheme.titleSmall),
            const SizedBox(height: 8),
            Wrap(
              spacing: 8,
              runSpacing: 8,
              children: [
                for (final variant in product.variants)
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

/// Kategori id'sini görünen ada çevirir; liste henüz yüklenmemişse ya da
/// eşleşme yoksa `null` döner (bu durumda çip gizlenir).
String? _resolveCategoryName(List<Category>? categories, String categoryId) {
  if (categories == null) return null;
  for (final category in categories) {
    if (category.id == categoryId) return category.name;
  }
  return null;
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
