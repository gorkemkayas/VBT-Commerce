namespace Payment.Contracts;

/// <summary>
/// Provider-neutral payment DTOs (architecture.md §3). Callers outside the Payment module (chiefly
/// Order, during checkout) only ever see these — never the iyzico-specific Iyzico* gateway types,
/// mirroring how Identity never lets Google's own token leak into other modules.
/// </summary>
public record PaymentCardInfo(string HolderName, string CardNumber, string ExpireMonth, string ExpireYear, string Cvc);

public record PaymentBuyerInfo(string Name, string Surname, string Email, string IdentityNumber, string PhoneNumber, string Ip);

public record PaymentAddressInfo(string Description, string City, string Country, string ZipCode);

public record PaymentBasketItem(string Name, string Category, decimal Price);
