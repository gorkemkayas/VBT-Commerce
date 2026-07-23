import '../../../../core/errors/failure.dart';
import '../../../../core/utils/result.dart';
import '../repositories/product_repository.dart';

class GetVariantPriceUseCase {
  const GetVariantPriceUseCase(this._repository);
  final ProductRepository _repository;

  Future<Result<double?>> call(String variantId) {
    if (variantId.isEmpty) {
      return Future.value(
        const Result.failure(ValidationFailure('Geçersiz varyant kimliği.')),
      );
    }
    return _repository.getVariantPrice(variantId);
  }
}
