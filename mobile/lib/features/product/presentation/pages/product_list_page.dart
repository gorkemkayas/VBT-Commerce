import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/constants/route_paths.dart';
import '../../../../core/widgets/async_state_views.dart';
import '../../../cart/presentation/widgets/cart_icon_button.dart';
import '../providers/product_providers.dart';
import '../widgets/product_card.dart';

class ProductListPage extends ConsumerStatefulWidget {
  const ProductListPage({super.key});
  @override
  ConsumerState<ProductListPage> createState() => _ProductListPageState();
}

class _ProductListPageState extends ConsumerState<ProductListPage> {
  @override
  void initState() {
    super.initState();
    Future.microtask(
      () => ref.read(productListControllerProvider.notifier).loadProducts(),
    );
  }

  @override
  Widget build(BuildContext context) {
    final state = ref.watch(productListControllerProvider);
    return Scaffold(
      appBar: AppBar(
        title: const Text('Ürünler'),
        actions: [
          IconButton(
            tooltip: 'Ara ve filtrele',
            onPressed: () => context.push(RoutePaths.search),
            icon: const Icon(Icons.search),
          ),
          const CartIconButton(),
        ],
      ),
      body: switch (state) {
        ProductListState(isLoading: true, products: []) => const LoadingView(
          message: 'Ürünler yükleniyor...',
        ),
        ProductListState(failure: final failure?) => ErrorView(
          message: failure.message,
          onRetry: () =>
              ref.read(productListControllerProvider.notifier).loadProducts(),
        ),
        ProductListState(products: []) => EmptyView(
          message: 'Gösterilecek ürün bulunamadı.',
          onRetry: () =>
              ref.read(productListControllerProvider.notifier).loadProducts(),
        ),
        ProductListState(products: final products) => RefreshIndicator(
          onRefresh: () =>
              ref.read(productListControllerProvider.notifier).loadProducts(),
          child: GridView.builder(
            padding: const EdgeInsets.all(16),
            itemCount: products.length,
            gridDelegate: const SliverGridDelegateWithFixedCrossAxisCount(
              crossAxisCount: 2,
              crossAxisSpacing: 12,
              mainAxisSpacing: 12,
              childAspectRatio: .58,
            ),
            itemBuilder: (context, index) => ProductCard(
              product: products[index],
              onTap: () => context.push('/product/${products[index].id}'),
            ),
          ),
        ),
      },
    );
  }
}
