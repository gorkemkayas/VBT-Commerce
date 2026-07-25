import '../../../../core/utils/result.dart';
import '../repositories/review_repository.dart';

class DeleteReviewUseCase {
  const DeleteReviewUseCase(this._repository);
  final ReviewRepository _repository;

  Future<Result<bool>> call(String reviewId) => _repository.deleteReview(reviewId);
}
