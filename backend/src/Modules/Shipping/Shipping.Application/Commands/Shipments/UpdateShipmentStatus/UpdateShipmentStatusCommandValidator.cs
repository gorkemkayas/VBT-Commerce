using FluentValidation;

namespace Shipping.Application.Commands.Shipments.UpdateShipmentStatus;

public class UpdateShipmentStatusCommandValidator : AbstractValidator<UpdateShipmentStatusCommand>
{
    public UpdateShipmentStatusCommandValidator()
    {
        RuleFor(x => x.ShipmentId).NotEmpty();
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.TrackingNumber).MaximumLength(100);
    }
}
