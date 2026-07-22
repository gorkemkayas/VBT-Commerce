import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/widgets/app_network_image.dart';
import '../../../../core/widgets/async_state_views.dart';
import '../../../cart/presentation/widgets/cart_icon_button.dart';
import '../../domain/entities/product.dart';
import '../providers/filter_state.dart';
import '../providers/product_providers.dart';
import '../widgets/category_chip.dart';
import '../widgets/product_search_bar.dart';
import '../widgets/sort_bottom_sheet.dart';

class SearchPage extends ConsumerStatefulWidget {
  const SearchPage({super.key});
  @override
  ConsumerState<SearchPage> createState() => _SearchPageState();
}

class _SearchPageState extends ConsumerState<SearchPage> {
  @override
  Widget build(BuildContext context) {
    final state = ref.watch(searchFilterControllerProvider);
    final controller = ref.read(searchFilterControllerProvider.notifier);
    return Scaffold(
      appBar: AppBar(
        title: const Text('Ara ve Filtrele'),
        actions: const [CartIconButton()],
      ),
      body: Column(
        children: [
          ProductSearchBar(onChanged: controller.setQuery),
          _CategoryList(state: state, onSelected: controller.setCategory),
          _SortControl(
            onSort: () => SortBottomSheet.show(
              context,
              selected: state.sort,
              onSelected: controller.setSort,
            ),
          ),
          Padding(
            padding: const EdgeInsets.fromLTRB(16, 4, 16, 8),
            child: Row(
              children: [
                Text(
                  '${state.products.length} sonuç',
                  style: Theme.of(context).textTheme.titleSmall,
                ),
                if (state.isLoading) ...[
                  const SizedBox(width: 12),
                  const SizedBox(
                    height: 14,
                    width: 14,
                    child: CircularProgressIndicator(strokeWidth: 2),
                  ),
                ],
              ],
            ),
          ),
          Expanded(
            child: _SearchResults(state: state, onRetry: controller.clear),
          ),
        ],
      ),
    );
  }
}

class _CategoryList extends StatelessWidget {
  const _CategoryList({required this.state, required this.onSelected});
  final FilterState state;
  final ValueChanged<String?> onSelected;

  @override
  Widget build(BuildContext context) => SizedBox(
    height: 44,
    child: ListView(
      scrollDirection: Axis.horizontal,
      padding: const EdgeInsets.symmetric(horizontal: 16),
      children: [
        CategoryChip(
          label: 'Tümü',
          selected: state.category == null,
          onSelected: (_) => onSelected(null),
        ),
        for (final category in state.categories)
          CategoryChip(
            label: category.name,
            selected: category.id == state.category,
            onSelected: (_) => onSelected(category.id),
          ),
      ],
    ),
  );
}

class _SortControl extends StatelessWidget {
  const _SortControl({required this.onSort});
  final VoidCallback onSort;

  @override
  Widget build(BuildContext context) => Padding(
    padding: const EdgeInsets.fromLTRB(16, 8, 16, 0),
    child: Row(
      mainAxisAlignment: MainAxisAlignment.end,
      children: [
        TextButton.icon(
          onPressed: onSort,
          icon: const Icon(Icons.sort),
          label: const Text('Sırala'),
        ),
      ],
    ),
  );
}

class _SearchResults extends StatelessWidget {
  const _SearchResults({required this.state, required this.onRetry});
  final FilterState state;
  final VoidCallback onRetry;

  @override
  Widget build(BuildContext context) {
    if (state.isLoading && state.products.isEmpty) {
      return const LoadingView(message: 'Ürünler aranıyor...');
    }
    if (state.failure != null && state.products.isEmpty) {
      return ErrorView(message: state.failure!.message, onRetry: onRetry);
    }
    if (state.products.isEmpty) {
      return const EmptyView(message: 'Filtrelerinize uygun ürün bulunamadı.');
    }
    return ListView.builder(
      key: const PageStorageKey('search-results'),
      itemCount: state.products.length,
      itemExtent: 108,
      itemBuilder: (context, index) => RepaintBoundary(
        child: _SearchResultTile(product: state.products[index]),
      ),
    );
  }
}

class _SearchResultTile extends StatelessWidget {
  const _SearchResultTile({required this.product});
  final Product product;

  @override
  Widget build(BuildContext context) => ListTile(
    onTap: () => context.push('/product/${product.id}'),
    leading: SizedBox(
      height: 72,
      width: 56,
      child: AppNetworkImage(imageUrl: product.imageUrl),
    ),
    title: Text(product.title, maxLines: 2, overflow: TextOverflow.ellipsis),
    subtitle: Text(product.category),
    trailing: Text(
      product.price != null
          ? '\$${product.price!.toStringAsFixed(2)}'
          : 'Fiyat yakında',
    ),
  );
}
