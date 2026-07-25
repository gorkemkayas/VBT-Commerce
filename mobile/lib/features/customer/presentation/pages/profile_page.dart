import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:intl/intl.dart';

import '../../../../core/utils/result.dart';
import '../../../auth/presentation/providers/auth_providers.dart';
import '../../domain/entities/customer.dart';
import '../providers/customer_providers.dart';

final _dateFormat = DateFormat('d MMMM y', 'tr_TR');

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

/// Telefon/doğum tarihi burada doğrudan düzenlenip `PUT /api/customers/me`
/// ile kaydedilir — sadece iki alan olduğu için (Adreslerim'deki gibi) ayrı
/// bir form ekranı açmaya gerek görülmedi.
class _ProfileContent extends ConsumerStatefulWidget {
  const _ProfileContent({required this.customer});
  final Customer customer;

  @override
  ConsumerState<_ProfileContent> createState() => _ProfileContentState();
}

class _ProfileContentState extends ConsumerState<_ProfileContent> {
  late final _phoneController = TextEditingController(
    text: widget.customer.phoneNumber,
  );
  DateTime? _dateOfBirth;
  bool _isSubmitting = false;

  @override
  void initState() {
    super.initState();
    final raw = widget.customer.dateOfBirth;
    _dateOfBirth = raw == null ? null : DateTime.tryParse(raw);
  }

  @override
  void dispose() {
    _phoneController.dispose();
    super.dispose();
  }

  Future<void> _pickDateOfBirth() async {
    final now = DateTime.now();
    final picked = await showDatePicker(
      context: context,
      initialDate: _dateOfBirth ?? DateTime(now.year - 25),
      firstDate: DateTime(1900),
      lastDate: now.subtract(const Duration(days: 1)),
    );
    if (picked != null) setState(() => _dateOfBirth = picked);
  }

  Future<void> _save() async {
    setState(() => _isSubmitting = true);
    final phoneNumber = _phoneController.text.trim();
    final dateOfBirth = _dateOfBirth;
    final result = await ref.read(updateCustomerProfileUseCaseProvider)(
      phoneNumber: phoneNumber.isEmpty ? null : phoneNumber,
      // Backend `DateOnly` (`yyyy-MM-dd`) bekliyor.
      dateOfBirth: dateOfBirth == null
          ? null
          : '${dateOfBirth.year.toString().padLeft(4, '0')}-'
                '${dateOfBirth.month.toString().padLeft(2, '0')}-'
                '${dateOfBirth.day.toString().padLeft(2, '0')}',
    );
    if (!mounted) return;
    setState(() => _isSubmitting = false);
    switch (result) {
      case Success<bool>():
        // Diğer ekranlar (ör. bir sonraki `currentCustomerProvider`
        // izleyicisi) güncel veriyi görsün diye yeniden yüklenmeye zorlanır.
        ref.invalidate(currentCustomerProvider);
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(content: Text('Profil bilgileriniz güncellendi.')),
        );
      case ResultFailure<bool>(:final failure):
        ScaffoldMessenger.of(
          context,
        ).showSnackBar(SnackBar(content: Text(failure.message)));
    }
  }

  @override
  Widget build(BuildContext context) {
    final dateOfBirth = _dateOfBirth;
    final userAsync = ref.watch(currentUserProvider);
    final email = userAsync.asData?.value?.email;
    return ListView(
      padding: const EdgeInsets.all(16),
      children: [
        _InfoTile(
          icon: Icons.email_outlined,
          label: 'E-posta',
          value: email ?? '—',
        ),
        const SizedBox(height: 20),
        Text(
          'İletişim Bilgileri',
          style: Theme.of(context).textTheme.titleMedium,
        ),
        const SizedBox(height: 12),
        TextField(
          controller: _phoneController,
          keyboardType: TextInputType.phone,
          decoration: const InputDecoration(labelText: 'Telefon Numarası'),
        ),
        const SizedBox(height: 12),
        InkWell(
          onTap: _pickDateOfBirth,
          child: InputDecorator(
            decoration: const InputDecoration(labelText: 'Doğum Tarihi'),
            child: Text(
              dateOfBirth != null
                  ? _dateFormat.format(dateOfBirth)
                  : 'Belirtilmemiş',
            ),
          ),
        ),
        const SizedBox(height: 24),
        FilledButton(
          onPressed: _isSubmitting ? null : _save,
          child: _isSubmitting
              ? const SizedBox(
                  height: 20,
                  width: 20,
                  child: CircularProgressIndicator(strokeWidth: 2),
                )
              : const Text('Kaydet'),
        ),
      ],
    );
  }
}

/// Değiştirilemeyen (salt-okunur) profil bilgisi satırı — ör. e-posta.
class _InfoTile extends StatelessWidget {
  const _InfoTile({
    required this.icon,
    required this.label,
    required this.value,
  });

  final IconData icon;
  final String label;
  final String value;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return Row(
      children: [
        Icon(icon, color: theme.colorScheme.primary),
        const SizedBox(width: 12),
        Expanded(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(label, style: theme.textTheme.labelMedium),
              const SizedBox(height: 2),
              Text(value, style: theme.textTheme.bodyLarge),
            ],
          ),
        ),
      ],
    );
  }
}
