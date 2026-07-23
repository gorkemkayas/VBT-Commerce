import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/widgets/app_network_image.dart';
import '../../../../core/widgets/async_state_views.dart';
import '../providers/favorites_providers.dart';

class FavoritesPage extends ConsumerWidget {
  const FavoritesPage({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final state = ref.watch(favoritesControllerProvider);
    final Widget body;
    if (state.isLoading && state.items.isEmpty) {
      body = const LoadingView(message: 'Favoriler yükleniyor...');
    } else if (state.items.isEmpty) {
      body = const EmptyView(message: 'Henüz favori ürününüz yok.');
    } else {
      body = ListView.separated(
        padding: const EdgeInsets.symmetric(vertical: 8),
        itemCount: state.items.length,
        separatorBuilder: (context, index) => const Divider(height: 1),
        itemBuilder: (context, index) {
          final item = state.items[index];
          return ListTile(
            leading: SizedBox(
              width: 56,
              height: 56,
              child: AppNetworkImage(imageUrl: item.imageUrl),
            ),
            title: Text(
              item.title,
              maxLines: 2,
              overflow: TextOverflow.ellipsis,
            ),
            trailing: IconButton(
              icon: const Icon(Icons.favorite, color: Colors.redAccent),
              tooltip: 'Favorilerden çıkar',
              onPressed: () => ref
                  .read(favoritesControllerProvider.notifier)
                  .remove(item.productId),
            ),
            onTap: () => context.push('/product/${item.productId}'),
          );
        },
      );
    }

    return Scaffold(
      appBar: AppBar(title: const Text('Favoriler')),
      body: body,
    );
  }
}
