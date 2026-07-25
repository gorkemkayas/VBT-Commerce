import 'package:dio/dio.dart';

import '../../../../core/errors/failure.dart';
import '../../../../core/network/network_error_mapper.dart';
import '../../../../core/utils/result.dart';
import '../../domain/entities/review.dart';
import '../../domain/entities/review_item_type.dart';
import '../../domain/entities/review_summary.dart';
import '../../domain/repositories/review_repository.dart';
import '../datasources/review_remote_data_source.dart';

class ReviewRepositoryImpl implements ReviewRepository {
  ReviewRepositoryImpl(this._remoteDataSource);
  final ReviewRemoteDataSource _remoteDataSource;

  @override
  Future<Result<ReviewSummary>> getSummary(
    String sellableItemId,
    ReviewItemType sellableItemType,
  ) async {
    try {
      return Result.success(
        await _remoteDataSource.getSummary(sellableItemId, sellableItemType),
      );
    } on DioException catch (error) {
      return Result.failure(mapDioException(error));
    } on FormatException catch (error) {
      return Result.failure(ServerFailure(error.message));
    } catch (_) {
      return const Result.failure(
        UnknownFailure(
          'Değerlendirme özeti alınırken beklenmeyen bir hata oluştu.',
        ),
      );
    }
  }

  @override
  Future<Result<List<Review>>> getReviews(
    String sellableItemId,
    ReviewItemType sellableItemType,
  ) async {
    try {
      return Result.success(
        await _remoteDataSource.getReviews(sellableItemId, sellableItemType),
      );
    } on DioException catch (error) {
      return Result.failure(mapDioException(error));
    } on FormatException catch (error) {
      return Result.failure(ServerFailure(error.message));
    } catch (_) {
      return const Result.failure(
        UnknownFailure(
          'Değerlendirmeler alınırken beklenmeyen bir hata oluştu.',
        ),
      );
    }
  }

  @override
  Future<Result<String>> createReview({
    required String sellableItemId,
    required ReviewItemType sellableItemType,
    required int rating,
    required String comment,
  }) async {
    try {
      return Result.success(
        await _remoteDataSource.createReview(
          sellableItemId: sellableItemId,
          sellableItemType: sellableItemType,
          rating: rating,
          comment: comment,
        ),
      );
    } on DioException catch (error) {
      return Result.failure(mapDioException(error));
    } on FormatException catch (error) {
      return Result.failure(ServerFailure(error.message));
    } catch (_) {
      return const Result.failure(
        UnknownFailure(
          'Değerlendirme eklenirken beklenmeyen bir hata oluştu.',
        ),
      );
    }
  }

  @override
  Future<Result<List<Review>>> getMyReviews() async {
    try {
      return Result.success(await _remoteDataSource.getMyReviews());
    } on DioException catch (error) {
      return Result.failure(mapDioException(error));
    } on FormatException catch (error) {
      return Result.failure(ServerFailure(error.message));
    } catch (_) {
      return const Result.failure(
        UnknownFailure('Yorumlarınız alınırken beklenmeyen bir hata oluştu.'),
      );
    }
  }

  @override
  Future<Result<bool>> updateReview({
    required String reviewId,
    required int rating,
    required String comment,
  }) async {
    try {
      await _remoteDataSource.updateReview(
        reviewId: reviewId,
        rating: rating,
        comment: comment,
      );
      return const Result.success(true);
    } on DioException catch (error) {
      return Result.failure(mapDioException(error));
    } catch (_) {
      return const Result.failure(
        UnknownFailure('Yorum güncellenirken beklenmeyen bir hata oluştu.'),
      );
    }
  }

  @override
  Future<Result<bool>> deleteReview(String reviewId) async {
    try {
      await _remoteDataSource.deleteReview(reviewId);
      return const Result.success(true);
    } on DioException catch (error) {
      return Result.failure(mapDioException(error));
    } catch (_) {
      return const Result.failure(
        UnknownFailure('Yorum silinirken beklenmeyen bir hata oluştu.'),
      );
    }
  }
}
