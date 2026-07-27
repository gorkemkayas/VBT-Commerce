import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/constants/route_paths.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_motion.dart';
import '../../../../core/utils/currency_formatter.dart';
import '../../../../core/utils/result.dart';
import '../../../../core/widgets/app_network_image.dart';
import '../../../../core/widgets/async_state_views.dart';
import '../../../cart/presentation/providers/cart_providers.dart';
import '../../../cart/presentation/widgets/cart_icon_button.dart';
import '../../../favorites/domain/entities/favorite_item.dart';
import '../../../favorites/presentation/widgets/favorite_button.dart';
import '../../../review/domain/entities/review_item_type.dart';
import '../../../review/domain/entities/review_summary.dart';
import '../../../review/presentation/providers/review_providers.dart';
import '../../../review/presentation/widgets/review_section.dart';
import '../../domain/entities/category.dart';
import '../../domain/entities/product.dart';
import '../../domain/entities/product_variant.dart';
import '../providers/product_providers.dart';

class ProductDetailPage extends ConsumerWidget {
  const ProductDetailPage({super.key, required this.productId}) : slug = null;

  /// Ürünü id yerine SEO slug'ıyla açar (`/product/slug/:slug` derin
  /// bağlantısı). Ekranın geri kalanı değişmez; yalnızca ürünü getiren
  /// provider farklıdır.
  const ProductDetailPage.bySlug({super.key, required String this.slug})
    : productId = '';

  final String productId;
  final String? slug;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final slug = this.slug;
    final detailProvider = slug == null
        ? productDetailProvider(productId)
        : productDetailBySlugProvider(slug);
    final product = ref.watch(detailProvider);
    return Scaffold(
      appBar: AppBar(
        title: const Text('Ürün detayı'),
        actions: [
          // Sepet ikonunun solunda Hesabım kısayolu — sekme çubuğu bu
          // sayfada görünmediği için. Giriş kontrolü gerekmez: `AccountPage`
          // misafirken kendi (giriş/kayıt + sipariş sorgulama) görünümünü
          // gösteriyor.
          IconButton(
            icon: const Icon(Icons.person_outline),
            tooltip: 'Hesabım',
            onPressed: () => context.push(RoutePaths.account),
          ),
          const CartIconButton(),
        ],
      ),
      body: product.when(
        loading: () => const LoadingView(message: 'Ürün detayı yükleniyor...'),
        error: (_, _) => ErrorView(
          message: 'Ürün detayı yüklenemedi.',
          onRetry: () => ref.invalidate(detailProvider),
        ),
        data: (result) => switch (result) {
          Success<Product>(:final value) => _ProductDetail(product: value),
          ResultFailure<Product>(:final failure) => ErrorView(
            message: failure.message,
            onRetry: () => ref.invalidate(detailProvider),
          ),
        },
      ),
    );
  }
}

class _ProductDetail extends ConsumerStatefulWidget {
  const _ProductDetail({required this.product});
  final Product product;

  @override
  ConsumerState<_ProductDetail> createState() => _ProductDetailState();
}

class _ProductDetailState extends ConsumerState<_ProductDetail> {
  bool _isAddingToCart = false;

  /// Kullanıcının seçtiği renk/beden — ham varyant id'si değil, tekilleşmiş
  /// öznitelik değeri. Gerçek varyant id'si (`_selectedVariantId`) bu
  /// ikisinden türetilir (bkz. aşağısı).
  String? _selectedColor;
  String? _selectedSize;

  @override
  void initState() {
    super.initState();
    // Tek varyant varsa (gerçek bir seçim yok) rengi/bedeni baştan seçili
    // göster — eski davranışla aynı (`variants.length == 1` durumu).
    if (widget.product.variants.length == 1) {
      final only = widget.product.variants.first;
      _selectedColor = only.color.isEmpty ? null : only.color;
      _selectedSize = only.size.isEmpty ? null : only.size;
    }
  }

  bool get _hasVariants => widget.product.variants.isNotEmpty;

