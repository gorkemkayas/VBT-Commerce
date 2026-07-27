import '../../../../core/errors/failure.dart';
import '../../../../core/utils/result.dart';
import '../entities/product.dart';
import '../repositories/product_repository.dart';

/// Ürünü SEO slug'ıyla getirir — `/product/slug/{slug}` derin bağlantısıyla
/// açılan ürün detayı bunu kullanır (bkz. `GetProductDetailUseCase`, aynı
/// ürünü id ile getirir).
class GetProductDetailBySlugUseCase {
  const GetProductDetailBySlugUseCase(this._repository);
  final ProductRepository _repository;

  Future<Result<Product>> call(String slug) {
    final trimmed = slug.trim();
    if (trimmed.isEmpty) {
      return Future.value(
        const Result.failure(ValidationFailure('Geçersiz ürün bağlantısı.')),
      );
    }
    return _repository.getProductDetailBySlug(trimmed);
  }
}
