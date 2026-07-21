import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/constants/route_paths.dart';
import '../../../../core/widgets/async_state_views.dart';
import '../../../cart/presentation/widgets/cart_icon_button.dart';
import '../../../product/domain/entities/product.dart';
import '../../../product/presentation/widgets/product_card.dart';
import '../providers/home_providers.dart';
import '../widgets/home_category_chips.dart';
import '../widgets/home_hero_banner.dart';
import '../widgets/home_section_header.dart';

class HomePage extends ConsumerStatefulWidget {
  const HomePage({super.key});

  @override
  ConsumerState<HomePage> createState() => _HomePageState();
}

class _HomePageState extends ConsumerState<HomePage> {
  @override
  void initState() {
    super.initState();
    Future.microtask(() => ref.read(homeControllerProvider.notifier).load());
  }

  void _openProduct(Product product) =>
      context.push('/product/${product.id}');

  Future<void> _reload() =>
      ref.read(homeControllerProvider.notifier).load();

  @override
  Widget build(BuildContext context) {
    final state = ref.watch(homeControllerProvider);

    return Scaffold(
      appBar: AppBar(
        title: const Text('Sneaker Store'),
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
        HomeState(isLoading: true, isEmpty: true) => const LoadingView(
          message: 'Ana sayfa yükleniyor...',
        ),
        HomeState(failure: final failure?) => ErrorView(
          message: failure.message,
          onRetry: _reload,
        ),
        HomeState(isEmpty: true) => EmptyView(
          message: 'Gösterilecek ürün bulunamadı.',
          onRetry: _reload,
        ),
        _ => _HomeContent(
          state: state,
          onRefresh: _reload,
          onProductTap: _openProduct,
          onCategorySelected: (category) => ref
              .read(homeControllerProvider.notifier)
              .selectCategory(category),
          onSeeAllTap: () => context.push(RoutePaths.products),
        ),
      },
    );
  }
}

class _HomeContent extends StatelessWidget {
  const _HomeContent({
    required this.state,
    required this.onRefresh,
    required this.onProductTap,
    required this.onCategorySelected,
    required this.onSeeAllTap,
  });

  final HomeState state;
  final Future<void> Function() onRefresh;
  final ValueChanged<Product> onProductTap;
  final ValueChanged<String?> onCategorySelected;
  final VoidCallback onSeeAllTap;

  @override
  Widget build(BuildContext context) {
    return RefreshIndicator(
      onRefresh: onRefresh,
      child: CustomScrollView(
        slivers: [
          const SliverToBoxAdapter(child: SizedBox(height: 12)),
          SliverToBoxAdapter(
            child: HomeCategoryChips(
              categories: state.categories,
              selectedCategory: state.selectedCategory,
              onCategorySelected: onCategorySelected,
            ),
          ),
          const SliverToBoxAdapter(child: SizedBox(height: 16)),
          SliverToBoxAdapter(
            child: HomeHeroBanner(onActionTap: onSeeAllTap),
          ),

          if (state.featuredProducts.isNotEmpty) ...[
            SliverToBoxAdapter(
              child: HomeSectionHeader(
                title: 'Öne çıkanlar',
                actionLabel: 'Tümünü gör',
                onActionTap: onSeeAllTap,
              ),
            ),
            SliverToBoxAdapter(
              child: SizedBox(
                height: 280,
                child: ListView.separated(
                  scrollDirection: Axis.horizontal,
                  padding: const EdgeInsets.symmetric(horizontal: 16),
                  itemCount: state.featuredProducts.length,
                  separatorBuilder: (context, index) =>
                      const SizedBox(width: 12),
                  itemBuilder: (context, index) {
                    final product = state.featuredProducts[index];
                    return SizedBox(
                      width: 170,
                      child: ProductCard(
                        product: product,
                        onTap: () => onProductTap(product),
                      ),
                    );
                  },
                ),
              ),
            ),
          ],

          SliverToBoxAdapter(
            child: HomeSectionHeader(
              title: state.selectedCategory ?? 'Senin için önerilenler',
            ),
          ),

          if (state.isLoading)
            const SliverToBoxAdapter(
              child: Padding(
                padding: EdgeInsets.symmetric(vertical: 32),
                child: Center(child: CircularProgressIndicator()),
              ),
            )
          else if (state.recommendedProducts.isEmpty)
            const SliverToBoxAdapter(
              child: Padding(
                padding: EdgeInsets.symmetric(vertical: 32),
                child: Center(
                  child: Text('Bu kategoride ürün bulunamadı.'),
                ),
              ),
            )
          else
            SliverPadding(
              padding: const EdgeInsets.all(16),
              sliver: SliverGrid.builder(
                itemCount: state.recommendedProducts.length,
                gridDelegate:
                    const SliverGridDelegateWithFixedCrossAxisCount(
                      crossAxisCount: 2,
                      crossAxisSpacing: 12,
                      mainAxisSpacing: 12,
                      childAspectRatio: .58,
                    ),
                itemBuilder: (context, index) {
                  final product = state.recommendedProducts[index];
                  return ProductCard(
                    product: product,
                    onTap: () => onProductTap(product),
                  );
                },
              ),
            ),

          const SliverToBoxAdapter(child: SizedBox(height: 24)),
        ],
      ),
    );
  }
}
