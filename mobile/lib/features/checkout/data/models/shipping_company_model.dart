import '../../domain/entities/shipping_company.dart';

/// Backend'in `GET /api/shipping-companies` yanıtındaki `ShippingCompanyDto`
/// şekline karşılık gelir (`id, name, fee, isActive`) — `isActive` burada
/// okunmuyor çünkü endpoint zaten yalnızca aktif firmaları döndürüyor.
class ShippingCompanyModel extends ShippingCompany {
  const ShippingCompanyModel({
    required super.id,
    required super.name,
    required super.fee,
  });

  factory ShippingCompanyModel.fromJson(Map<String, dynamic> json) =>
      ShippingCompanyModel(
        id: json['id'] as String,
        name: json['name'] as String,
        fee: (json['fee'] as num).toDouble(),
      );
}
