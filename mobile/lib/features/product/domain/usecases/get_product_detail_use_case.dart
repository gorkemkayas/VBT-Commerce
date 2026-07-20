import '../../../../core/errors/failure.dart';
import '../../../../core/utils/result.dart';
import '../entities/product.dart';
import '../repositories/product_repository.dart';

class GetProductDetailUseCase {
  const GetProductDetailUseCase(this._repository);
  final ProductRepository _repository;

  Future<Result<Product>> call(String id) {
    if (id.isEmpty) {
      return Future.value(
        const Result.failure(ValidationFailure('Geçersiz ürün kimliği.')),
      );
    }
    return _repository.getProductDetail(id);
  }
}
