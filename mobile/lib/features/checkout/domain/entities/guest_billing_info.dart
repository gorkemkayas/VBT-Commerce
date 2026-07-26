/// Misafir checkout'ta, "fatura adresim teslimat adresimle aynı" seçeneği
/// kapatıldığında toplanan ayrı fatura adresi. `GuestCheckoutInfo`'nun aksine
/// telefon numarası taşımaz — backend'in `PlaceGuestOrderRequest`'indeki
/// `Billing*` alanlarıyla birebir eşleşir.
class GuestBillingInfo {
  const GuestBillingInfo({
    required this.recipientName,
    required this.country,
    required this.city,
    required this.district,
    required this.postalCode,
    required this.addressLine1,
    this.addressLine2,
  });

  final String recipientName;
  final String country;
  final String city;
  final String district;
  final String postalCode;
  final String addressLine1;
  final String? addressLine2;
}
