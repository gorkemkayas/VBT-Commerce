using Iyzipay.Model;
using Iyzipay.Request;
using Microsoft.Extensions.Options;
using Payment.Application.Gateway;
using Payment.Domain.Exceptions;
using Payment.Infrastructure.Options;

namespace Payment.Infrastructure.Gateway;

/// <summary>
/// Wraps the Iyzipay .NET SDK (non-3DS direct charge via Payment.Create, full-payment reversal via
/// Cancel.Create — see the plan's "sadece tam iade" decision). The SDK itself is synchronous, so
/// each call is dispatched via Task.Run to keep this gateway's interface async without blocking a
/// request thread on a blocking HTTP call for longer than necessary.
/// </summary>
public class IyzicoGateway(IOptions<IyzicoOptions> options) : IIyzicoGateway
{
    private readonly IyzicoOptions _options = options.Value;

    public async Task<IyzicoChargeResult> ChargeAsync(IyzicoChargeRequest request, CancellationToken cancellationToken)
    {
        var iyzicoOptions = BuildOptions();

        var paymentRequest = new CreatePaymentRequest
        {
            Locale = Iyzipay.Model.Locale.TR.ToString(),
            ConversationId = request.OrderId.ToString(),
            Price = request.BasketTotal.ToString("F2", System.Globalization.CultureInfo.InvariantCulture),
            PaidPrice = request.PaidTotal.ToString("F2", System.Globalization.CultureInfo.InvariantCulture),
            Currency = Iyzipay.Model.Currency.TRY.ToString(),
            Installment = 1,
            BasketId = request.OrderId.ToString(),
            PaymentChannel = PaymentChannel.WEB.ToString(),
            PaymentGroup = PaymentGroup.PRODUCT.ToString(),
            PaymentCard = new PaymentCard
            {
                CardHolderName = request.Card.HolderName,
                CardNumber = request.Card.CardNumber,
                ExpireMonth = request.Card.ExpireMonth,
                ExpireYear = request.Card.ExpireYear,
                Cvc = request.Card.Cvc,
                RegisterCard = 0
            },
            Buyer = new Buyer
            {
                Id = request.OrderId.ToString(),
                Name = request.Buyer.Name,
                Surname = request.Buyer.Surname,
                GsmNumber = request.Buyer.PhoneNumber,
                Email = request.Buyer.Email,
                IdentityNumber = request.Buyer.IdentityNumber,
                RegistrationAddress = request.Address.Description,
                Ip = request.Buyer.Ip,
                City = request.Address.City,
                Country = request.Address.Country,
                ZipCode = request.Address.ZipCode
            },
            ShippingAddress = new Iyzipay.Model.Address
            {
                ContactName = request.Buyer.Name + " " + request.Buyer.Surname,
                City = request.Address.City,
                Country = request.Address.Country,
                ZipCode = request.Address.ZipCode,
                Description = request.Address.Description
            },
            BillingAddress = new Iyzipay.Model.Address
            {
                ContactName = request.Buyer.Name + " " + request.Buyer.Surname,
                City = request.Address.City,
                Country = request.Address.Country,
                ZipCode = request.Address.ZipCode,
                Description = request.Address.Description
            },
            BasketItems = request.BasketItems.Select((item, index) => new BasketItem
            {
                Id = index.ToString(),
                Name = item.Name,
                Category1 = item.Category,
                ItemType = BasketItemType.PHYSICAL.ToString(),
                Price = item.Price.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)
            }).ToList()
        };

        Iyzipay.Model.Payment result;
        try
        {
            result = await Task.Run(() => Iyzipay.Model.Payment.Create(paymentRequest, iyzicoOptions), cancellationToken);
        }
        catch (Exception exception)
        {
            throw new PaymentGatewayException(exception.Message);
        }

        if (result.Status != "success")
            return new IyzicoChargeResult(false, null, null, null, null, result.ErrorMessage ?? "Card was declined.");

        return new IyzicoChargeResult(true, result.PaymentId, result.CardAssociation, result.CardFamily, result.LastFourDigits, null);
    }

    public async Task<IyzicoRefundResult> RefundAsync(IyzicoRefundRequest request, CancellationToken cancellationToken)
    {
        var iyzicoOptions = BuildOptions();

        var cancelRequest = new CreateCancelRequest
        {
            Locale = Iyzipay.Model.Locale.TR.ToString(),
            ConversationId = Guid.NewGuid().ToString(),
            PaymentId = request.ProviderPaymentId,
            Ip = request.Ip
        };

        Cancel result;
        try
        {
            result = await Task.Run(() => Cancel.Create(cancelRequest, iyzicoOptions), cancellationToken);
        }
        catch (Exception exception)
        {
            throw new PaymentGatewayException(exception.Message);
        }

        return result.Status != "success"
            ? new IyzicoRefundResult(false, result.ErrorMessage ?? "Refund was rejected.")
            : new IyzicoRefundResult(true, null);
    }

    private Iyzipay.Options BuildOptions() => new()
    {
        ApiKey = _options.ApiKey,
        SecretKey = _options.SecretKey,
        BaseUrl = _options.BaseUrl
    };
}
