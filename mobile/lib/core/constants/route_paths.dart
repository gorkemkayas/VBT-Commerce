abstract final class RoutePaths {
  static const login = '/login';
  static const register = '/register';
  static const forgotPassword = '/forgot-password';
  static const resetPassword = '/reset-password';
  static const home = '/';
  static const products = '/products';
  static const productDetail = '/product/:id';

  /// Ürünü SEO slug'ıyla açan derin bağlantı (`GET /api/products/by-slug/...`).
  /// `productDetail` tek segmentli olduğu için iki yol çakışmaz.
  static const productDetailBySlug = '/product/slug/:slug';
  static const cart = '/cart';

  /// Hesabım ekranı. Alt sekmede de var; `/cart` gibi ayrıca push edilebilir
  /// olması, sekme çubuğunun görünmediği sayfalardan (ör. ürün detayı)
  /// kısayolla gelinebilmesi içindir.
  static const account = '/account';
  static const search = '/search';
  static const checkout = '/checkout';
  static const orderConfirmation = '/checkout/confirmation';
  static const profile = '/profile';
  static const addresses = '/addresses';
  static const orders = '/orders';
  static const orderDetail = '/orders/:id';
  static const myReviews = '/my-reviews';
  static const shipmentTracking = '/shipment-tracking';
  static const guestOrderLookup = '/guest-order-lookup';
}
