import 'package:dio/dio.dart';

import '../../domain/entities/review_item_type.dart';
import '../models/review_model.dart';
import '../models/review_summary_model.dart';

abstract interface class ReviewRemoteDataSource {
  Future<ReviewSummaryModel> getSummary(
    String sellableItemId,
    ReviewItemType sellableItemType,
  );
  Future<List<ReviewModel>> getReviews(
    String sellableItemId,
    ReviewItemType sellableItemType,
  );

  /// `POST /api/reviews/me` — oturum gerektirir (`MyReviewsController`).
  Future<String> createReview({
    required String sellableItemId,
    required ReviewItemType sellableItemType,
    required int rating,
    required String comment,
  });
}

/// Okuma uçları herkese açıktır (bkz. `ReviewsController`) — auth
/// gerektirmez. Yazma (`createReview`) `MyReviewsController` altında,
/// oturum gerektirir.
class ReviewRemoteDataSourceImpl implements ReviewRemoteDataSource {
  ReviewRemoteDataSourceImpl(this._dio);
  final Dio _dio;

  static const _pageSize = 20;

  @override
  Future<ReviewSummaryModel> getSummary(
    String sellableItemId,
    ReviewItemType sellableItemType,
  ) async {
    final response = await _dio.get<Map<String, dynamic>>(
      '/api/reviews/summary',
      queryParameters: {
        'sellableItemId': sellableItemId,
        'sellableItemType': sellableItemType.toJson(),
      },
    );
    final body = response.data;
    if (body == null) {
      throw const FormatException(
        'Sunucudan boş değerlendirme özeti alındı.',
      );
    }
    return ReviewSummaryModel.fromJson(body);
  }

  @override
  Future<List<ReviewModel>> getReviews(
    String sellableItemId,
    ReviewItemType sellableItemType,
  ) async {
    final response = await _dio.get<Map<String, dynamic>>(
      '/api/reviews',
      queryParameters: {
        'sellableItemId': sellableItemId,
        'sellableItemType': sellableItemType.toJson(),
        'pageNumber': 1,
        'pageSize': _pageSize,
      },
    );
    final body = response.data;
    if (body == null) {
      throw const FormatException(
        'Sunucudan boş değerlendirme listesi alındı.',
      );
    }
    final items = body['items'];
    if (items is! List) {
      throw const FormatException(
        'Değerlendirme listesi yanıtı beklenen şekilde değil.',
      );
    }
    return items
        .map((item) => ReviewModel.fromJson(item as Map<String, dynamic>))
        .toList(growable: false);
  }

  @override
  Future<String> createReview({
    required String sellableItemId,
    required ReviewItemType sellableItemType,
    required int rating,
    required String comment,
  }) async {
    final response = await _dio.post<dynamic>(
      '/api/reviews/me',
      data: {
        'sellableItemId': sellableItemId,
        'sellableItemType': sellableItemType.toJson(),
        'rating': rating,
        'comment': comment,
      },
    );
    final id = response.data;
    if (id is! String || id.isEmpty) {
      throw const FormatException(
        'Değerlendirme eklenirken sunucudan geçersiz yanıt alındı.',
      );
    }
    return id;
  }
}
