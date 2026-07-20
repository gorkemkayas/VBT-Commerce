import 'package:flutter/material.dart';

import '../../domain/entities/address.dart';

class AddressForm extends StatefulWidget {
  const AddressForm({
    super.key,
    required this.address,
    required this.onChanged,
  });
  final Address address;
  final ValueChanged<Address> onChanged;

  @override
  State<AddressForm> createState() => _AddressFormState();
}

class _AddressFormState extends State<AddressForm> {
  late final _fullNameController = TextEditingController(
    text: widget.address.fullName,
  );
  late final _phoneController = TextEditingController(
    text: widget.address.phone,
  );
  late final _addressLineController = TextEditingController(
    text: widget.address.addressLine,
  );
  late final _cityController = TextEditingController(text: widget.address.city);
  late final _postalCodeController = TextEditingController(
    text: widget.address.postalCode,
  );

  @override
  void dispose() {
    _fullNameController.dispose();
    _phoneController.dispose();
    _addressLineController.dispose();
    _cityController.dispose();
    _postalCodeController.dispose();
    super.dispose();
  }

  void _emitChange() => widget.onChanged(
    Address(
      fullName: _fullNameController.text,
      phone: _phoneController.text,
      addressLine: _addressLineController.text,
      city: _cityController.text,
      postalCode: _postalCodeController.text,
    ),
  );

  @override
  Widget build(BuildContext context) => Card(
    child: Padding(
      padding: const EdgeInsets.all(16),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            'Teslimat Adresi',
            style: Theme.of(context).textTheme.titleMedium,
          ),
          const SizedBox(height: 12),
          TextField(
            controller: _fullNameController,
            decoration: const InputDecoration(labelText: 'Ad Soyad'),
            onChanged: (_) => _emitChange(),
          ),
          const SizedBox(height: 8),
          TextField(
            controller: _phoneController,
            decoration: const InputDecoration(labelText: 'Telefon'),
            keyboardType: TextInputType.phone,
            onChanged: (_) => _emitChange(),
          ),
          const SizedBox(height: 8),
          TextField(
            controller: _addressLineController,
            decoration: const InputDecoration(labelText: 'Adres'),
            maxLines: 2,
            onChanged: (_) => _emitChange(),
          ),
          const SizedBox(height: 8),
          TextField(
            controller: _cityController,
            decoration: const InputDecoration(labelText: 'Şehir'),
            onChanged: (_) => _emitChange(),
          ),
          const SizedBox(height: 8),
          TextField(
            controller: _postalCodeController,
            decoration: const InputDecoration(labelText: 'Posta Kodu'),
            keyboardType: TextInputType.number,
            onChanged: (_) => _emitChange(),
          ),
        ],
      ),
    ),
  );
}
