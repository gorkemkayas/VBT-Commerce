/// Misafir checkout'ta toplanan iletişim ve teslimat bilgileri. İlk dört alan
/// `POST /api/guest-customers` için, kalanı sipariş oluşturmadaki adres
/// gövdesi için kullanılır (bkz. `GuestCustomersController`/`GuestOrdersController`).
class GuestCheckoutInfo {
  const GuestCheckoutInfo({
    required this.firstName,
    required this.lastName,
    required this.email,
    required this.phoneNumber,
    required this.recipientName,
    required this.country,
    required this.city,
    required this.district,
    required this.postalCode,
    required this.addressLine1,
    this.addressLine2,
  });

  final String firstName;
  final String lastName;
  final String email;
  final String phoneNumber;
  final String recipientName;
  final String country;
  final String city;
  final String district;
  final String postalCode;
  final String addressLine1;
  final String? addressLine2;
}
