import '../../domain/entities/guest_contact.dart';

/// `GET /api/guest-customers/{guestCustomerId}` yanıtındaki
/// `GuestCustomerDto` şekline karşılık gelir.
class GuestContactModel extends GuestContact {
  const GuestContactModel({
    required super.id,
    required super.firstName,
    required super.lastName,
    required super.email,
    required super.phoneNumber,
  });

  factory GuestContactModel.fromJson(Map<String, dynamic> json) =>
      GuestContactModel(
        id: json['id'] as String,
        firstName: json['firstName'] as String? ?? '',
        lastName: json['lastName'] as String? ?? '',
        email: json['email'] as String? ?? '',
        phoneNumber: json['phoneNumber'] as String? ?? '',
      );
}
