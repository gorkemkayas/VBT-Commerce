import 'package:freezed_annotation/freezed_annotation.dart';

import '../../domain/entities/customer_address.dart';

part 'customer_address_model.freezed.dart';
part 'customer_address_model.g.dart';

/// `CustomerAddressDto` şekline karşılık gelir (`CustomerDto.addresses` içinde
/// gelir).
@freezed
abstract class CustomerAddressModel extends CustomerAddress
    with _$CustomerAddressModel {
  const CustomerAddressModel._({
    required super.id,
    required super.label,
    required super.recipientName,
    required super.phoneNumber,
    required super.country,
    required super.city,
    required super.district,
    required super.postalCode,
    required super.addressLine1,
    super.addressLine2,
    required super.isDefault,
  });

  const factory CustomerAddressModel({
    required String id,
    required String label,
    required String recipientName,
    required String phoneNumber,
    required String country,
    required String city,
    required String district,
    required String postalCode,
    required String addressLine1,
    String? addressLine2,
    required bool isDefault,
  }) = _CustomerAddressModel;

  factory CustomerAddressModel.fromJson(Map<String, dynamic> json) =>
      _$CustomerAddressModelFromJson(json);
}