  /// Varyantlar yalnızca gerçekten "Beden" özniteliğine sahipse (bkz.
  /// `ProductDetailModel._variantsFromJson` — yalnızca `attributeName ==
  /// 'Beden'` olan seçenek `size`'a yazılır) beden seçimi anlamlıdır. Kıyafet
  /// olmayan varyantlı ürünlerde (ör. renk seçenekli ürünler) `size` boş
  /// string olarak gelir — bu durumda "Beden" bölümü hiç gösterilmez.
  bool get _hasSizeVariants =>
      widget.product.variants.any((variant) => variant.size.isNotEmpty);

  /// Aynı mantık renk için: yalnızca gerçekten "Renk" değeri olan
  /// varyantlarda renk seçimi gösterilir.
  bool get _hasColorVariants =>
      widget.product.variants.any((variant) => variant.color.isNotEmpty);

  /// Ürünün benzersiz renkleri — aynı renk birden fazla varyantta (farklı
  /// bedenlerde) tekrar ediyorsa burada yalnızca bir kez yer alır.
  List<String> get _uniqueColors {
    final seen = <String>{};
    return [
      for (final variant in widget.product.variants)
        if (variant.color.isNotEmpty && seen.add(variant.color)) variant.color,
    ];
  }

  /// Ürünün tüm benzersiz bedenleri (mantıksal sırada), seçili renkten
  /// bağımsız olarak — beden bölümü artık renk seçilmeden gizlenmiyor,
  /// her zaman tüm bedenler gösteriliyor. Hangilerinin seçili renkte
  /// gerçekten seçilebilir olduğu `_isSizeAvailable` ile ayrıca belirlenir.
  List<String> get _allSizes {
    final seen = <String>{};
    return [
      for (final variant in _sortedBySizeOrder(widget.product.variants))
        if (variant.size.isNotEmpty && seen.add(variant.size)) variant.size,
    ];
  }

  /// Bu beden, seçili renkte (bir renk seçildiyse) gerçekten bir varyant
  /// olarak var mı? Renk henüz seçilmediyse (ya da ürünün renk boyutu
  /// yoksa) her beden seçilebilir kabul edilir. Bu, "renk -> beden"
  /// ilişkisinin mevcut varyant verisinden (her kaydın hem rengi hem
  /// bedeni birlikte taşıması) doğrudan kurulduğu yer — geçersiz bir
  /// renk+beden kombinasyonu bu sayede hiç seçilemez (buton disabled olur).
  bool _isSizeAvailable(String size) {
    if (!_hasColorVariants || _selectedColor == null) return true;
    return widget.product.variants.any(
      (variant) => variant.size == size && variant.color == _selectedColor,
    );
  }

  /// Kullanıcının seçtiği renk+beden kombinasyonuna uyan gerçek varyant.
  /// Sepete eklenen/backend'e gönderilen id hâlâ budur — sadece kullanıcıya
  /// artık ham varyant listesi değil, tekilleşmiş seçenekler gösteriliyor.
  /// Ürünün hem renk hem beden boyutu varsa, ikisi de seçilmeden `null`
  /// kalır (sepete yanlış/eksik bir varyant eklenmesin diye kasıtlı).
  String? get _selectedVariantId {
    if (!_hasVariants) return null;
    for (final variant in widget.product.variants) {
      if (_hasColorVariants && variant.color != _selectedColor) continue;
      if (_hasSizeVariants && variant.size != _selectedSize) continue;
      return variant.id;
    }
    return null;
  }

  /// Galeri/fiyat önizlemesi için kullanılır — `_selectedVariantId`'nin
  /// aksine, kullanıcı henüz **tüm** boyutları seçmemiş olsa bile (ör.
  /// sadece renk seçip bedeni henüz seçmediyse) şimdiye kadar seçilenlere
  /// uyan İLK varyantı gösterir. Bu sayede bir renge dokunur dokunmaz
  /// ürün görseli/fiyatı hemen o renge geçer — beden seçimi tamamlanmasını
  /// beklemez. Sepete ekleme bundan ETKİLENMEZ, o hâlâ kesin eşleşme
  /// (`_selectedVariantId`) kullanır.
  String? get _previewVariantId {
    if (_selectedVariantId != null) return _selectedVariantId;
    if (!_hasVariants) return null;
    for (final variant in widget.product.variants) {
      if (_selectedColor != null && variant.color != _selectedColor) continue;
      if (_selectedSize != null && variant.size != _selectedSize) continue;
      return variant.id;
    }
    return _defaultVariantId;
  }

