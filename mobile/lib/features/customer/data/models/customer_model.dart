import '../../domain/entities/customer.dart';
import '../../domain/entities/customer_address.dart';
import 'customer_address_model.dart';

/// `GET /api/customers/me` yanıtındaki `CustomerDto` şekline karşılık gelir.
/// Sadece sunucudan gelen yanıtı parse etmek için kullanıldığından (tekrar
/// sunucuya gönderilmediğinden, karşılaştırılmadığından) Freezed yerine düz
/// bir sınıf olarak tanımlanır — `addresses` alanı `Customer` entity'sinden
/// miras alınan `List<CustomerAddress>` tipiyle birebir aynı olduğundan
/// Freezed'in otomatik JSON/equality kod üretimi bu alanda derleme hatası
/// veriyor.
class CustomerModel extends Customer {
  const CustomerModel({
    required super.id,
    required super.userId,
    super.phoneNumber,
    super.dateOfBirth,
    required super.addresses,
  });

  factory CustomerModel.fromJson(Map<String, dynamic> json) {
    return CustomerModel(
      id: json['id'] as String,
      userId: json['userId'] as String,
      phoneNumber: json['phoneNumber'] as String?,
      dateOfBirth: json['dateOfBirth'] as String?,
      addresses: _addressesFromJson(json['addresses']),
    );
  }
}

List<CustomerAddress> _addressesFromJson(Object? json) {
  if (json is! List) return const [];
  return json
      .map(
        (item) => CustomerAddressModel.fromJson(item as Map<String, dynamic>),
      )
      .toList(growable: false);
}
