import '../../../../core/utils/result.dart';
import '../repositories/review_repository.dart';

class UpdateReviewUseCase {
  const UpdateReviewUseCase(this._repository);
  final ReviewRepository _repository;

  Future<Result<bool>> call({
    required String reviewId,
    required int rating,
    required String comment,
  }) => _repository.updateReview(
    reviewId: reviewId,
    rating: rating,
    comment: comment,
  );
}
