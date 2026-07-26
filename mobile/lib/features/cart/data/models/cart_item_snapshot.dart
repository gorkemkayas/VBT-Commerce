/// "Sepete Ekle" anında yerelde saklanan ürün adı/görsel/fiyat bilgisi.
/// Backend'in sepet kalemi yanıtı bu bilgileri içermediği için, sepet kalemi
/// kimliğine (`itemId`) göre yerelde tutulur ve sepet görüntülenirken
/// backend'den gelen kalemlerle eşleştirilir.
class CartItemSnapshot {
  const CartItemSnapshot({
    required this.title,
    required this.imageUrl,
    required this.unitPrice,
    this.variantLabel,
  });

  final String title;
  final String imageUrl;
  final double unitPrice;

  /// Seçilen varyantın renk/beden bilgisi (ör. "Siyah, 42") — varyantsız
  /// ürünlerde veya renk/beden taşımayan varyantlarda `null`.
  final String? variantLabel;

  factory CartItemSnapshot.fromJson(Map<String, dynamic> json) =>
      CartItemSnapshot(
        title: json['title'] as String,
        imageUrl: json['imageUrl'] as String,
        unitPrice: (json['unitPrice'] as num).toDouble(),
        variantLabel: json['variantLabel'] as String?,
      );

  Map<String, dynamic> toJson() => {
    'title': title,
    'imageUrl': imageUrl,
    'unitPrice': unitPrice,
    'variantLabel': variantLabel,
  };
}
