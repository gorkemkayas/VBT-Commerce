import '../../domain/entities/review.dart';
import '../../domain/entities/review_item_type.dart';

/// `GET /api/reviews` yanıtındaki `ReviewDto`'ya karşılık gelir (bkz.
/// `Review.Application.Common.ReviewDto`).
class ReviewModel extends Review {
  const ReviewModel({
    required super.id,
    required super.userId,
    required super.sellableItemId,
    required super.sellableItemType,
    required super.rating,
    required super.comment,
    required super.createdAt,
    super.updatedAt,
    super.reviewerDisplayName,
  });

  factory ReviewModel.fromJson(Map<String, dynamic> json) => ReviewModel(
    id: json['id'] as String,
    userId: json['userId'] as String,
    sellableItemId: json['sellableItemId'] as String,
    // Enum'lar backend'de string olarak serileştirilir (JsonStringEnumConverter).
    sellableItemType: reviewItemTypeFromJson(
      json['sellableItemType'] as String? ?? '',
    ),
    rating: (json['rating'] as num?)?.toInt() ?? 0,
    comment: json['comment'] as String? ?? '',
    createdAt:
        DateTime.tryParse(json['createdAt'] as String? ?? '')?.toLocal() ??
        DateTime.now(),
    updatedAt: (json['updatedAt'] as String?) == null
        ? null
        : DateTime.tryParse(json['updatedAt'] as String)?.toLocal(),
    reviewerDisplayName: json['reviewerDisplayName'] as String?,
  );
}
