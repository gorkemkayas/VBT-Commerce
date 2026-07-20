import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/utils/result.dart';
import '../../domain/entities/customer.dart';
import '../providers/customer_providers.dart';

class ProfilePage extends ConsumerWidget {
  const ProfilePage({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final customerAsync = ref.watch(currentCustomerProvider);
    return Scaffold(
      appBar: AppBar(title: const Text('Profil Bilgilerim')),
      body: customerAsync.when(
        loading: () => const Center(child: CircularProgressIndicator()),
        error: (error, stackTrace) =>
            Center(child: Text('Profil bilgileri alınamadı: $error')),
        data: (result) => switch (result) {
          Success<Customer>(:final value) => _ProfileContent(customer: value),
          ResultFailure<Customer>(:final failure) => Center(
            child: Text(failure.message),
          ),
        },
      ),
    );
  }
}

class _ProfileContent extends StatelessWidget {
  const _ProfileContent({required this.customer});
  final Customer customer;

  @override
  Widget build(BuildContext context) => ListView(
    padding: const EdgeInsets.all(16),
    children: [
      _ProfileField(label: 'Telefon Numarası', value: customer.phoneNumber),
      const SizedBox(height: 12),
      _ProfileField(label: 'Doğum Tarihi', value: customer.dateOfBirth),
    ],
  );
}

class _ProfileField extends StatelessWidget {
  const _ProfileField({required this.label, required this.value});
  final String label;
  final String? value;

  @override
  Widget build(BuildContext context) => Card(
    child: ListTile(
      title: Text(label),
      subtitle: Text(
        value == null || value!.isEmpty ? 'Belirtilmemiş' : value!,
      ),
    ),
  );
}
