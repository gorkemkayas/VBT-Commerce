import '../../../../core/utils/result.dart';
import '../entities/category.dart';
import '../entities/product.dart';
import '../entities/product_filter.dart';

abstract interface class ProductRepository {
  Future<Result<List<Product>>> getProducts();
  Future<Result<Product>> getProductDetail(String id);

  /// Ürünü SEO slug'ıyla getirir (`GET /api/products/by-slug/{slug}`).
  /// [getProductDetail] ile aynı ürünü döner; yalnızca arama anahtarı farklıdır.
  Future<Result<Product>> getProductDetailBySlug(String slug);

  /// Seçili bir varyantın anlık fiyatını sorgular. `Product.price` doldurulan
  /// varsayılan (ilk varyant) fiyattan farklı bir varyant seçildiğinde
  /// kullanılır.
  Future<Result<double?>> getVariantPrice(String variantId);
  Future<Result<List<Category>>> getCategories();

  /// Tek bir kategorinin tam kaydı (`GET /api/categories/{categoryId}`).
  Future<Result<CategoryDetail>> getCategoryDetail(String id);
  Future<Result<List<Product>>> searchProducts(String query);
  Future<Result<List<Product>>> filterProducts(ProductFilter filter);
}
