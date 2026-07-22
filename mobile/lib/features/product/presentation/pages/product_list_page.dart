import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/constants/route_paths.dart';
import '../../../../core/widgets/async_state_views.dart';
import '../../../cart/presentation/widgets/cart_icon_button.dart';
import '../../domain/entities/category.dart';
import '../providers/product_providers.dart';
import '../widgets/category_chip.dart';
import '../widgets/product_card.dart';

/// "Koleksiyonu keşfet" ile açılan koleksiyon ekranı. Kategori seçimi ana
/// sayfada değil burada yapılır: çipler her zaman görünür kalır, altındaki
/// ızgara seçime göre filtrelenir.
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
    final controller = ref.read(productListControllerProvider.notifier);
    final categories = ref.watch(categoriesProvider).asData?.value ?? const [];

    return Scaffold(
      appBar: AppBar(
        title: const Text('Koleksiyon'),
        actions: [
          IconButton(
            tooltip: 'Ara ve filtrele',
            onPressed: () => context.push(RoutePaths.search),
            icon: const Icon(Icons.search),
          ),
          const CartIconButton(),
        ],
      ),
      body: Column(
        children: [
          if (categories.isNotEmpty)
            _CategoryList(
              categories: categories,
              selectedCategory: state.selectedCategory,
              onSelected: controller.selectCategory,
            ),
          Expanded(
            child: _ProductGrid(
              state: state,
              onRetry: controller.loadProducts,
              onProductTap: (id) => context.push('/product/$id'),
            ),
          ),
        ],
      ),
    );
  }
}

class _CategoryList extends StatelessWidget {
  const _CategoryList({
    required this.categories,
    required this.selectedCategory,
    required this.onSelected,
  });

  final List<Category> categories;
  final String? selectedCategory;
  final ValueChanged<String?> onSelected;

  @override
  Widget build(BuildContext context) => SizedBox(
    height: 52,
    child: ListView(
      scrollDirection: Axis.horizontal,
      padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 4),
      children: [
        CategoryChip(
          label: 'Tümü',
          selected: selectedCategory == null,
          onSelected: (_) => onSelected(null),
        ),
        for (final category in categories)
          CategoryChip(
            label: category.name,
            selected: category.id == selectedCategory,
            onSelected: (_) => onSelected(category.id),
          ),
      ],
    ),
  );
}

class _ProductGrid extends StatelessWidget {
  const _ProductGrid({
    required this.state,
    required this.onRetry,
    required this.onProductTap,
  });

  final ProductListState state;
  final Future<void> Function() onRetry;
  final ValueChanged<String> onProductTap;

  @override
  Widget build(BuildContext context) {
    if (state.isLoading && state.products.isEmpty) {
      return const LoadingView(message: 'Ürünler yükleniyor...');
    }
    if (state.failure != null && state.products.isEmpty) {
      return ErrorView(message: state.failure!.message, onRetry: onRetry);
    }
    if (state.products.isEmpty) {
      return EmptyView(
        message: state.selectedCategory == null
            ? 'Gösterilecek ürün bulunamadı.'
            : 'Bu kategoride ürün bulunamadı.',
        onRetry: onRetry,
      );
    }
    return RefreshIndicator(
      onRefresh: onRetry,
      child: GridView.builder(
        padding: const EdgeInsets.all(16),
        itemCount: state.products.length,
        gridDelegate: const SliverGridDelegateWithFixedCrossAxisCount(
          crossAxisCount: 2,
          crossAxisSpacing: 12,
          mainAxisSpacing: 12,
          childAspectRatio: .58,
        ),
        itemBuilder: (context, index) {
          final product = state.products[index];
          return ProductCard(
            product: product,
            onTap: () => onProductTap(product.id),
          );
        },
      ),
    );
  }
}
