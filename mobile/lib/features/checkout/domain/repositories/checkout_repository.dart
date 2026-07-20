import '../../../../core/utils/result.dart';
import '../../../cart/domain/entities/cart_item.dart';
import '../entities/address.dart';
import '../entities/order.dart';

abstract interface class CheckoutRepository {
  Future<Result<Order>> completeOrder({
    required Address address,
    required List<CartItem> items,
  });
}
