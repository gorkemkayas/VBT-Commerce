import '../../domain/entities/review_item_type.dart';
import '../../domain/entities/review_summary.dart';

/// `GET /api/reviews/summary` yanıtındaki `ReviewSummaryDto`'ya karşılık gelir
/// (bkz. `Review.Application.Common.ReviewSummaryDto`).
class ReviewSummaryModel extends ReviewSummary {
  const ReviewSummaryModel({
    required super.sellableItemId,
    required super.sellableItemType,
    required super.averageRating,
    required super.totalCount,
  });

  factory ReviewSummaryModel.fromJson(Map<String, dynamic> json) =>
      ReviewSummaryModel(
        sellableItemId: json['sellableItemId'] as String? ?? '',
        sellableItemType: reviewItemTypeFromJson(
          json['sellableItemType'] as String? ?? '',
        ),
        averageRating: (json['averageRating'] as num?)?.toDouble() ?? 0,
        totalCount: (json['totalCount'] as num?)?.toInt() ?? 0,
      );
}
