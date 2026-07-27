import 'package:flutter/material.dart';

import '../../domain/entities/guest_checkout_info.dart';
import '../../domain/entities/guest_contact.dart';

/// Misafir checkout formu: iletişim bilgileri (`POST /api/guest-customers`
/// için) ve teslimat adresi (sipariş oluşturma için) tek formda toplanır.
/// Onaylandığında `onSubmit` çağrılır — üst widget bunu
/// `CheckoutController.submitGuestInfo` ile ilişkilendirir.
class GuestCheckoutForm extends StatefulWidget {
  const GuestCheckoutForm({
    super.key,
    required this.isSubmitting,
    required this.isConfirmed,
    required this.onSubmit,
    this.initialContact,
  });

  final bool isSubmitting;

  /// Misafir müşteri kaydı başarıyla oluşturulduysa `true` — bu durumda form
  /// alanları yerine kısa bir onay mesajı gösterilir.
  final bool isConfirmed;
  final ValueChanged<GuestCheckoutInfo> onSubmit;

  /// Aynı cihazdaki önceki misafir checkout'undan çözülen iletişim bilgileri
  /// (`GET /api/guest-customers/{id}`); verilirse ad/soyad/e-posta/telefon
  /// alanları bunlarla açılır. Adres alanları bu kayıtta olmadığından boş
  /// kalır. Veri form kurulduktan sonra geldiğinde alanların yeniden
  /// tohumlanması için üst widget `key` verir (bkz. `_AddressOrGuestSection`).
  final GuestContact? initialContact;

  @override
  State<GuestCheckoutForm> createState() => _GuestCheckoutFormState();
}

class _GuestCheckoutFormState extends State<GuestCheckoutForm> {
  final _formKey = GlobalKey<FormState>();
  final _firstNameController = TextEditingController();
  final _lastNameController = TextEditingController();
  final _emailController = TextEditingController();
  final _phoneNumberController = TextEditingController();
  final _recipientNameController = TextEditingController();
  final _countryController = TextEditingController(text: 'Türkiye');
  final _cityController = TextEditingController();
  final _districtController = TextEditingController();
  final _postalCodeController = TextEditingController();
  final _addressLine1Controller = TextEditingController();
  final _addressLine2Controller = TextEditingController();

  @override
  void initState() {
    super.initState();
    final contact = widget.initialContact;
    if (contact == null) return;
    _firstNameController.text = contact.firstName;
    _lastNameController.text = contact.lastName;
    _emailController.text = contact.email;
    _phoneNumberController.text = contact.phoneNumber;
    // Alıcı adı varsayılan olarak misafirin kendi adıdır; kullanıcı farklı
    // bir alıcı girmek isterse alanı değiştirebilir.
    _recipientNameController.text = '${contact.firstName} ${contact.lastName}'
        .trim();
  }

  @override
  void dispose() {
    _firstNameController.dispose();
    _lastNameController.dispose();
    _emailController.dispose();
    _phoneNumberController.dispose();
    _recipientNameController.dispose();
    _countryController.dispose();
    _cityController.dispose();
    _districtController.dispose();
    _postalCodeController.dispose();
    _addressLine1Controller.dispose();
    _addressLine2Controller.dispose();
    super.dispose();
  }

  String? _required(String? value) =>
      value == null || value.trim().isEmpty ? 'Bu alan zorunludur.' : null;

  void _submit() {
    if (!_formKey.currentState!.validate()) return;
    widget.onSubmit(
      GuestCheckoutInfo(
        firstName: _firstNameController.text.trim(),
        lastName: _lastNameController.text.trim(),
        email: _emailController.text.trim(),
        phoneNumber: _phoneNumberController.text.trim(),
        recipientName: _recipientNameController.text.trim(),
        country: _countryController.text.trim(),
        city: _cityController.text.trim(),
        district: _districtController.text.trim(),
        postalCode: _postalCodeController.text.trim(),
        addressLine1: _addressLine1Controller.text.trim(),
        addressLine2: _addressLine2Controller.text.trim().isEmpty
            ? null
            : _addressLine2Controller.text.trim(),
      ),
    );
  }

  @override
  Widget build(BuildContext context) => Card(
    child: Padding(
      padding: const EdgeInsets.all(16),
      child: Form(
        key: _formKey,
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              'Misafir Bilgileri',
              style: Theme.of(context).textTheme.titleMedium,
            ),
            const SizedBox(height: 12),
            if (widget.isConfirmed)
              const Text('Bilgileriniz kaydedildi.')
            else ...[
              if (widget.initialContact != null) ...[
                Text(
                  'Önceki siparişinizdeki bilgiler dolduruldu, '
                  'gerekirse değiştirebilirsiniz.',
                  style: Theme.of(context).textTheme.bodySmall?.copyWith(
                    color: Theme.of(context).colorScheme.onSurfaceVariant,
                  ),
                ),
                const SizedBox(height: 12),
              ],
              TextFormField(
                controller: _firstNameController,
                decoration: const InputDecoration(labelText: 'Ad'),
                validator: _required,
              ),
              const SizedBox(height: 12),
              TextFormField(
                controller: _lastNameController,
                decoration: const InputDecoration(labelText: 'Soyad'),
                validator: _required,
              ),
              const SizedBox(height: 12),
              TextFormField(
                controller: _emailController,
                keyboardType: TextInputType.emailAddress,
                decoration: const InputDecoration(labelText: 'E-posta'),
                validator: _required,
              ),
              const SizedBox(height: 12),
              TextFormField(
                controller: _phoneNumberController,
                keyboardType: TextInputType.phone,
                decoration: const InputDecoration(
                  labelText: 'Telefon Numarası',
                ),
                validator: _required,
              ),
              const SizedBox(height: 12),
              TextFormField(
                controller: _recipientNameController,
                decoration: const InputDecoration(
                  labelText: 'Alıcı Adı Soyadı',
                ),
                validator: _required,
              ),
              const SizedBox(height: 12),
              TextFormField(
                controller: _countryController,
                decoration: const InputDecoration(labelText: 'Ülke'),
                validator: _required,
              ),
              const SizedBox(height: 12),
              TextFormField(
                controller: _cityController,
                decoration: const InputDecoration(labelText: 'Şehir'),
                validator: _required,
              ),
              const SizedBox(height: 12),
              TextFormField(
                controller: _districtController,
                decoration: const InputDecoration(labelText: 'İlçe'),
                validator: _required,
              ),
              const SizedBox(height: 12),
              TextFormField(
                controller: _postalCodeController,
                keyboardType: TextInputType.number,
                decoration: const InputDecoration(labelText: 'Posta Kodu'),
                validator: _required,
              ),
              const SizedBox(height: 12),
              TextFormField(
                controller: _addressLine1Controller,
                decoration: const InputDecoration(labelText: 'Adres Satırı 1'),
                validator: _required,
              ),
              const SizedBox(height: 12),
              TextFormField(
                controller: _addressLine2Controller,
                decoration: const InputDecoration(
                  labelText: 'Adres Satırı 2 (opsiyonel)',
                ),
              ),
              const SizedBox(height: 16),
              Align(
                alignment: Alignment.centerRight,
                child: OutlinedButton(
                  onPressed: widget.isSubmitting ? null : _submit,
                  child: widget.isSubmitting
                      ? const SizedBox(
                          height: 18,
                          width: 18,
                          child: CircularProgressIndicator(strokeWidth: 2),
                        )
                      : const Text('Bilgilerimi Onayla'),
                ),
              ),
            ],
          ],
        ),
      ),
    ),
  );
}
