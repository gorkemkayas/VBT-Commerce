using FluentValidation;

namespace Order.Application.Commands.Checkout.PlaceMyOrder;

public class PlaceMyOrderCommandValidator : AbstractValidator<PlaceMyOrderCommand>
{
    public PlaceMyOrderCommandValidator()
    {
        RuleFor(x => x.AddressId).NotEmpty();
        RuleFor(x => x.ShippingCompanyId).NotEmpty();
    }
}
