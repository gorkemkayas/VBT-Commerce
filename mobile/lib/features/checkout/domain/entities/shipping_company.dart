/// `GET /api/shipping-companies` yalnızca aktif firmaları döner.
class ShippingCompany {
  const ShippingCompany({
    required this.id,
    required this.name,
    required this.fee,
  });

  final String id;
  final String name;
  final double fee;
}
