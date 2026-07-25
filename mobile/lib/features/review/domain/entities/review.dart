import 'review_item_type.dart';

class Review {
  const Review({
    required this.id,
    required this.userId,
    required this.sellableItemId,
    required this.sellableItemType,
    required this.rating,
    required this.comment,
    required this.createdAt,
    this.updatedAt,
  });

  final String id;
  final String userId;
  final String sellableItemId;
  final ReviewItemType sellableItemType;
  final int rating;
  final String comment;
  final DateTime createdAt;
  final DateTime? updatedAt;
}
