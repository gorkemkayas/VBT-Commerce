using FluentValidation;

namespace Inventory.Application.Commands.Reservations.ConfirmReservationsByReference;

public class ConfirmReservationsByReferenceCommandValidator : AbstractValidator<ConfirmReservationsByReferenceCommand>
{
    public ConfirmReservationsByReferenceCommandValidator()
    {
        RuleFor(x => x.ReferenceId).NotEmpty();
    }
}