  /// Sepete eklenirken kullanılan renk/beden etiketi (ör. "Siyah, 42") —
  /// web'in sepette gösterdiği `variantLabel` ile aynı mantık: yalnızca
  /// seçili olan (renk/beden) parçalar, sırasıyla ve virgülle ayrılarak
  /// birleştirilir. Hiçbiri yoksa `null` — bu durumda cart/checkout hiçbir
  /// varyant bilgisi göstermez.
  String? get _selectedVariantLabel {
    if (!_hasVariants) return null;
    final parts = [
      if (_hasColorVariants && _selectedColor != null) _selectedColor!,
      if (_hasSizeVariants && _selectedSize != null) _selectedSize!,
    ];
    return parts.isEmpty ? null : parts.join(', ');
  }

  void _selectColor(String color) {
    setState(() {
      _selectedColor = color;
      // Yeni renkte artık mevcut olmayan bir beden seçiliyse sıfırla —
      // kullanıcı geçersiz bir renk+beden kombinasyonunda kalmasın.
      if (_selectedSize != null && !_isSizeAvailable(_selectedSize!)) {
        _selectedSize = null;
      }
    });
  }

  void _selectSize(String size) {
    setState(() => _selectedSize = size);
  }

  bool get _canAddToCart => !_hasVariants || _selectedVariantId != null;

  /// `product.price`, repository tarafından bu varyant üzerinden zaten
  /// doldurulmuştur (bkz. `ProductRepositoryImpl._priceReferenceFor`).
  String? get _defaultVariantId =>
      _hasVariants ? widget.product.variants.first.id : null;

  /// Galeri, seçili (ya da henüz seçilmediyse varsayılan) varyantın kendine
  /// özel görselleri varsa onları gösterir (bkz. `ProductVariant.imageUrls`)
  /// — ör. renk değişince fotoğraflar da değişir. Varyantın özel görseli
  /// yoksa (ör. yalnızca beden farkı olan ürünlerde fotoğraflar ortak
  /// olabilir) ürünün genel görsellerine, o da yoksa tekil `imageUrl`'e
  /// düşülür.
  List<String> get _galleryImageUrls {
    final variantId = _previewVariantId;
    if (variantId != null) {
      for (final variant in widget.product.variants) {
        if (variant.id == variantId && variant.imageUrls.isNotEmpty) {
          return variant.imageUrls;
        }
      }
    }
    if (widget.product.imageUrls.isNotEmpty) return widget.product.imageUrls;
    return [widget.product.imageUrl];
  }

  /// Bir varyant satın alan kullanıcı da üst ürünü değerlendirebilir —
  /// backend bunu açıkça destekler (bkz. `CreateMyReviewCommandHandler`:
  /// "A product can also be reviewed by someone who bought one of its
  /// variants rather than the bare product itself"). Bu yüzden
  /// değerlendirmeler her zaman ürünün kendi id'siyle (varyant seçimi ne
  /// olursa olsun sabit) sorgulanır — aksi halde varyantlı bir üründe,
  /// ürün seviyesinde girilmiş değerlendirmeler varyant id'siyle sorgu
  /// yapıldığı için hiç görünmez.
  String get _reviewSellableItemId => widget.product.id;

  ReviewItemType get _reviewSellableItemType => ReviewItemType.product;

