import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/constants/route_paths.dart';
import '../../../../core/utils/result.dart';
import '../../../customer/domain/entities/customer.dart';
import '../../../customer/domain/entities/customer_address.dart';
import '../../../customer/presentation/providers/customer_providers.dart';
import '../../domain/entities/guest_billing_info.dart';
import '../providers/checkout_providers.dart';
import 'address_selector.dart';

/// Fatura adresi bölümü — web'deki "Fatura adresim teslimat adresimle aynı"
/// akışıyla aynı: checkbox açıkken (varsayılan) hiçbir ek alan/istek
/// gerekmez; kapatılınca giriş yapmış kullanıcı kayıtlı (`isBillingAddress`
/// işaretli) adreslerinden birini seçer, misafir ise ayrı bir form doldurur.
/// `loggedIn == null` iken (oturum durumu henüz belirlenmedi)
/// `_AddressOrGuestSection` ile aynı yaklaşımla hiçbir şey göstermez.
class BillingAddressSection extends ConsumerWidget {
  const BillingAddressSection({super.key, required this.loggedIn});
  final bool? loggedIn;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    if (loggedIn == null) return const SizedBox.shrink();
    final sameBillingAddress = ref.watch(
      checkoutControllerProvider.select((state) => state.sameBillingAddress),
    );
    final controller = ref.read(checkoutControllerProvider.notifier);
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              'Fatura Adresi',
              style: Theme.of(context).textTheme.titleMedium,
            ),
            CheckboxListTile(
              contentPadding: EdgeInsets.zero,
              controlAffinity: ListTileControlAffinity.leading,
              value: sameBillingAddress,
              onChanged: (value) =>
                  controller.setSameBillingAddress(value ?? true),
              title: const Text('Fatura adresim teslimat adresimle aynı'),
            ),
            if (!sameBillingAddress)
              loggedIn!
                  ? const _BillingAddressPicker()
                  : const _GuestBillingForm(),
          ],
        ),
      ),
    );
  }
}

class _BillingAddressPicker extends ConsumerWidget {
  const _BillingAddressPicker();

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final billingAddressId = ref.watch(
      checkoutControllerProvider.select((state) => state.billingAddressId),
    );
    final controller = ref.read(checkoutControllerProvider.notifier);
    final customerResult = ref.watch(currentCustomerProvider);
    return customerResult.when(
      loading: () => const Padding(
        padding: EdgeInsets.symmetric(vertical: 16),
        child: Center(child: CircularProgressIndicator()),
      ),
      error: (_, _) => const Padding(
        padding: EdgeInsets.only(top: 12),
        child: Text('Adresleriniz yüklenemedi.'),
      ),
      data: (result) => switch (result) {
        Success<Customer>(:final value) => _billingList(
          context,
          value.addresses.where((a) => a.isBillingAddress).toList(),
          billingAddressId,
          controller.selectBillingAddress,
        ),
        ResultFailure<Customer>() => const Padding(
          padding: EdgeInsets.only(top: 12),
          child: Text('Adresleriniz yüklenemedi.'),
        ),
      },
    );
  }

  Widget _billingList(
    BuildContext context,
    List<CustomerAddress> billingAddresses,
    String? selectedId,
    ValueChanged<String> onSelected,
  ) {
    if (billingAddresses.isEmpty) {
      return Padding(
        padding: const EdgeInsets.only(top: 12),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Text(
              'Fatura adresi olarak işaretlenmiş kayıtlı adresiniz yok.',
            ),
            const SizedBox(height: 8),
            OutlinedButton.icon(
              onPressed: () => context.push(RoutePaths.addresses),
              icon: const Icon(Icons.add),
              label: const Text('Adreslerim'),
            ),
          ],
        ),
      );
    }
    return Padding(
      padding: const EdgeInsets.only(top: 8),
      child: AddressSelector(
        addresses: billingAddresses,
        selectedAddressId: selectedId,
        onSelected: onSelected,
        title: 'Fatura adresi seçin',
      ),
    );
  }
}

/// Yerel form: alanlar `TextEditingController`'larda tutulur, Riverpod
/// state'ine (`CheckoutController.setGuestBillingInfo`) yalnızca "Kaydet"e
/// basıldığında **tek seferlik** yazılır — her tuş vuruşunda değil. Bu,
/// `CheckoutPage`'in en üstte `checkoutControllerProvider`'ın tamamını
/// izlemesi nedeniyle (bkz. `coupon_input.dart`'taki aynı önlem) gereksiz/
/// riskli tam sayfa yeniden derlemelerini önler.
class _GuestBillingForm extends ConsumerStatefulWidget {
  const _GuestBillingForm();

  @override
  ConsumerState<_GuestBillingForm> createState() => _GuestBillingFormState();
}

class _GuestBillingFormState extends ConsumerState<_GuestBillingForm> {
  final _formKey = GlobalKey<FormState>();
  final _recipientNameController = TextEditingController();
  final _countryController = TextEditingController(text: 'Türkiye');
  final _cityController = TextEditingController();
  final _districtController = TextEditingController();
  final _postalCodeController = TextEditingController();
  final _addressLine1Controller = TextEditingController();
  final _addressLine2Controller = TextEditingController();

  @override
  void dispose() {
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

  void _save() {
    if (!_formKey.currentState!.validate()) return;
    ref
        .read(checkoutControllerProvider.notifier)
        .setGuestBillingInfo(
          GuestBillingInfo(
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
    ScaffoldMessenger.of(
      context,
    ).showSnackBar(const SnackBar(content: Text('Fatura adresi kaydedildi.')));
  }

  @override
  Widget build(BuildContext context) => Padding(
    padding: const EdgeInsets.only(top: 12),
    child: Form(
      key: _formKey,
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          TextFormField(
            controller: _recipientNameController,
            decoration: const InputDecoration(labelText: 'Alıcı Adı Soyadı'),
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
          const SizedBox(height: 12),
          Align(
            alignment: Alignment.centerRight,
            child: OutlinedButton(
              onPressed: _save,
              child: const Text('Fatura Bilgilerini Kaydet'),
            ),
          ),
        ],
      ),
    ),
  );
}
