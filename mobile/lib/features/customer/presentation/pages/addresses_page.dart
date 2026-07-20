import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/utils/result.dart';
import '../../domain/entities/customer_address.dart';
import '../providers/customer_providers.dart';
import 'address_form_page.dart';

class AddressesPage extends ConsumerWidget {
  const AddressesPage({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final state = ref.watch(addressesControllerProvider);
    return Scaffold(
      appBar: AppBar(title: const Text('Adreslerim')),
      floatingActionButton: FloatingActionButton(
        onPressed: () => Navigator.of(
          context,
        ).push(MaterialPageRoute(builder: (_) => const AddressFormPage())),
        child: const Icon(Icons.add),
      ),
      body: _buildBody(context, ref, state),
    );
  }

  Widget _buildBody(BuildContext context, WidgetRef ref, AddressesState state) {
    if (state.isLoading && state.addresses.isEmpty) {
      return const Center(child: CircularProgressIndicator());
    }
    if (state.failure != null && state.addresses.isEmpty) {
      return Center(child: Text(state.failure!.message));
    }
    if (state.addresses.isEmpty) {
      return _EmptyAddressesView(
        onAddAddress: () => Navigator.of(
          context,
        ).push(MaterialPageRoute(builder: (_) => const AddressFormPage())),
      );
    }
    return RefreshIndicator(
      onRefresh: () =>
          ref.read(addressesControllerProvider.notifier).loadAddresses(),
      child: ListView.builder(
        padding: const EdgeInsets.all(16),
        itemCount: state.addresses.length,
        itemBuilder: (context, index) =>
            _AddressCard(address: state.addresses[index]),
      ),
    );
  }
}

class _EmptyAddressesView extends StatelessWidget {
  const _EmptyAddressesView({required this.onAddAddress});
  final VoidCallback onAddAddress;

  @override
  Widget build(BuildContext context) => Center(
    child: Padding(
      padding: const EdgeInsets.all(24),
      child: Column(
        mainAxisSize: MainAxisSize.min,
        children: [
          Icon(
            Icons.location_on_outlined,
            size: 64,
            color: Theme.of(context).colorScheme.outline,
          ),
          const SizedBox(height: 16),
          const Text(
            'Henüz kayıtlı adresiniz yok.',
            style: TextStyle(fontWeight: FontWeight.w600),
          ),
          const SizedBox(height: 8),
          const Text(
            'Teslimat için kullanacağınız adresleri buradan ekleyebilirsiniz.',
            textAlign: TextAlign.center,
          ),
          const SizedBox(height: 24),
          FilledButton.icon(
            onPressed: onAddAddress,
            icon: const Icon(Icons.add),
            label: const Text('Adres Ekle'),
          ),
        ],
      ),
    ),
  );
}

class _AddressCard extends ConsumerWidget {
  const _AddressCard({required this.address});
  final CustomerAddress address;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final theme = Theme.of(context);
    final isDefault = address.isDefault;
    return Card(
      margin: const EdgeInsets.only(bottom: 12),
      color: isDefault ? theme.colorScheme.primaryContainer : null,
      shape: isDefault
          ? RoundedRectangleBorder(
              borderRadius: BorderRadius.circular(12),
              side: BorderSide(color: theme.colorScheme.primary, width: 2),
            )
          : null,
      child: Padding(
        padding: const EdgeInsets.all(12),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                Expanded(
                  child: Text(
                    address.label,
                    style: theme.textTheme.titleMedium,
                  ),
                ),
                if (isDefault)
                  const Chip(
                    label: Text('⭐ Varsayılan'),
                    visualDensity: VisualDensity.compact,
                  )
                else
                  PopupMenuButton<void>(
                    itemBuilder: (context) => [
                      PopupMenuItem(
                        onTap: () => ref
                            .read(addressesControllerProvider.notifier)
                            .setDefaultAddress(address.id),
                        child: const Text('Varsayılan Yap'),
                      ),
                    ],
                  ),
              ],
            ),
            const SizedBox(height: 4),
            Text(address.recipientName),
            Text(address.phoneNumber),
            Text(
              '${address.addressLine1}'
              '${address.addressLine2 == null || address.addressLine2!.isEmpty ? '' : ', ${address.addressLine2}'}, '
              '${address.district}/${address.city}, ${address.country} ${address.postalCode}',
            ),
            const SizedBox(height: 8),
            Wrap(
              spacing: 8,
              children: [
                TextButton(
                  onPressed: () => Navigator.of(context).push(
                    MaterialPageRoute(
                      builder: (_) => AddressFormPage(existing: address),
                    ),
                  ),
                  child: const Text('Düzenle'),
                ),
                TextButton(
                  onPressed: () => _confirmDelete(context, ref),
                  child: const Text('Sil'),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }

  Future<void> _confirmDelete(BuildContext context, WidgetRef ref) async {
    final confirmed = await showDialog<bool>(
      context: context,
      builder: (dialogContext) => AlertDialog(
        title: const Text('Adresi sil'),
        content: const Text('Bu adresi silmek istediğinize emin misiniz?'),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(dialogContext).pop(false),
            child: const Text('Vazgeç'),
          ),
          TextButton(
            onPressed: () => Navigator.of(dialogContext).pop(true),
            child: const Text('Sil'),
          ),
        ],
      ),
    );
    if (confirmed != true || !context.mounted) return;
    final result = await ref
        .read(addressesControllerProvider.notifier)
        .deleteAddress(address.id);
    if (!context.mounted) return;
    if (result case ResultFailure(:final failure)) {
      ScaffoldMessenger.of(
        context,
      ).showSnackBar(SnackBar(content: Text(failure.message)));
    }
  }
}
