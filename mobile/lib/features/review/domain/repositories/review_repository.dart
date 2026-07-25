import '../../../../core/utils/result.dart';
import '../entities/review.dart';
import '../entities/review_item_type.dart';
import '../entities/review_summary.dart';

abstract interface class ReviewRepository {
  /// `GET /api/reviews/summary` — herkese açık, auth gerektirmez.
  Future<Result<ReviewSummary>> getSummary(
    String sellableItemId,
    ReviewItemType sellableItemType,
  );

  /// `GET /api/reviews` — herkese açık, auth gerektirmez.
  Future<Result<List<Review>>> getReviews(
    String sellableItemId,
    ReviewItemType sellableItemType,
  );

  /// `POST /api/reviews/me` — oturum (Customer/Admin rolü) ve satın alma
  /// kontrolü backend'de yapılır (`CreateMyReviewCommandHandler`); burada
  /// tekrarlanmaz.
  Future<Result<String>> createReview({
    required String sellableItemId,
    required ReviewItemType sellableItemType,
    required int rating,
    required String comment,
  });

  /// `GET /api/reviews/me` — giriş yapmış kullanıcının kendi yorumları.
  Future<Result<List<Review>>> getMyReviews();

  /// `PUT /api/reviews/me/{reviewId}` — sahiplik kontrolü backend'de yapılır
  /// (`UpdateMyReviewCommandHandler`); burada tekrarlanmaz.
  Future<Result<bool>> updateReview({
    required String reviewId,
    required int rating,
    required String comment,
  });

  /// `DELETE /api/reviews/me/{reviewId}` — sahiplik kontrolü backend'de
  /// yapılır (`DeleteMyReviewCommandHandler`); burada tekrarlanmaz.
  Future<Result<bool>> deleteReview(String reviewId);
}
