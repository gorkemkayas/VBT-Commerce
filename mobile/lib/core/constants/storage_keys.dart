abstract final class StorageKeys {
  static const currentUser = 'current_user';
  static const accessToken = 'access_token';
  static const refreshToken = 'refresh_token';
  static const anonymousId = 'anonymous_id';

  /// Son misafir checkout'unda oluşturulan misafir müşteri kaydının id'si —
  /// aynı cihazda tekrar misafir olarak alışveriş yapıldığında iletişim
  /// bilgilerini önceden doldurmak için saklanır.
  static const guestCustomerId = 'guest_customer_id';
  static const cartItemSnapshots = 'cart_item_snapshots';
  static const favoriteItems = 'favorite_items';
}
