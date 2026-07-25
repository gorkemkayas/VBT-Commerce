import 'review_item_type.dart';

class ReviewSummary {
  const ReviewSummary({
    required this.sellableItemId,
    required this.sellableItemType,
    required this.averageRating,
    required this.totalCount,
  });

  final String sellableItemId;
  final ReviewItemType sellableItemType;
  final double averageRating;
  final int totalCount;
}
