namespace Payment.Application.Gateway;

public record IyzicoCardInfo(string HolderName, string CardNumber, string ExpireMonth, string ExpireYear, string Cvc);

public record IyzicoBuyerInfo(string Name, string Surname, string Email, string IdentityNumber, string PhoneNumber, string Ip);

public record IyzicoAddressInfo(string Description, string City, string Country, string ZipCode);

public record IyzicoBasketItem(string Name, string Category, decimal Price);

public record IyzicoChargeRequest(
    Guid OrderId,
    decimal BasketTotal,
    decimal PaidTotal,
    IyzicoCardInfo Card,
    IyzicoBuyerInfo Buyer,
    IyzicoAddressInfo Address,
    IReadOnlyCollection<IyzicoBasketItem> BasketItems);

public record IyzicoChargeResult(
    bool IsSuccess,
    string? ProviderPaymentId,
    string? CardAssociation,
    string? CardFamily,
    string? CardLastFourDigits,
    string? ErrorMessage);

public record IyzicoRefundRequest(string ProviderPaymentId, string Ip);

public record IyzicoRefundResult(bool IsSuccess, string? ErrorMessage);
