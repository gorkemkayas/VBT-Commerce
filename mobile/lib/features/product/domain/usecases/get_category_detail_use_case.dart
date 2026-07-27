import '../../../../core/errors/failure.dart';
import '../../../../core/utils/result.dart';
import '../entities/category.dart';
import '../repositories/product_repository.dart';

/// Tek bir kategoriyi id'siyle getirir. Ürün detayındaki kategori rozeti,
/// ürünün `categoryId`'sini bununla ada çevirir — kategori ağacında
/// (`GetCategoriesUseCase`) bulunmayan pasif kategorilerde de çalışır.
class GetCategoryDetailUseCase {
  const GetCategoryDetailUseCase(this._repository);
  final ProductRepository _repository;

  Future<Result<CategoryDetail>> call(String id) {
    if (id.isEmpty) {
      return Future.value(
        const Result.failure(ValidationFailure('Geçersiz kategori kimliği.')),
      );
    }
    return _repository.getCategoryDetail(id);
  }
}
