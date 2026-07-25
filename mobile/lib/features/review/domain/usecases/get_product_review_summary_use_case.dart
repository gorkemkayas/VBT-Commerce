import '../../../../core/utils/result.dart';
import '../entities/review_item_type.dart';
import '../entities/review_summary.dart';
import '../repositories/review_repository.dart';

class GetProductReviewSummaryUseCase {
  const GetProductReviewSummaryUseCase(this._repository);
  final ReviewRepository _repository;

  Future<Result<ReviewSummary>> call(
    String sellableItemId,
    ReviewItemType sellableItemType,
  ) => _repository.getSummary(sellableItemId, sellableItemType);
}
