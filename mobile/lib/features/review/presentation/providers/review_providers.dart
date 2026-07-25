import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/constants/storage_keys.dart';
import '../../../../core/network/dio_client.dart';
import '../../../../core/services/secure_storage_service.dart';
import '../../../../core/utils/result.dart';
import '../../data/datasources/review_remote_data_source.dart';
import '../../data/repositories/review_repository_impl.dart';
import '../../domain/entities/review.dart';
import '../../domain/entities/review_item_type.dart';
import '../../domain/entities/review_summary.dart';
import '../../domain/repositories/review_repository.dart';
import '../../domain/usecases/create_review_use_case.dart';
import '../../domain/usecases/get_product_review_summary_use_case.dart';
import '../../domain/usecases/get_product_reviews_use_case.dart';

final reviewRemoteDataSourceProvider = Provider<ReviewRemoteDataSource>(
  (ref) => ReviewRemoteDataSourceImpl(ref.watch(dioProvider)),
);
final reviewRepositoryProvider = Provider<ReviewRepository>(
  (ref) => ReviewRepositoryImpl(ref.watch(reviewRemoteDataSourceProvider)),
);
final getProductReviewSummaryUseCaseProvider =
    Provider<GetProductReviewSummaryUseCase>(
      (ref) =>
          GetProductReviewSummaryUseCase(ref.watch(reviewRepositoryProvider)),
    );
final getProductReviewsUseCaseProvider = Provider<GetProductReviewsUseCase>(
  (ref) => GetProductReviewsUseCase(ref.watch(reviewRepositoryProvider)),
);
final createReviewUseCaseProvider = Provider<CreateReviewUseCase>(
  (ref) => CreateReviewUseCase(ref.watch(reviewRepositoryProvider)),
);

/// Yorum formunu göstermeden önce oturum durumunu kontrol eder. Backend
/// `/api/reviews/me` oturum gerektiriyor (`IRequireRole`); token yokken
/// çağrılırsa 401 döner ve `AuthInterceptor` kullanıcıyı zorla `/login`'e
/// yönlendirir (checkout'ta karşılaşılan aynı risk) — bu yüzden form, giriş
/// yapılmadıysa hiç gösterilmez. Checkout feature'ına bağımlılık kurmamak
/// için burada kasıtlı olarak (küçük bir tekrar ile) ayrıca tanımlanır.
final isReviewerLoggedInProvider = FutureProvider.autoDispose<bool>((
  ref,
) async {
  final token = await ref
      .watch(secureStorageServiceProvider)
      .getString(StorageKeys.accessToken);
  return token != null;
});

/// Bir ürün/varyant için (id, tür) ikilisini taşır. `FutureProvider.family`
/// tek parametre aldığından özet ve liste sorguları bu anahtarla
/// parametrize edilir. Dart record'ları yapısal eşitlik sağladığından
/// family key olarak doğrudan kullanılabilir.
typedef ReviewTarget = ({String sellableItemId, ReviewItemType sellableItemType});

/// Ürün detay ekranı bunu izler; `productDetailProvider` ile aynı desen
/// (autoDispose family).
final productReviewSummaryProvider = FutureProvider.autoDispose
    .family<Result<ReviewSummary>, ReviewTarget>(
      (ref, target) => ref.watch(getProductReviewSummaryUseCaseProvider)(
        target.sellableItemId,
        target.sellableItemType,
      ),
    );

final productReviewsProvider = FutureProvider.autoDispose
    .family<Result<List<Review>>, ReviewTarget>(
      (ref, target) => ref.watch(getProductReviewsUseCaseProvider)(
        target.sellableItemId,
        target.sellableItemType,
      ),
    );
