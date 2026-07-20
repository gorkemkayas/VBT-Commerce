// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'customer_address_model.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

_CustomerAddressModel _$CustomerAddressModelFromJson(
  Map<String, dynamic> json,
) => _CustomerAddressModel(
  id: json['id'] as String,
  label: json['label'] as String,
  recipientName: json['recipientName'] as String,
  phoneNumber: json['phoneNumber'] as String,
  country: json['country'] as String,
  city: json['city'] as String,
  district: json['district'] as String,
  postalCode: json['postalCode'] as String,
  addressLine1: json['addressLine1'] as String,
  addressLine2: json['addressLine2'] as String?,
  isDefault: json['isDefault'] as bool,
);

Map<String, dynamic> _$CustomerAddressModelToJson(
  _CustomerAddressModel instance,
) => <String, dynamic>{
  'id': instance.id,
  'label': instance.label,
  'recipientName': instance.recipientName,
  'phoneNumber': instance.phoneNumber,
  'country': instance.country,
  'city': instance.city,
  'district': instance.district,
  'postalCode': instance.postalCode,
  'addressLine1': instance.addressLine1,
  'addressLine2': instance.addressLine2,
  'isDefault': instance.isDefault,
};
