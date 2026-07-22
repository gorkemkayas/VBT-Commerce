/// `OrderDto`'nun teslimat adresi alanları — sipariş verildiği andaki
/// anlık görüntüdür (müşterinin daha sonra adresi düzenlemesinden etkilenmez).
class OrderAddress {
  const OrderAddress({
    required this.recipientName,
    required this.phoneNumber,
    required this.country,
    required this.city,
    required this.district,
    required this.postalCode,
    required this.addressLine1,
    this.addressLine2,
  });

  final String recipientName;
  final String phoneNumber;
  final String country;
  final String city;
  final String district;
  final String postalCode;
  final String addressLine1;
  final String? addressLine2;
}
