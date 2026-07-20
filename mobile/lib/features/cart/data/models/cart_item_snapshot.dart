/// "Sepete Ekle" anında yerelde saklanan ürün adı/görsel/fiyat bilgisi.
/// Backend'in sepet kalemi yanıtı bu bilgileri içermediği için, sepet kalemi
/// kimliğine (`itemId`) göre yerelde tutulur ve sepet görüntülenirken
/// backend'den gelen kalemlerle eşleştirilir.
class CartItemSnapshot {
  const CartItemSnapshot({
    required this.title,
    required this.imageUrl,
    required this.unitPrice,
  });

  final String title;
  final String imageUrl;
  final double unitPrice;

  factory CartItemSnapshot.fromJson(Map<String, dynamic> json) =>
      CartItemSnapshot(
        title: json['title'] as String,
        imageUrl: json['imageUrl'] as String,
        unitPrice: (json['unitPrice'] as num).toDouble(),
      );

  Map<String, dynamic> toJson() => {
    'title': title,
    'imageUrl': imageUrl,
    'unitPrice': unitPrice,
  };
}
