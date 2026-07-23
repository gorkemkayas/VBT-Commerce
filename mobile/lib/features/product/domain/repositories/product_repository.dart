import '../../../../core/utils/result.dart';
import '../entities/category.dart';
import '../entities/product.dart';
import '../entities/product_filter.dart';

abstract interface class ProductRepository {
  Future<Result<List<Product>>> getProducts();
  Future<Result<Product>> getProductDetail(String id);

  /// Seçili bir varyantın anlık fiyatını sorgular. `Product.price` doldurulan
  /// varsayılan (ilk varyant) fiyattan farklı bir varyant seçildiğinde
  /// kullanılır.
  Future<Result<double?>> getVariantPrice(String variantId);
  Future<Result<List<Category>>> getCategories();
  Future<Result<List<Product>>> searchProducts(String query);
  Future<Result<List<Product>>> filterProducts(ProductFilter filter);
}
