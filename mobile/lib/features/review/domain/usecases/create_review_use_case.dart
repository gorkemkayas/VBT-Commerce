import '../../../../core/utils/result.dart';
import '../entities/review_item_type.dart';
import '../repositories/review_repository.dart';

class CreateReviewUseCase {
  const CreateReviewUseCase(this._repository);
  final ReviewRepository _repository;

  Future<Result<String>> call({
    required String sellableItemId,
    required ReviewItemType sellableItemType,
    required int rating,
    required String comment,
  }) => _repository.createReview(
    sellableItemId: sellableItemId,
    sellableItemType: sellableItemType,
    rating: rating,
    comment: comment,
  );
}
