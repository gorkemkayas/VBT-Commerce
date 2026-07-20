class CustomerAddress {
  const CustomerAddress({
    required this.id,
    required this.label,
    required this.recipientName,
    required this.phoneNumber,
    required this.country,
    required this.city,
    required this.district,
    required this.postalCode,
    required this.addressLine1,
    this.addressLine2,
    required this.isDefault,
  });

  final String id;
  final String label;
  final String recipientName;
  final String phoneNumber;
  final String country;
  final String city;
  final String district;
  final String postalCode;
  final String addressLine1;
  final String? addressLine2;
  final bool isDefault;
}

/// `AddCustomerAddressRequest` / `UpdateCustomerAddressRequest` gövdesine
/// karşılık gelir — `id` sunucu tarafından üretildiği için burada yoktur.
class CustomerAddressInput {
  const CustomerAddressInput({
    required this.label,
    required this.recipientName,
    required this.phoneNumber,
    required this.country,
    required this.city,
    required this.district,
    required this.postalCode,
    required this.addressLine1,
    this.addressLine2,
    required this.isDefault,
  });

  final String label;
  final String recipientName;
  final String phoneNumber;
  final String country;
  final String city;
  final String district;
  final String postalCode;
  final String addressLine1;
  final String? addressLine2;
  final bool isDefault;
}
