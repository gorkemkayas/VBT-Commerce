import 'package:flutter/material.dart';

import '../../../customer/domain/entities/customer_address.dart';

/// Kayıtlı adresleri radio-list olarak gösterir. Seçilen adresin id'si
/// `CheckoutState.selectedAddressId`'de tutulur — sipariş oluşturma bu id'yi
/// kullanacak (henüz bu aşamada uygulanmıyor, bkz. `CompleteOrderUseCase`).
class AddressSelector extends StatelessWidget {
  const AddressSelector({
    super.key,
    required this.addresses,
    required this.selectedAddressId,
    required this.onSelected,
  });

  final List<CustomerAddress> addresses;
  final String? selectedAddressId;
  final ValueChanged<String> onSelected;

  @override
  Widget build(BuildContext context) => Card(
    child: Padding(
      padding: const EdgeInsets.symmetric(vertical: 8),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Padding(
            padding: const EdgeInsets.fromLTRB(16, 8, 16, 4),
            child: Text(
              'Teslimat Adresi',
              style: Theme.of(context).textTheme.titleMedium,
            ),
          ),
          RadioGroup<String>(
            groupValue: selectedAddressId,
            onChanged: (value) {
              if (value != null) onSelected(value);
            },
            child: Column(
              children: [
                for (final address in addresses)
                  RadioListTile<String>(
                    value: address.id,
                    title: Text(address.label),
                    subtitle: Text(_summary(address)),
                    isThreeLine: true,
                  ),
              ],
            ),
          ),
        ],
      ),
    ),
  );

  String _summary(CustomerAddress address) {
    final line2 = address.addressLine2;
    final addressLine = line2 == null || line2.isEmpty
        ? address.addressLine1
        : '${address.addressLine1}, $line2';
    return '${address.recipientName}\n'
        '$addressLine, ${address.district}/${address.city}, '
        '${address.country} ${address.postalCode}';
  }
}
