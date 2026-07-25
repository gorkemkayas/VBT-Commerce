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

  /// `GET /api/reviews/me` — oturum gerektirir.
  Future<List<ReviewModel>> getMyReviews();

  /// `PUT /api/reviews/me/{reviewId}` — oturum gerektirir.
  Future<void> updateReview({
    required String reviewId,
    required int rating,
    required String comment,
  });

  /// `DELETE /api/reviews/me/{reviewId}` — oturum gerektirir.
  Future<void> deleteReview(String reviewId);
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

  /// Sayfalama UI'ı henüz yok (bkz. `OrderRemoteDataSourceImpl`'daki aynı
  /// yaklaşım) — ilk sayfa yeterince büyük bir `pageSize` ile çekilir.
  @override
  Future<List<ReviewModel>> getMyReviews() async {
    final response = await _dio.get<Map<String, dynamic>>(
      '/api/reviews/me',
      queryParameters: {'pageNumber': 1, 'pageSize': 50},
    );
    final body = response.data;
    if (body == null) {
      throw const FormatException('Sunucudan boş yorum listesi alındı.');
    }
    final items = body['items'];
    if (items is! List) {
      throw const FormatException(
        'Yorum listesi yanıtı beklenen şekilde değil.',
      );
    }
    return items
        .map((item) => ReviewModel.fromJson(item as Map<String, dynamic>))
        .toList(growable: false);
  }

  @override
  Future<void> updateReview({
    required String reviewId,
    required int rating,
    required String comment,
  }) async {
    await _dio.put<void>(
      '/api/reviews/me/$reviewId',
      data: {'rating': rating, 'comment': comment},
    );
  }

  @override
  Future<void> deleteReview(String reviewId) async {
    await _dio.delete<void>('/api/reviews/me/$reviewId');
  }
}
