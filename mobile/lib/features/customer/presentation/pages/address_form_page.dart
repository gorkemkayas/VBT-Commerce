import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/utils/result.dart';
import '../../../auth/presentation/widgets/primary_button.dart';
import '../../domain/entities/customer_address.dart';
import '../providers/customer_providers.dart';

const _quickPickCities = [
  'Antalya',
  'İstanbul',
  'Ankara',
  'İzmir',
  'Bursa',
  'Adana',
  'Konya',
  'Gaziantep',
  'Mersin',
  'Kocaeli',
];

/// `existing` verilirse düzenleme, verilmezse yeni adres ekleme modunda çalışır.
class AddressFormPage extends ConsumerStatefulWidget {
  const AddressFormPage({super.key, this.existing});
  final CustomerAddress? existing;

  @override
  ConsumerState<AddressFormPage> createState() => _AddressFormPageState();
}

class _AddressFormPageState extends ConsumerState<AddressFormPage> {
  final _formKey = GlobalKey<FormState>();
  late final _labelController = TextEditingController(
    text: widget.existing?.label,
  );
  late final _recipientNameController = TextEditingController(
    text: widget.existing?.recipientName,
  );
  late final _phoneNumberController = TextEditingController(
    text: widget.existing?.phoneNumber,
  );
  late final _countryController = TextEditingController(
    text: widget.existing?.country ?? 'Türkiye',
  );
  late final _cityController = TextEditingController(
    text: widget.existing?.city,
  );
  late final _districtController = TextEditingController(
    text: widget.existing?.district,
  );
  late final _postalCodeController = TextEditingController(
    text: widget.existing?.postalCode,
  );
  late final _addressLine1Controller = TextEditingController(
    text: widget.existing?.addressLine1,
  );
  late final _addressLine2Controller = TextEditingController(
    text: widget.existing?.addressLine2,
  );
  late bool _isDefault = widget.existing?.isDefault ?? false;
  bool _isSubmitting = false;

  @override
  void dispose() {
    _labelController.dispose();
    _recipientNameController.dispose();
    _phoneNumberController.dispose();
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

  Future<void> _submit() async {
    if (!_formKey.currentState!.validate()) return;
    setState(() => _isSubmitting = true);
    final input = CustomerAddressInput(
      label: _labelController.text.trim(),
      recipientName: _recipientNameController.text.trim(),
      phoneNumber: _phoneNumberController.text.trim(),
      country: _countryController.text.trim(),
      city: _cityController.text.trim(),
      district: _districtController.text.trim(),
      postalCode: _postalCodeController.text.trim(),
      addressLine1: _addressLine1Controller.text.trim(),
      addressLine2: _addressLine2Controller.text.trim().isEmpty
          ? null
          : _addressLine2Controller.text.trim(),
      isDefault: _isDefault,
    );
    final controller = ref.read(addressesControllerProvider.notifier);
    final existing = widget.existing;
    final result = existing == null
        ? await controller.addAddress(input)
        : await controller.updateAddress(existing.id, input);
    if (!mounted) return;
    setState(() => _isSubmitting = false);
    switch (result) {
      case Success():
        Navigator.of(context).pop();
      case ResultFailure(:final failure):
        ScaffoldMessenger.of(
          context,
        ).showSnackBar(SnackBar(content: Text(failure.message)));
    }
  }

  @override
  Widget build(BuildContext context) => Scaffold(
    appBar: AppBar(
      title: Text(widget.existing == null ? 'Adres Ekle' : 'Adresi Düzenle'),
    ),
    body: SafeArea(
      child: SingleChildScrollView(
        padding: const EdgeInsets.all(16),
        child: Form(
          key: _formKey,
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              TextFormField(
                controller: _labelController,
                decoration: const InputDecoration(
                  labelText: 'Etiket (Ev, İş...)',
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
                controller: _phoneNumberController,
                keyboardType: TextInputType.phone,
                decoration: const InputDecoration(
                  labelText: 'Telefon Numarası',
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
              Autocomplete<String>(
                initialValue: TextEditingValue(text: _cityController.text),
                optionsBuilder: (textEditingValue) {
                  if (textEditingValue.text.isEmpty) {
                    return _quickPickCities;
                  }
                  return _quickPickCities.where(
                    (city) => city.toLowerCase().contains(
                      textEditingValue.text.toLowerCase(),
                    ),
                  );
                },
                onSelected: (selection) => _cityController.text = selection,
                fieldViewBuilder:
                    (context, fieldController, fieldFocusNode, _) {
                      return TextFormField(
                        controller: fieldController,
                        focusNode: fieldFocusNode,
                        decoration: const InputDecoration(labelText: 'Şehir'),
                        validator: _required,
                        onChanged: (value) => _cityController.text = value,
                      );
                    },
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
              const SizedBox(height: 12),
              SwitchListTile(
                contentPadding: EdgeInsets.zero,
                title: const Text('Varsayılan adres olarak ayarla'),
                value: _isDefault,
                onChanged: (value) => setState(() => _isDefault = value),
              ),
              const SizedBox(height: 24),
              PrimaryButton(
                label: 'Kaydet',
                isLoading: _isSubmitting,
                onPressed: _submit,
              ),
            ],
          ),
        ),
      ),
    ),
  );
}
