using BuildingBlocks.Application.Messaging;
using Payment.Application.Gateway;

namespace Payment.Application.Commands.Charges.ChargeOrderPayment;

/// <summary>
/// No IRequireRole and no controller by design — meant to be called in-process via ISender by the
/// Order module's checkout saga, exactly like Shipping's CreateShipmentCommand and Inventory's
/// ReserveStockCommand. Card/buyer/basket data is passed straight through as the Gateway's own
/// Iyzico* types rather than a second, duplicate DTO set.
/// </summary>
public record ChargeOrderPaymentCommand(
    Guid OrderId,
    decimal BasketTotal,
    decimal PaidTotal,
    IyzicoCardInfo Card,
    IyzicoBuyerInfo Buyer,
    IyzicoAddressInfo Address,
    IReadOnlyCollection<IyzicoBasketItem> BasketItems) : ICommand<Guid>;
