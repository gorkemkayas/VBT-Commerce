import 'package:flutter/material.dart';

import '../../../../core/widgets/async_state_views.dart';

class FavoritesPage extends StatelessWidget {
  const FavoritesPage({super.key});

  @override
  Widget build(BuildContext context) => Scaffold(
    appBar: AppBar(title: const Text('Favoriler')),
    body: const EmptyView(message: 'Henüz favori ürününüz yok.'),
  );
}
