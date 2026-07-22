import '../../../../core/utils/result.dart';
import '../../../cart/domain/entities/cart_item.dart';
import '../entities/order.dart';

abstract interface class CheckoutRepository {
  Future<Result<Order>> completeOrder({
    required String addressId,
    required List<CartItem> items,
  });
}
