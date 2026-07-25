import '../../../../core/utils/result.dart';
import '../entities/review.dart';
import '../entities/review_item_type.dart';
import '../repositories/review_repository.dart';

class GetProductReviewsUseCase {
  const GetProductReviewsUseCase(this._repository);
  final ReviewRepository _repository;

  Future<Result<List<Review>>> call(
    String sellableItemId,
    ReviewItemType sellableItemType,
  ) => _repository.getReviews(sellableItemId, sellableItemType);
}
