using MediatR;
using Payment.Application.Commands.Charges.ChargeOrderPayment;
using Payment.Application.Commands.Refunds.RefundOrderPayment;
using Payment.Contracts;
using IyzicoAddressInfo = Payment.Application.Gateway.IyzicoAddressInfo;
using IyzicoBasketItem = Payment.Application.Gateway.IyzicoBasketItem;
using IyzicoBuyerInfo = Payment.Application.Gateway.IyzicoBuyerInfo;
using IyzicoCardInfo = Payment.Application.Gateway.IyzicoCardInfo;

namespace Payment.Infrastructure.Integrations;

/// <summary>
/// Implementation of Payment's outbound contract (<see cref="IPaymentGateway"/>). Translates the
/// provider-neutral Payment.Contracts DTOs to the iyzico-specific Gateway types here — this is the
/// only place outside Payment.Application/Payment.Infrastructure that ever constructs an Iyzico*
/// type — and delegates to the same MediatR commands used internally.
/// </summary>
public class PaymentGatewayContractService(ISender sender) : IPaymentGateway
{
    public Task<Guid> ChargeAsync(
        Guid orderId,
        decimal basketTotal,
        decimal paidTotal,
        PaymentCardInfo card,
        PaymentBuyerInfo buyer,
        PaymentAddressInfo address,
        IReadOnlyCollection<PaymentBasketItem> basketItems,
        CancellationToken cancellationToken)
    {
        var iyzicoCard = new IyzicoCardInfo(card.HolderName, card.CardNumber, card.ExpireMonth, card.ExpireYear, card.Cvc);
        var iyzicoBuyer = new IyzicoBuyerInfo(buyer.Name, buyer.Surname, buyer.Email, buyer.IdentityNumber, buyer.PhoneNumber, buyer.Ip);
        var iyzicoAddress = new IyzicoAddressInfo(address.Description, address.City, address.Country, address.ZipCode);
        var iyzicoBasketItems = basketItems.Select(i => new IyzicoBasketItem(i.Name, i.Category, i.Price)).ToList();

        return sender.Send(
            new ChargeOrderPaymentCommand(orderId, basketTotal, paidTotal, iyzicoCard, iyzicoBuyer, iyzicoAddress, iyzicoBasketItems),
            cancellationToken);
    }

    public async Task RefundAsync(Guid orderId, string ip, CancellationToken cancellationToken)
    {
        await sender.Send(new RefundOrderPaymentCommand(orderId, ip), cancellationToken);
    }
}
