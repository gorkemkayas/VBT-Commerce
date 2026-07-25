import '../../../../core/utils/result.dart';
import '../entities/review.dart';
import '../repositories/review_repository.dart';

class GetMyReviewsUseCase {
  const GetMyReviewsUseCase(this._repository);
  final ReviewRepository _repository;

  Future<Result<List<Review>>> call() => _repository.getMyReviews();
}
