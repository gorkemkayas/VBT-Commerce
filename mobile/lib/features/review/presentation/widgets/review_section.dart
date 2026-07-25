import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:intl/intl.dart';

import '../../../../core/theme/app_spacing.dart';
import '../../../../core/utils/result.dart';
import '../../../../core/widgets/async_state_views.dart';
import '../../domain/entities/review.dart';
import '../../domain/entities/review_item_type.dart';
import '../../domain/entities/review_summary.dart';
import '../providers/review_providers.dart';
import 'star_rating_input.dart';

final _dateFormat = DateFormat('d MMMM y', 'tr_TR');

/// "Değerlendirme Yaz" formu — satın alma kontrolü tamamen backend'de
/// yapılır (`CreateMyReviewCommandHandler`); burada tekrarlanmaz. Backend
/// hatası (ör. satın alınmamış ürün, tekrar değerlendirme) `Result.failure`
/// üzerinden aynen kullanıcıya gösterilir (bkz. `_CreateReviewFormState._submit`).
const _showCreateReviewForm = true;

/// Ürün detay sayfasına gömülü değerlendirme bölümü: özet (ortalama puan +
/// toplam sayı), yorum listesi ve (oturum açmış kullanıcılar için, şu an
/// kapalı) yorum yazma formu. Düzenleme/silme bu aşamanın kapsamı dışıdır.
///
/// `sellableItemId`/`sellableItemType`, kullanıcının fiilen satın aldığı (ya
/// da alacağı) kalemi belirtmelidir — backend değerlendirmeleri her zaman
/// üst ürüne değil, tam olarak bu ikiliye bağlar (bkz.
/// `Review.Domain.Entities.ProductReview`).
class ReviewSection extends ConsumerWidget {
  const ReviewSection({
    super.key,
    required this.sellableItemId,
    required this.sellableItemType,
  });

  final String sellableItemId;
  final ReviewItemType sellableItemType;

  ReviewTarget get _target => (
    sellableItemId: sellableItemId,
    sellableItemType: sellableItemType,
  );

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final summary = ref.watch(productReviewSummaryProvider(_target));
    final reviews = ref.watch(productReviewsProvider(_target));
    final isLoggedIn = ref.watch(isReviewerLoggedInProvider).value;
    // Ortalama puan zaten ürün başlığının altında gösteriliyor (bkz.
    // `_ReviewSummaryRow` in `product_detail_page.dart`) — web'in kendisi de
    // aynı bilgiyi hem orada hem burada (başlıkta sayı olarak) tekrarlıyor.
    final totalCount = switch (summary.value) {
      Success<ReviewSummary>(:final value) => value.totalCount,
      _ => 0,
    };

    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        const Divider(height: 32),
        Text(
          totalCount > 0 ? 'Değerlendirmeler ($totalCount)' : 'Değerlendirmeler',
          style: Theme.of(context).textTheme.titleMedium,
        ),
        const SizedBox(height: 12),
        reviews.when(
          loading: () => const Padding(
            padding: EdgeInsets.symmetric(vertical: 16),
            child: LoadingView(message: 'Değerlendirmeler yükleniyor...'),
          ),
          error: (_, _) => ErrorView(
            message: 'Değerlendirmeler yüklenemedi.',
            onRetry: () => ref.invalidate(productReviewsProvider(_target)),
          ),
          data: (result) => switch (result) {
            Success<List<Review>>(:final value) => value.isEmpty
                ? const EmptyView(
                    message: 'Bu ürün için henüz değerlendirme yapılmamış.',
                  )
                : _ReviewList(reviews: value),
            ResultFailure<List<Review>>(:final failure) => ErrorView(
              message: failure.message,
              onRetry: () => ref.invalidate(productReviewsProvider(_target)),
            ),
          },
        ),
        if (_showCreateReviewForm) ...[
          const SizedBox(height: 16),
          if (isLoggedIn == true)
            _ExpandableReviewForm(target: _target)
          else if (isLoggedIn == false)
            Text(
              'Değerlendirme yazmak için giriş yapmalısınız.',
              style: Theme.of(context).textTheme.bodySmall,
            ),
        ],
      ],
    );
  }
}

/// "Değerlendirme yaz" alanını açılır/kapanır yapar — form varsayılan
/// olarak kapalı, sadece kompakt bir buton görünür; kullanıcı tıklayınca
/// `_CreateReviewForm` açılır. Gönderim akışı (`_CreateReviewFormState._submit`)
/// hiç değişmedi — sadece başarılı gönderimden sonra ve "Vazgeç" ile tekrar
/// kapanabiliyor.
class _ExpandableReviewForm extends StatefulWidget {
  const _ExpandableReviewForm({required this.target});
  final ReviewTarget target;

  @override
  State<_ExpandableReviewForm> createState() => _ExpandableReviewFormState();
}

class _ExpandableReviewFormState extends State<_ExpandableReviewForm> {
  bool _expanded = false;

  @override
  Widget build(BuildContext context) {
    if (!_expanded) {
      return Align(
        alignment: Alignment.centerLeft,
        child: OutlinedButton.icon(
          onPressed: () => setState(() => _expanded = true),
          icon: const Icon(Icons.rate_review_outlined, size: 18),
          label: const Text('Değerlendirme Yaz'),
        ),
      );
    }
    return _CreateReviewForm(
      target: widget.target,
      onCollapse: () => setState(() => _expanded = false),
    );
  }
}

