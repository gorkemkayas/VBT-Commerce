/// `GET /api/guest-customers/{guestCustomerId}` ile dönen misafir kaydının
/// iletişim bilgileri (`GuestCustomerDto`). Yalnızca ad/soyad/e-posta/telefon
/// içerir — adres bu kaydın parçası değildir, sipariş oluşturulurken ayrıca
/// alınır (bkz. `GuestCheckoutInfo`).
class GuestContact {
  const GuestContact({
    required this.id,
    required this.firstName,
    required this.lastName,
    required this.email,
    required this.phoneNumber,
  });

  final String id;
  final String firstName;
  final String lastName;
  final String email;
  final String phoneNumber;
}
