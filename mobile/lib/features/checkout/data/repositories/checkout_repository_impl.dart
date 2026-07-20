import 'dart:math';

import '../../../../core/utils/result.dart';
import '../../../cart/domain/entities/cart_item.dart';
import '../../domain/entities/address.dart';
import '../../domain/entities/order.dart';
import '../../domain/repositories/checkout_repository.dart';

/// Backend hazır olana kadar siparişi bellekte simüle eder.
class CheckoutRepositoryImpl implements CheckoutRepository {
  CheckoutRepositoryImpl({Random? random}) : _random = random ?? Random();
  final Random _random;

  @override
  Future<Result<Order>> completeOrder({
    required Address address,
    required List<CartItem> items,
  }) async {
    await Future.delayed(const Duration(seconds: 1));
    final total = items.fold(0.0, (total, item) => total + item.lineTotal);
    final order = Order(
      orderId: 'MOCK-${_random.nextInt(900000) + 100000}',
      placedAt: DateTime.now(),
      address: address,
      items: items,
      total: total,
    );
    return Result.success(order);
  }
}