  Future<void> _addToCart() async {
    final hasVariants = _hasVariants;
    setState(() => _isAddingToCart = true);
    await ref
        .read(cartControllerProvider.notifier)
        .add(
          sellableItemId: hasVariants ? _selectedVariantId! : widget.product.id,
          isVariant: hasVariants,
          title: widget.product.title,
          imageUrl: _galleryImageUrls.first,
          variantLabel: _selectedVariantLabel,
        );
    if (!mounted) return;
    setState(() => _isAddingToCart = false);
    final failure = ref.read(cartControllerProvider).failure;
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(content: Text(failure?.message ?? 'Ürün sepete eklendi.')),
    );
  }

  @override
  Widget build(BuildContext context) {
    final product = widget.product;
    final theme = Theme.of(context);
    return SingleChildScrollView(
      padding: EdgeInsets.zero,
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          // 3:4 en-boy oranı — web'deki `aspect-[3/4]` ürün görseli
          // yerleşimiyle aynı; kenardan kenara (tam genişlik), eski sabit
          // 300px yüksekliğe göre daha büyük ve dikkat çekici.
          AspectRatio(
            aspectRatio: 3 / 4,
            child: _ProductImageGallery(
              // Seçili varyant (ör. renk) değişince galerinin sıfırdan
              // (ilk sayfadan) başlaması için varyant id'sine göre key.
              key: ValueKey(_previewVariantId ?? 'default'),
              imageUrls: _galleryImageUrls,
            ),
          ),
          Padding(
            padding: const EdgeInsets.all(24),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.stretch,
              children: [
                _CategoryBadge(categoryId: product.category),
                Row(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Expanded(
                      child: Text(
                        product.title,
                        style: theme.textTheme.headlineLarge,
                      ),
                    ),
                    FavoriteButton(
                      item: FavoriteItem(
                        productId: product.id,
                        title: product.title,
                        imageUrl: product.imageUrl,
                      ),
                    ),
                  ],
                ),
                const SizedBox(height: 8),
                _ReviewSummaryRow(
                  sellableItemId: _reviewSellableItemId,
                  sellableItemType: _reviewSellableItemType,
                ),
                const SizedBox(height: 12),
                _PriceText(
                  defaultPrice: product.price,
                  selectedVariantId: _previewVariantId,
                  defaultVariantId: _defaultVariantId,
                ),
                const SizedBox(height: 20),
                Text(product.description, style: theme.textTheme.bodyLarge),
                if (_hasColorVariants) ...[
                  const SizedBox(height: 24),
                  Text(
                    'RENK',
                    style: theme.textTheme.labelSmall?.copyWith(
                      letterSpacing: 1.2,
                    ),
                  ),
                  const SizedBox(height: 8),
                  Wrap(
                    spacing: 10,
                    runSpacing: 10,
                    children: [
                      for (final color in _uniqueColors)
                        _ColorOption(
                          value: color,
                          selected: _selectedColor == color,
                          onTap: () => _selectColor(color),
                        ),
                    ],
                  ),
                ],
                if (_hasSizeVariants) ...[
                  const SizedBox(height: 24),
                  Text(
                    'BEDEN',
                    style: theme.textTheme.labelSmall?.copyWith(
                      letterSpacing: 1.2,
                    ),
                  ),
                  const SizedBox(height: 8),
                  Wrap(
                    spacing: 8,
                    runSpacing: 8,
                    children: [
                      for (final size in _allSizes)
                        _SizeBox(
                          label: size,
                          selected: _selectedSize == size,
                          disabled: !_isSizeAvailable(size),
                          onTap: () => _selectSize(size),
                        ),
                    ],
                  ),
                ],
                const SizedBox(height: 32),
                SizedBox(
                  height: 52,
                  child: FilledButton.icon(
                    onPressed: (_isAddingToCart || !_canAddToCart)
                        ? null
                        : _addToCart,
                    icon: _isAddingToCart
                        ? const SizedBox(
                            height: 18,
                            width: 18,
                            child: CircularProgressIndicator(
                              strokeWidth: 2,
                              color: Colors.white,
                            ),
                          )
                        : const Icon(Icons.add_shopping_cart),
                    label: Text(
                      'SEPETE EKLE',
                      style: theme.textTheme.labelLarge?.copyWith(
                        color: theme.colorScheme.onPrimary,
                      ),
                    ),
                  ),
                ),
                ReviewSection(
                  sellableItemId: _reviewSellableItemId,
                  sellableItemType: _reviewSellableItemType,
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}

/// Ürün başlığının üstünde, ürünün kategorisini gösteren küçük etiket.
/// Adı `GET /api/categories/{categoryId}` ile çözer: ürün DTO'su yalnızca
/// `categoryId` taşır ve kategori ağacı (`categoriesProvider`) pasif
/// kategorileri hiç içermez, bu uç ise her zaman doğru adı verir.
///
/// Dokunulduğunda koleksiyon ekranı o kategoriyle filtrelenmiş açılır.
/// Kategori çözülemezse (silinmiş kategori, ağ hatası) hiçbir şey
/// gösterilmez — ürün detayının kendisi bundan etkilenmemeli.
class _CategoryBadge extends ConsumerWidget {
  const _CategoryBadge({required this.categoryId});
  final String categoryId;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    if (categoryId.isEmpty) return const SizedBox.shrink();
    final result = ref.watch(categoryDetailProvider(categoryId)).value;
    final category = switch (result) {
      Success<CategoryDetail>(:final value) => value,
      ResultFailure<CategoryDetail>() => null,
      null => null,
    };
    if (category == null) return const SizedBox.shrink();

    final theme = Theme.of(context);
    return Align(
      alignment: Alignment.centerLeft,
      child: InkWell(
        onTap: () {
          ref
              .read(productListControllerProvider.notifier)
              .selectCategory(categoryId);
          context.push(RoutePaths.products);
        },
        borderRadius: BorderRadius.circular(4),
        child: Padding(
          padding: const EdgeInsets.symmetric(vertical: 4),
          child: Row(
            mainAxisSize: MainAxisSize.min,
            children: [
              Text(
                category.name.toUpperCase(),
                style: theme.textTheme.labelSmall?.copyWith(
                  letterSpacing: 1.2,
                  color: theme.colorScheme.primary,
                ),
              ),
              const SizedBox(width: 2),
              Icon(
                Icons.chevron_right,
                size: 14,
                color: theme.colorScheme.primary,
              ),
            ],
          ),
        ),
      ),
    );
  }
}

/// Ürün başlığının hemen altında kompakt bir "★ 4.5 (12)" özeti — web'in
/// ürün detayındaki aynı yerleşimi. Zaten var olan
/// `productReviewSummaryProvider`'ı izler; yeni provider/usecase eklenmedi.
/// Değerlendirme yoksa (ya da yüklenirken/hata durumunda) hiçbir şey
/// göstermez, aşağıdaki `ReviewSection` zaten tam durumu gösteriyor.
class _ReviewSummaryRow extends ConsumerWidget {
  const _ReviewSummaryRow({
    required this.sellableItemId,
    required this.sellableItemType,
  });

  final String sellableItemId;
  final ReviewItemType sellableItemType;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final target = (
      sellableItemId: sellableItemId,
      sellableItemType: sellableItemType,
    );
    final summary = ref.watch(productReviewSummaryProvider(target));
    return summary.when(
      data: (result) => switch (result) {
        Success<ReviewSummary>(:final value) when value.totalCount > 0 =>
          Row(
            children: [
              const Icon(Icons.star, size: 16),
              const SizedBox(width: 4),
              Text(
                value.averageRating.toStringAsFixed(1),
                style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                  fontWeight: FontWeight.w600,
                ),
              ),
              const SizedBox(width: 4),
              Text(
                '(${value.totalCount} değerlendirme)',
                style: Theme.of(context).textTheme.bodySmall,
              ),
            ],
          ),
        _ => const SizedBox.shrink(),
      },
      loading: () => const SizedBox.shrink(),
      error: (_, _) => const SizedBox.shrink(),
    );
  }
}

