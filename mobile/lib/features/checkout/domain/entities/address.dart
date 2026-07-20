class Address {
  const Address({
    required this.fullName,
    required this.phone,
    required this.addressLine,
    required this.city,
    required this.postalCode,
  });

  final String fullName;
  final String phone;
  final String addressLine;
  final String city;
  final String postalCode;

  static const empty = Address(
    fullName: '',
    phone: '',
    addressLine: '',
    city: '',
    postalCode: '',
  );

  String? get validationError {
    if (fullName.trim().isEmpty) return 'Ad soyad zorunludur.';
    if (phone.trim().isEmpty) return 'Telefon numarası zorunludur.';
    if (addressLine.trim().isEmpty) return 'Adres zorunludur.';
    if (city.trim().isEmpty) return 'Şehir zorunludur.';
    if (postalCode.trim().isEmpty) return 'Posta kodu zorunludur.';
    return null;
  }

  Address copyWith({
    String? fullName,
    String? phone,
    String? addressLine,
    String? city,
    String? postalCode,
  }) => Address(
    fullName: fullName ?? this.fullName,
    phone: phone ?? this.phone,
    addressLine: addressLine ?? this.addressLine,
    city: city ?? this.city,
    postalCode: postalCode ?? this.postalCode,
  );
}
