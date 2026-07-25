import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:intl/intl.dart';

import '../../../../core/utils/result.dart';
import '../../domain/entities/review.dart';
import '../providers/review_providers.dart';
import '../widgets/star_rating_input.dart';

final _dateFormat = DateFormat('d MMMM y', 'tr_TR');

/// Hesabım > Yorumlarım — `OrdersPage` ile aynı desen (bkz.
/// `orders_page.dart`): ilk açılışta otomatik yüklenir, aşağı çekince
/// yenilenir. Ürün adı gösterilmez (`ReviewDto` bunu taşımıyor) — sadece
/// puan, yorum metni ve tarih; her satırda düzenle/sil aksiyonu vardır.
class MyReviewsPage extends ConsumerWidget {
  const MyReviewsPage({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final state = ref.watch(myReviewsControllerProvider);
    return Scaffold(
      appBar: AppBar(title: const Text('Yorumlarım')),
      body: RefreshIndicator(
        onRefresh: () =>
            ref.read(myReviewsControllerProvider.notifier).loadMyReviews(),
        child: _buildBody(context, ref, state),
      ),
    );
  }

  Widget _buildBody(BuildContext context, WidgetRef ref, MyReviewsState state) {
    if (state.isLoading && state.reviews.isEmpty) {
      return const Center(child: CircularProgressIndicator());
    }
    if (state.reviews.isEmpty) {
      // `AlwaysScrollableScrollPhysics`, liste boşken de aşağı çekip
      // yenilemeye izin verir.
      return ListView(
        physics: const AlwaysScrollableScrollPhysics(),
        children: [
          SizedBox(
            height: 400,
            child: Center(
              child: Text(
                state.failure?.message ?? 'Henüz yorum yapmadınız.',
                textAlign: TextAlign.center,
              ),
            ),
          ),
        ],
      );
    }
    return ListView.separated(
      physics: const AlwaysScrollableScrollPhysics(),
      padding: const EdgeInsets.symmetric(vertical: 8, horizontal: 16),
      itemCount: state.reviews.length,
      separatorBuilder: (context, index) => const Divider(height: 24),
      itemBuilder: (context, index) =>
          _MyReviewTile(review: state.reviews[index]),
    );
  }
}

class _MyReviewTile extends ConsumerWidget {
  const _MyReviewTile({required this.review});
  final Review review;

  Future<void> _confirmDelete(BuildContext context, WidgetRef ref) async {
    final confirmed = await showDialog<bool>(
      context: context,
      builder: (dialogContext) => AlertDialog(
        title: const Text('Yorumu Sil'),
        content: const Text('Bu yorumu silmek istediğinize emin misiniz?'),
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

    final result = await ref.read(deleteReviewUseCaseProvider)(review.id);
    if (!context.mounted) return;
    switch (result) {
      case Success<bool>():
        ref.read(myReviewsControllerProvider.notifier).loadMyReviews();
        ScaffoldMessenger.of(
          context,
        ).showSnackBar(const SnackBar(content: Text('Yorumunuz silindi.')));
      case ResultFailure<bool>(:final failure):
        ScaffoldMessenger.of(
          context,
        ).showSnackBar(SnackBar(content: Text(failure.message)));
    }
  }

  Future<void> _openEditSheet(BuildContext context) => showModalBottomSheet<void>(
    context: context,
    isScrollControlled: true,
    builder: (_) => _EditReviewSheet(review: review),
  );

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final theme = Theme.of(context);
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Row(
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          children: [
            _ReadOnlyStars(rating: review.rating),
            Row(
              mainAxisSize: MainAxisSize.min,
              children: [
                IconButton(
                  icon: const Icon(Icons.edit_outlined, size: 20),
                  tooltip: 'Düzenle',
                  onPressed: () => _openEditSheet(context),
                ),
                IconButton(
                  icon: const Icon(Icons.delete_outline, size: 20),
                  tooltip: 'Sil',
                  onPressed: () => _confirmDelete(context, ref),
                ),
              ],
            ),
          ],
        ),
        Text(
          _dateFormat.format(review.createdAt),
          style: theme.textTheme.bodySmall,
        ),
        const SizedBox(height: 4),
        Text(review.comment, style: theme.textTheme.bodyMedium),
      ],
    );
  }
}

class _ReadOnlyStars extends StatelessWidget {
  const _ReadOnlyStars({required this.rating});
  final int rating;

  @override
  Widget build(BuildContext context) => Row(
    mainAxisSize: MainAxisSize.min,
    children: List.generate(
      5,
      (index) => Icon(
        index < rating ? Icons.star : Icons.star_border,
        size: 18,
        color: Colors.black,
      ),
    ),
  );
}

class _EditReviewSheet extends ConsumerStatefulWidget {
  const _EditReviewSheet({required this.review});
  final Review review;

  @override
  ConsumerState<_EditReviewSheet> createState() => _EditReviewSheetState();
}

class _EditReviewSheetState extends ConsumerState<_EditReviewSheet> {
  final _formKey = GlobalKey<FormState>();
  late final _commentController = TextEditingController(
    text: widget.review.comment,
  );
  late int _rating = widget.review.rating;
  bool _isSubmitting = false;

  @override
  void dispose() {
    _commentController.dispose();
    super.dispose();
  }

  Future<void> _submit() async {
    if (_rating == 0) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Lütfen 1-5 arasında bir puan seçin.')),
      );
      return;
    }
    if (!_formKey.currentState!.validate()) return;
    setState(() => _isSubmitting = true);
    final result = await ref.read(updateReviewUseCaseProvider)(
      reviewId: widget.review.id,
      rating: _rating,
      comment: _commentController.text.trim(),
    );
    if (!mounted) return;
    setState(() => _isSubmitting = false);

    switch (result) {
      case Success<bool>():
        ref.read(myReviewsControllerProvider.notifier).loadMyReviews();
        Navigator.of(context).pop();
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(content: Text('Yorumunuz güncellendi.')),
        );
      case ResultFailure<bool>(:final failure):
        ScaffoldMessenger.of(
          context,
        ).showSnackBar(SnackBar(content: Text(failure.message)));
    }
  }

  @override
  Widget build(BuildContext context) => Padding(
    padding: EdgeInsets.fromLTRB(
      16,
      16,
      16,
      16 + MediaQuery.of(context).viewInsets.bottom,
    ),
    child: Form(
      key: _formKey,
      child: Column(
        mainAxisSize: MainAxisSize.min,
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          Text(
            'Yorumu Düzenle',
            style: Theme.of(context).textTheme.titleMedium,
          ),
          const SizedBox(height: 12),
          StarRatingInput(
            rating: _rating,
            onChanged: (value) => setState(() => _rating = value),
          ),
          const SizedBox(height: 12),
          TextFormField(
            controller: _commentController,
            maxLines: 2,
            maxLength: 2000,
            decoration: const InputDecoration(
              hintText: 'Yorumunuz...',
              isDense: true,
              counterText: '',
            ),
            validator: (value) => value == null || value.trim().isEmpty
                ? 'Yorum boş olamaz.'
                : null,
          ),
          const SizedBox(height: 8),
          Align(
            alignment: Alignment.centerRight,
            child: FilledButton(
              onPressed: _isSubmitting ? null : _submit,
              child: _isSubmitting
                  ? const SizedBox(
                      height: 16,
                      width: 16,
                      child: CircularProgressIndicator(strokeWidth: 2),
                    )
                  : const Text('Kaydet'),
            ),
          ),
        ],
      ),
    ),
  );
}
