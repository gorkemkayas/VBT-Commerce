/// Web'deki "Ödeme Bilgileri" formuyla birebir aynı alanlar — backend'in
/// `PlaceMyOrderRequest`/`PlaceGuestOrderRequest`'indeki `Card*` ve
/// `BuyerIdentityNumber` alanlarıyla eşleşir. Cihazda hiçbir yerde
/// saklanmaz; yalnızca sipariş oluşturma isteği sırasında kullanılır.
class PaymentCardInfo {
  const PaymentCardInfo({
    required this.cardHolderName,
    required this.cardNumber,
    required this.cardExpireMonth,
    required this.cardExpireYear,
    required this.cardCvc,
    required this.buyerIdentityNumber,
  });

  final String cardHolderName;
  final String cardNumber;
  final String cardExpireMonth;
  final String cardExpireYear;
  final String cardCvc;
  final String buyerIdentityNumber;
}
