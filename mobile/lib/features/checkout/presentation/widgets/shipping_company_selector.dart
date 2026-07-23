import 'package:flutter/material.dart';

import '../../../../core/utils/currency_formatter.dart';
import '../../domain/entities/shipping_company.dart';

/// Aktif kargo firmalarını radio-list olarak gösterir (`AddressSelector` ile
/// aynı desen). Seçilen firmanın id'si `CheckoutState.selectedShippingCompanyId`
/// içinde tutulur; ücreti Ödeme Özeti'nde ayrıca gösterilir (bkz.
/// `PaymentSummaryView`).
class ShippingCompanySelector extends StatelessWidget {
  const ShippingCompanySelector({
    super.key,
    required this.companies,
    required this.selectedShippingCompanyId,
    required this.onSelected,
  });

  final List<ShippingCompany> companies;
  final String? selectedShippingCompanyId;
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
              'Kargo Firması',
              style: Theme.of(context).textTheme.titleMedium,
            ),
          ),
          RadioGroup<String>(
            groupValue: selectedShippingCompanyId,
            onChanged: (value) {
              if (value != null) onSelected(value);
            },
            child: Column(
              children: [
                for (final company in companies)
                  RadioListTile<String>(
                    value: company.id,
                    title: Text(company.name),
                    subtitle: Text(company.fee.toTryCurrency()),
                  ),
              ],
            ),
          ),
        ],
      ),
    ),
  );
}
