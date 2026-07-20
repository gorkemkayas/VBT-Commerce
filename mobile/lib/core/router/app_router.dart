import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../features/auth/presentation/pages/login_page.dart';
import '../../features/auth/presentation/pages/register_page.dart';
import '../../features/cart/presentation/pages/cart_page.dart';
import '../../features/checkout/presentation/pages/checkout_page.dart';
import '../../features/customer/presentation/pages/addresses_page.dart';
import '../../features/customer/presentation/pages/profile_page.dart';
import '../../features/orders/presentation/pages/orders_page.dart';
import '../../features/product/presentation/pages/product_detail_page.dart';
import '../../features/product/presentation/pages/product_list_page.dart';
import '../../features/product/presentation/pages/search_page.dart';
import '../constants/route_paths.dart';
import '../navigation/main_shell_page.dart';
import 'navigation_service.dart';

final appRouterProvider = Provider<GoRouter>(
  (ref) => GoRouter(
    navigatorKey: rootNavigatorKey,
    initialLocation: RoutePaths.home,
    routes: [
      GoRoute(
        path: RoutePaths.login,
        builder: (context, state) => const LoginPage(),
      ),
      GoRoute(
        path: RoutePaths.register,
        builder: (context, state) => const RegisterPage(),
      ),
      GoRoute(
        path: RoutePaths.home,
        builder: (context, state) => const MainShellPage(),
      ),
      GoRoute(
        path: RoutePaths.products,
        builder: (context, state) => const ProductListPage(),
      ),
      GoRoute(
        path: RoutePaths.search,
        builder: (context, state) => const SearchPage(),
      ),
      GoRoute(
        path: RoutePaths.cart,
        builder: (context, state) => const CartPage(),
      ),
      GoRoute(
        path: RoutePaths.checkout,
        builder: (context, state) => const CheckoutPage(),
      ),
      GoRoute(
        path: RoutePaths.profile,
        builder: (context, state) => const ProfilePage(),
      ),
      GoRoute(
        path: RoutePaths.addresses,
        builder: (context, state) => const AddressesPage(),
      ),
      GoRoute(
        path: RoutePaths.orders,
        builder: (context, state) => const OrdersPage(),
      ),
      GoRoute(
        path: RoutePaths.productDetail,
        builder: (context, state) {
          final id = state.pathParameters['id'];
          if (id == null || id.isEmpty) {
            return const ProductDetailPage(productId: '');
          }
          return ProductDetailPage(productId: id);
        },
      ),
    ],
  ),
);