/// Ürün detayındaki görsel alanı. Tek görsel varsa eski davranış gibi
/// doğrudan `AppNetworkImage` gösterir (kaydırma/gösterge yok); birden
/// fazla görsel varsa yatay kaydırılabilir bir `PageView` + alt nokta
/// göstergesi ekler. Çağıran taraftaki `SizedBox(height: 300)` (yükseklik)
/// ve etraftaki boşluklar değişmedi.
class _ProductImageGallery extends StatefulWidget {
  const _ProductImageGallery({super.key, required this.imageUrls});
  final List<String> imageUrls;

  @override
  State<_ProductImageGallery> createState() => _ProductImageGalleryState();
}

class _ProductImageGalleryState extends State<_ProductImageGallery> {
  final _pageController = PageController();
  int _page = 0;

  @override
  void dispose() {
    _pageController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final imageUrls = widget.imageUrls;
    if (imageUrls.length <= 1) {
      return AppNetworkImage(
        imageUrl: imageUrls.isNotEmpty ? imageUrls.first : '',
        fit: BoxFit.cover,
      );
    }
    return Stack(
      alignment: Alignment.bottomCenter,
      children: [
        PageView.builder(
          controller: _pageController,
          itemCount: imageUrls.length,
          onPageChanged: (index) => setState(() => _page = index),
          itemBuilder: (context, index) => AppNetworkImage(
            imageUrl: imageUrls[index],
            fit: BoxFit.cover,
          ),
        ),
        Padding(
          padding: const EdgeInsets.only(bottom: 12),
          child: Row(
            mainAxisSize: MainAxisSize.min,
            children: [
              for (var i = 0; i < imageUrls.length; i++)
                AnimatedContainer(
                  duration: const Duration(milliseconds: 200),
                  margin: const EdgeInsets.symmetric(horizontal: 3),
                  width: i == _page ? 16 : 6,
                  height: 6,
                  decoration: BoxDecoration(
                    borderRadius: BorderRadius.circular(3),
                    color: Colors.white.withValues(alpha: i == _page ? 1 : .5),
                  ),
                ),
            ],
          ),
        ),
      ],
    );
  }
}

