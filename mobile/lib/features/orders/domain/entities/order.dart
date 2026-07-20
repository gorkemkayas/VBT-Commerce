class Order {
  const Order({
    required this.id,
    required this.status,
    required this.createdAt,
    required this.totalAmount,
  });

  final String id;
  final String status;
  final DateTime createdAt;
  final double totalAmount;
}
