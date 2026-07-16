using FluentValidation;

namespace Inventory.Application.Commands.Reservations.ReleaseReservationsByReference;

public class ReleaseReservationsByReferenceCommandValidator : AbstractValidator<ReleaseReservationsByReferenceCommand>
{
    public ReleaseReservationsByReferenceCommandValidator()
    {
        RuleFor(x => x.ReferenceId).NotEmpty();
    }
}