/// Beden seçeneklerini mantıksal sırada gösterir; tanınmayan bir etiket
/// gelirse (beklenmedik bir değer) listenin sonuna eklenir, mevcut sırası
/// korunur.
const _sizeOrder = ['XS', 'S', 'M', 'L', 'XL', 'XXL'];

List<ProductVariant> _sortedBySizeOrder(List<ProductVariant> variants) {
  final sorted = [...variants];
  sorted.sort((first, second) {
    final firstIndex = _sizeOrder.indexOf(first.size);
    final secondIndex = _sizeOrder.indexOf(second.size);
    return (firstIndex == -1 ? _sizeOrder.length : firstIndex).compareTo(
      secondIndex == -1 ? _sizeOrder.length : secondIndex,
    );
  });
  return sorted;
}

class _SizeBox extends StatelessWidget {
  const _SizeBox({
    required this.label,
    required this.selected,
    required this.onTap,
    this.disabled = false,
  });

  final String label;
  final bool selected;
  final VoidCallback onTap;

  /// Seçili renkte bu bedenin gerçek bir varyantı yoksa `true` — buton
  /// tıklanamaz hale getirilir ve soluk gösterilir, böylece geçersiz bir
  /// renk+beden kombinasyonu hiç seçilemez (silinmek yerine görünür kalır).
  final bool disabled;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return InkWell(
      onTap: disabled ? null : onTap,
      borderRadius: BorderRadius.circular(4),
      // Kategori çipleriyle aynı süre ve aynı "seçili yüzey" rengi: seçim
      // animasyonu uygulamanın her yerinde tek bir dile sahip olsun diye.
      child: AnimatedContainer(
        duration: AppMotion.selection,
        curve: Curves.easeOut,
        width: 48,
        height: 48,
        alignment: Alignment.center,
        decoration: BoxDecoration(
          color: selected ? AppColors.selectedSurface : null,
          border: Border.all(
            color: disabled
                ? theme.colorScheme.outline.withValues(alpha: .3)
                : selected
                ? AppColors.selectedSurface
                : theme.colorScheme.outline,
            width: selected ? 2 : 1,
          ),
          borderRadius: BorderRadius.circular(4),
        ),
        // Metin de kutuyla aynı hızda ters çevrilir; anlık değişirse geçiş
        // sırasında beyaz yazı bir kare açık zeminde kalıyor.
        child: AnimatedDefaultTextStyle(
          duration: AppMotion.selection,
          curve: Curves.easeOut,
          style: (theme.textTheme.bodyMedium ?? const TextStyle()).copyWith(
            fontWeight: selected ? FontWeight.bold : FontWeight.normal,
            color: disabled
                ? theme.colorScheme.onSurface.withValues(alpha: .3)
                : selected
                ? AppColors.onSelectedSurface
                : theme.colorScheme.onSurface,
          ),
          child: Text(label),
        ),
      ),
    );
  }
}

