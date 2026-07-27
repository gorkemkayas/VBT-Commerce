using FluentAssertions;
using Inventory.Application.Commands.Reservations.ConfirmReservationsByReference;
using Xunit;

namespace Inventory.Application.Tests.Commands.Reservations.ConfirmReservationsByReference;

public class ConfirmReservationsByReferenceCommandValidatorTests
{
    private readonly ConfirmReservationsByReferenceCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_HasNoErrors()
    {
        var command = new ConfirmReservationsByReferenceCommand(Guid.NewGuid());

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyReferenceId_HasError()
    {
        var command = new ConfirmReservationsByReferenceCommand(Guid.Empty);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(ConfirmReservationsByReferenceCommand.ReferenceId));
    }
}
