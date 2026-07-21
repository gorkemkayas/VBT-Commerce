import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../features/account/presentation/pages/account_page.dart';
import '../../features/cart/presentation/pages/cart_page.dart';
import '../../features/cart/presentation/providers/cart_providers.dart';
import '../../features/favorites/presentation/pages/favorites_page.dart';
import '../../features/home/presentation/pages/home_page.dart';

class MainShellPage extends ConsumerStatefulWidget {
  const MainShellPage({super.key});

  @override
  ConsumerState<MainShellPage> createState() => _MainShellPageState();
}

class _MainShellPageState extends ConsumerState<MainShellPage> {
  int _index = 0;

  static const _pages = [
    HomePage(),
    FavoritesPage(),
    CartPage(),
    AccountPage(),
  ];

  @override
  Widget build(BuildContext context) {
    final cartItemCount = ref.watch(
      cartControllerProvider.select((state) => state.itemCount),
    );
    return Scaffold(
      body: IndexedStack(index: _index, children: _pages),
      bottomNavigationBar: NavigationBar(
        selectedIndex: _index,
        onDestinationSelected: (index) => setState(() => _index = index),
        destinations: [
          const NavigationDestination(
            icon: Icon(Icons.home_outlined),
            selectedIcon: Icon(Icons.home),
            label: 'Ana Sayfa',
          ),
          const NavigationDestination(
            icon: Icon(Icons.favorite_border),
            selectedIcon: Icon(Icons.favorite),
            label: 'Favoriler',
          ),
          NavigationDestination(
            icon: Badge(
              isLabelVisible: cartItemCount > 0,
              label: Text('$cartItemCount'),
              child: const Icon(Icons.shopping_cart_outlined),
            ),
            selectedIcon: Badge(
              isLabelVisible: cartItemCount > 0,
              label: Text('$cartItemCount'),
              child: const Icon(Icons.shopping_cart),
            ),
            label: 'Sepet',
          ),
          const NavigationDestination(
            icon: Icon(Icons.person_outline),
            selectedIcon: Icon(Icons.person),
            label: 'Hesabım',
          ),
        ],
      ),
    );
  }
}
