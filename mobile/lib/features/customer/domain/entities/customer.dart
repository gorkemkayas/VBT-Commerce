import 'customer_address.dart';

class Customer {
  const Customer({
    required this.id,
    required this.userId,
    this.phoneNumber,
    this.dateOfBirth,
    this.addresses = const [],
  });

  final String id;
  final String userId;
  final String? phoneNumber;
  final String? dateOfBirth;
  final List<CustomerAddress> addresses;
}
