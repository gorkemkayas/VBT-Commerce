using FluentValidation;

namespace Shipping.Application.Commands.Shipments.CreateShipment;

public class CreateShipmentCommandValidator : AbstractValidator<CreateShipmentCommand>
{
    public CreateShipmentCommandValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x.ShippingCompanyId).NotEmpty();
    }
}