/// Renk seçeneğini küçük bir renk kutusu (swatch) olarak gösterir — ham hex
/// kod ya da renk adı kullanıcıya asla metin olarak gösterilmez, her zaman
/// çözümlenmiş renge boyanmış küçük bir kare olarak render edilir. Seçili
/// olduğunda yalnızca ince bir border ile belli olur; büyük arka plan veya
/// chip görünümü yoktur.
class _ColorOption extends StatelessWidget {
  const _ColorOption({
    required this.value,
    required this.selected,
    required this.onTap,
  });

  final String value;
  final bool selected;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return InkWell(
      onTap: onTap,
      borderRadius: BorderRadius.circular(4),
      child: AnimatedContainer(
        duration: AppMotion.selection,
        curve: Curves.easeOut,
        width: 32,
        height: 32,
        decoration: BoxDecoration(
          color: _resolveColor(value),
          borderRadius: BorderRadius.circular(4),
          border: Border.all(
            color: selected
                ? AppColors.selectedSurface
                : theme.colorScheme.outline,
            width: selected ? 2 : 1,
          ),
        ),
      ),
    );
  }
}

/// `value`'yu (hex kod ya da bilinen bir renk adı) gösterilecek `Color`'a
/// çevirir; ikisi de tanınmazsa nötr bir yer tutucu renk döner — kullanıcıya
/// asla ham metin/hex gösterilmez.
Color _resolveColor(String value) {
  return _tryParseHexColor(value) ??
      _namedColors[value.trim().toLowerCase()] ??
      Colors.grey.shade300;
}

/// `"#RRGGBB"`/`"#AARRGGBB"` biçimindeki bir hex kodu `Color`'a çevirir;
/// geçersizse `null` döner.
Color? _tryParseHexColor(String value) {
  final hex = value.trim();
  if (!RegExp(r'^#([0-9a-fA-F]{6}|[0-9a-fA-F]{8})$').hasMatch(hex)) return null;
  final digits = hex.substring(1);
  final argb = digits.length == 6 ? 'FF$digits' : digits;
  return Color(int.parse(argb, radix: 16));
}

/// Backend'de hex yerine düz renk adı gelme ihtimaline karşı bilinen
/// Türkçe renk adlarının karşılıkları.
const _namedColors = <String, Color>{
  'siyah': Colors.black,
  'beyaz': Colors.white,
  'kırmızı': Colors.red,
  'kirmizi': Colors.red,
  'mavi': Colors.blue,
  'lacivert': Color(0xFF000080),
  'yeşil': Colors.green,
  'yesil': Colors.green,
  'sarı': Colors.yellow,
  'sari': Colors.yellow,
  'turuncu': Colors.orange,
  'mor': Colors.purple,
  'pembe': Colors.pink,
  'gri': Colors.grey,
  'kahverengi': Colors.brown,
  'bej': Color(0xFFF5F5DC),
};

class _PriceText extends ConsumerWidget {
  const _PriceText({
    required this.defaultPrice,
    required this.selectedVariantId,
    required this.defaultVariantId,
  });

  /// Repository tarafından varsayılan (ilk) varyant/ürün üzerinden
  /// doldurulan fiyat.
  final double? defaultPrice;
  final String? selectedVariantId;
  final String? defaultVariantId;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final style = Theme.of(context).textTheme.headlineMedium;
    final variantId = selectedVariantId;
    if (variantId == null || variantId == defaultVariantId) {
      return Text(_label(defaultPrice), style: style);
    }
    final variantPrice = ref.watch(variantPriceProvider(variantId));
    return variantPrice.when(
      data: (result) => Text(
        _label(switch (result) {
          Success<double?>(:final value) => value,
          ResultFailure<double?>() => null,
        }),
        style: style,
      ),
      loading: () => Text(_label(defaultPrice), style: style),
      error: (_, _) => Text(_label(null), style: style),
    );
  }

  String _label(double? price) =>
      price != null ? price.toTryCurrency() : 'Fiyat bilgisi yakında eklenecek';
}
