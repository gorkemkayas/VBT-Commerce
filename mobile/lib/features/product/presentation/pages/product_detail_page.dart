import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/utils/result.dart';
import '../../../../core/widgets/app_network_image.dart';
import '../../../../core/widgets/async_state_views.dart';
import '../../../cart/presentation/providers/cart_providers.dart';
import '../../../cart/presentation/widgets/cart_icon_button.dart';
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

  Future<void> _addToCart() async {
    final variantId = widget.product.variantId;
    if (variantId == null) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Bu ürün şu anda satın alınamıyor.')),
      );
      return;
    }
    setState(() => _isAddingToCart = true);
    await ref
        .read(cartControllerProvider.notifier)
        .add(
          sellableItemId: variantId,
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
          Text(product.title, style: Theme.of(context).textTheme.headlineSmall),
          const SizedBox(height: 12),
          Chip(label: Text(product.category)),
          const SizedBox(height: 12),
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
          const SizedBox(height: 32),
          FilledButton.icon(
            onPressed: _isAddingToCart ? null : _addToCart,
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
