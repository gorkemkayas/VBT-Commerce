import '../../../cart/domain/entities/cart_item.dart';
import 'address.dart';

class Order {
  const Order({
    required this.orderId,
    required this.placedAt,
    required this.address,
    required this.items,
    required this.total,
  });

  final String orderId;
  final DateTime placedAt;
  final Address address;
  final List<CartItem> items;
  final double total;
}
