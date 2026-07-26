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
    this.isShippingAddress = false,
    this.isBillingAddress = false,
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

  /// Bu adresin checkout'ta teslimat adresi olarak kullanılabilir
  /// işaretlenip işaretlenmediği.
  final bool isShippingAddress;

  /// Bu adresin checkout'ta fatura adresi olarak seçilebilir işaretlenip
  /// işaretlenmediği (bkz. `Customer.Domain.Entities.CustomerAddress
  /// .IsBillingAddress`).
  final bool isBillingAddress;
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
    this.isShippingAddress = true,
    this.isBillingAddress = false,
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
  final bool isShippingAddress;
  final bool isBillingAddress;
}
