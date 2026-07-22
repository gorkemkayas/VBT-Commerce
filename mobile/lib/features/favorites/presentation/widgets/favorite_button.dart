import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../domain/entities/favorite_item.dart';
import '../providers/favorites_providers.dart';

/// Bir ürünü favorilere ekleyip çıkaran kalp butonu. Favori durumunu global
/// [favoritesControllerProvider]'dan izler, böylece nerede gösterilirse
/// gösterilsin (ör. detay AppBar'ı) her zaman güncel kalır.
class FavoriteButton extends ConsumerWidget {
  const FavoriteButton({super.key, required this.item});

  final FavoriteItem item;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final isFavorite = ref.watch(
      favoritesControllerProvider.select(
        (state) => state.isFavorite(item.productId),
      ),
    );
    return IconButton(
      tooltip: isFavorite ? 'Favorilerden çıkar' : 'Favorilere ekle',
      icon: Icon(
        isFavorite ? Icons.favorite : Icons.favorite_border,
        color: isFavorite ? Colors.redAccent : null,
      ),
      onPressed: () =>
          ref.read(favoritesControllerProvider.notifier).toggle(item),
    );
  }
}
