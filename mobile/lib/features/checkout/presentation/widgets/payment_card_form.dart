import 'package:flutter/material.dart';

/// Web'deki "Ödeme Bilgileri" bölümüyle aynı alanlar. Controller'lar
/// `CheckoutPage`'in state'ine ait — bu widget onları yalnızca gösterir,
/// kendi başına bir kalıcı saklama yapmaz.
///
/// Alanlar bilerek dikey (Row/Expanded'sız) yerleştirilir: bu ekranda
/// (`CouponInput`'ta da belgelendiği gibi) Flutter'ın semantics motorunda
/// `Row` içindeki `Expanded(TextField)` beyaz ekrana yol açan bir çökmeye
/// neden oluyordu.
class PaymentCardForm extends StatelessWidget {
  const PaymentCardForm({
    super.key,
    required this.formKey,
    required this.cardHolderNameController,
    required this.cardNumberController,
    required this.cardExpireMonthController,
    required this.cardExpireYearController,
    required this.cardCvcController,
    required this.buyerIdentityNumberController,
  });

  final GlobalKey<FormState> formKey;
  final TextEditingController cardHolderNameController;
  final TextEditingController cardNumberController;
  final TextEditingController cardExpireMonthController;
  final TextEditingController cardExpireYearController;
  final TextEditingController cardCvcController;
  final TextEditingController buyerIdentityNumberController;

  String? _required(String? value) =>
      value == null || value.trim().isEmpty ? 'Bu alan zorunludur.' : null;

  String? _month(String? value) =>
      value != null && RegExp(r'^(0[1-9]|1[0-2])$').hasMatch(value)
      ? null
      : 'AA formatında girin (ör. 12).';

  String? _year(String? value) => value != null && RegExp(r'^\d{4}$').hasMatch(value)
      ? null
      : 'YYYY formatında girin (ör. 2030).';

  String? _cvc(String? value) =>
      value != null && RegExp(r'^\d{3,4}$').hasMatch(value)
      ? null
      : '3 veya 4 haneli olmalı.';

  String? _identityNumber(String? value) =>
      value != null && RegExp(r'^\d{11}$').hasMatch(value)
      ? null
      : '11 haneli olmalı.';

  @override
  Widget build(BuildContext context) => Card(
    child: Padding(
      padding: const EdgeInsets.all(16),
      child: Form(
        key: formKey,
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              'Ödeme Bilgileri',
              style: Theme.of(context).textTheme.titleMedium,
            ),
            const SizedBox(height: 12),
            TextFormField(
              controller: cardHolderNameController,
              decoration: const InputDecoration(
                labelText: 'Kart Üzerindeki İsim',
              ),
              validator: _required,
            ),
            const SizedBox(height: 12),
            TextFormField(
              controller: cardNumberController,
              keyboardType: TextInputType.number,
              decoration: const InputDecoration(labelText: 'Kart Numarası'),
              validator: _required,
            ),
            const SizedBox(height: 12),
            TextFormField(
              controller: cardExpireMonthController,
              keyboardType: TextInputType.number,
              decoration: const InputDecoration(
                labelText: 'Son Kullanma Ayı (MM)',
              ),
              validator: _month,
            ),
            const SizedBox(height: 12),
            TextFormField(
              controller: cardExpireYearController,
              keyboardType: TextInputType.number,
              decoration: const InputDecoration(
                labelText: 'Son Kullanma Yılı (YYYY)',
              ),
              validator: _year,
            ),
            const SizedBox(height: 12),
            TextFormField(
              controller: cardCvcController,
              keyboardType: TextInputType.number,
              decoration: const InputDecoration(labelText: 'CVC'),
              validator: _cvc,
            ),
            const SizedBox(height: 12),
            TextFormField(
              controller: buyerIdentityNumberController,
              keyboardType: TextInputType.number,
              decoration: const InputDecoration(
                labelText: 'TC Kimlik No (Alıcı)',
              ),
              validator: _identityNumber,
            ),
          ],
        ),
      ),
    ),
  );
}
