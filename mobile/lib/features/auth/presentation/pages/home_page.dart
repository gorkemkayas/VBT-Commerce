import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import '../../../../core/constants/route_paths.dart';
import '../../../cart/presentation/widgets/cart_icon_button.dart';

class HomePage extends StatelessWidget {
  const HomePage({super.key});
  @override
  Widget build(BuildContext context) => Scaffold(
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
    body: Center(
      child: Column(
        mainAxisSize: MainAxisSize.min,
        children: [
          FilledButton(
            onPressed: () => context.go(RoutePaths.products),
            child: const Text('Ürünleri görüntüle'),
          ),
          const SizedBox(height: 12),
          FilledButton.tonal(
            onPressed: () => context.go(RoutePaths.login),
            child: const Text('Çıkış yap'),
          ),
        ],
      ),
    ),
  );
}
