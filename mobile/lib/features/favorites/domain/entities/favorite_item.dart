/// Kullanıcının favorilerine eklediği bir ürünün anlık görüntüsü. Favoriler
/// için bir backend uç noktası olmadığından yerelde saklanır; listeyi
/// göstermek ve ürün detayına gitmek için gereken asgari alanları tutar.
class FavoriteItem {
  const FavoriteItem({
    required this.productId,
    required this.title,
    required this.imageUrl,
  });

  final String productId;
  final String title;
  final String imageUrl;

  @override
  bool operator ==(Object other) =>
      other is FavoriteItem &&
      other.productId == productId &&
      other.title == title &&
      other.imageUrl == imageUrl;

  @override
  int get hashCode => Object.hash(productId, title, imageUrl);
}
