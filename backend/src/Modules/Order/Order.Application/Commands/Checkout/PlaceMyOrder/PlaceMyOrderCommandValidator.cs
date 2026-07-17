using FluentValidation;

namespace Order.Application.Commands.Checkout.PlaceMyOrder;

public class PlaceMyOrderCommandValidator : AbstractValidator<PlaceMyOrderCommand>
{
    public PlaceMyOrderCommandValidator()
    {
        RuleFor(x => x.AddressId).NotEmpty();
        RuleFor(x => x.ShippingCompanyId).NotEmpty();
        RuleFor(x => x.CardHolderName).NotEmpty();
        RuleFor(x => x.CardNumber).NotEmpty();
        RuleFor(x => x.CardExpireMonth).Matches(@"^(0[1-9]|1[0-2])$");
        RuleFor(x => x.CardExpireYear).Matches(@"^\d{4}$");
        RuleFor(x => x.CardCvc).Matches(@"^\d{3,4}$");
        RuleFor(x => x.BuyerIdentityNumber).Matches(@"^\d{11}$");
    }
}