class _CreateReviewForm extends ConsumerStatefulWidget {
  const _CreateReviewForm({required this.target, required this.onCollapse});
  final ReviewTarget target;
  final VoidCallback onCollapse;

  @override
  ConsumerState<_CreateReviewForm> createState() => _CreateReviewFormState();
}

class _CreateReviewFormState extends ConsumerState<_CreateReviewForm> {
  final _formKey = GlobalKey<FormState>();
  final _commentController = TextEditingController();

  /// `0` = henüz bir puan seçilmedi. Kullanıcı bir yıldıza dokunana kadar
  /// hiçbir yıldız dolu gösterilmez ve gönderim engellenir.
  int _rating = 0;
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
    final result = await ref.read(createReviewUseCaseProvider)(
      sellableItemId: widget.target.sellableItemId,
      sellableItemType: widget.target.sellableItemType,
      rating: _rating,
      comment: _commentController.text.trim(),
    );
    if (!mounted) return;
    setState(() => _isSubmitting = false);

    switch (result) {
      case Success<String>():
        // Yeni yorum eklendi: özet ve liste güncel veriyi göstersin diye
        // yeniden yüklenmeye zorlanır (bkz. `OrderDetailPage`'deki aynı desen).
        ref.invalidate(productReviewSummaryProvider(widget.target));
        ref.invalidate(productReviewsProvider(widget.target));
        _commentController.clear();
        setState(() => _rating = 0);
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(content: Text('Değerlendirmeniz eklendi.')),
        );
        widget.onCollapse();
      case ResultFailure<String>(:final failure):
        ScaffoldMessenger.of(
          context,
        ).showSnackBar(SnackBar(content: Text(failure.message)));
    }
  }

  @override
  Widget build(BuildContext context) => Form(
    key: _formKey,
    child: Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        const Divider(height: 24),
        Text(
          'Değerlendirme yaz',
          style: Theme.of(context).textTheme.labelLarge,
        ),
        const SizedBox(height: 8),
        StarRatingInput(
          rating: _rating,
          onChanged: (value) => setState(() => _rating = value),
        ),
        const SizedBox(height: 8),
        TextFormField(
          controller: _commentController,
          maxLines: 2,
          maxLength: 2000,
          style: Theme.of(context).textTheme.bodyMedium,
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
        Row(
          mainAxisAlignment: MainAxisAlignment.end,
          children: [
            TextButton(
              onPressed: _isSubmitting ? null : widget.onCollapse,
              child: const Text('Vazgeç'),
            ),
            const SizedBox(width: 8),
            FilledButton(
              // Tema `minimumSize: Size.fromHeight(52)` tanımlıyor (genişlik
              // = double.infinity) — bu, doğrudan bir stretched `Column`
              // çocuğu (eski tek buton düzeni) için sorun değildi, ama bir
              // `Row` içinde (flex olmayan çocuklara `maxWidth: infinity`
              // verir) sonsuz genişlik layout hatasına yol açıyordu. Burada
              // sonlu bir minimum boyutla geçersiz kılınıyor.
              style: FilledButton.styleFrom(minimumSize: const Size(64, 40)),
              onPressed: _isSubmitting ? null : _submit,
              child: _isSubmitting
                  ? const SizedBox(
                      height: 16,
                      width: 16,
                      child: CircularProgressIndicator(strokeWidth: 2),
                    )
                  : const Text('Gönder'),
            ),
          ],
        ),
      ],
    ),
  );
}

class _ReviewList extends StatelessWidget {
  const _ReviewList({required this.reviews});
  final List<Review> reviews;

  @override
  Widget build(BuildContext context) => Column(
    crossAxisAlignment: CrossAxisAlignment.stretch,
    children: [
      for (final review in reviews) ...[
        _ReviewTile(review: review),
        if (review != reviews.last) const SizedBox(height: 12),
      ],
    ],
  );
}

class _ReviewTile extends StatelessWidget {
  const _ReviewTile({required this.review});
  final Review review;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final name = review.reviewerDisplayName ?? 'Anonim Kullanıcı';
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        border: Border.all(color: theme.colorScheme.outlineVariant),
        borderRadius: BorderRadius.circular(AppRadius.md),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              CircleAvatar(
                radius: 16,
                backgroundColor: theme.colorScheme.surfaceContainerHighest,
                child: Text(
                  name.isNotEmpty ? name[0].toUpperCase() : '?',
                  style: theme.textTheme.labelLarge,
                ),
              ),
              const SizedBox(width: 12),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      name,
                      style: theme.textTheme.bodyMedium?.copyWith(
                        fontWeight: FontWeight.w600,
                      ),
                    ),
                    const SizedBox(height: 2),
                    _StarRow(rating: review.rating),
                  ],
                ),
              ),
              Text(
                _dateFormat.format(review.createdAt),
                style: theme.textTheme.bodySmall,
              ),
            ],
          ),
          const SizedBox(height: 12),
          Text(review.comment, style: theme.textTheme.bodyMedium),
        ],
      ),
    );
  }
}

class _StarRow extends StatelessWidget {
  const _StarRow({required this.rating});
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
