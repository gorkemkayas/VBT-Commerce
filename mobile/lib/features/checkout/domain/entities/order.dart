import '../../../cart/domain/entities/cart_item.dart';

class Order {
  const Order({
    required this.orderId,
    required this.placedAt,
    required this.items,
    required this.total,
  });

  final String orderId;
  final DateTime placedAt;
  final List<CartItem> items;
  final double total;
}
