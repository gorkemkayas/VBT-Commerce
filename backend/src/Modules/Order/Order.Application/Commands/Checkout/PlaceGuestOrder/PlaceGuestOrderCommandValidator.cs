using FluentValidation;

namespace Order.Application.Commands.Checkout.PlaceGuestOrder;

public class PlaceGuestOrderCommandValidator : AbstractValidator<PlaceGuestOrderCommand>
{
    public PlaceGuestOrderCommandValidator()
    {
        RuleFor(x => x.GuestCustomerId).NotEmpty();
        RuleFor(x => x.AnonymousId).NotEmpty();
        RuleFor(x => x.ShippingCompanyId).NotEmpty();
        RuleFor(x => x.RecipientName).NotEmpty();
        RuleFor(x => x.PhoneNumber).NotEmpty();
        RuleFor(x => x.Country).NotEmpty();
        RuleFor(x => x.City).NotEmpty();
        RuleFor(x => x.District).NotEmpty();
        RuleFor(x => x.PostalCode).NotEmpty();
        RuleFor(x => x.AddressLine1).NotEmpty();
        RuleFor(x => x.CardHolderName).NotEmpty();
        RuleFor(x => x.CardNumber).NotEmpty();
        RuleFor(x => x.CardExpireMonth).Matches(@"^(0[1-9]|1[0-2])$");
        RuleFor(x => x.CardExpireYear).Matches(@"^\d{4}$");
        RuleFor(x => x.CardCvc).Matches(@"^\d{3,4}$");
        RuleFor(x => x.BuyerIdentityNumber).Matches(@"^\d{11}$");
    }
}
