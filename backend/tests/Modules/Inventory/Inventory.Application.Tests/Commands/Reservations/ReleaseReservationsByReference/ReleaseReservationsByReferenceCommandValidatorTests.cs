using FluentAssertions;
using Inventory.Application.Commands.Reservations.ReleaseReservationsByReference;
using Xunit;

namespace Inventory.Application.Tests.Commands.Reservations.ReleaseReservationsByReference;

public class ReleaseReservationsByReferenceCommandValidatorTests
{
    private readonly ReleaseReservationsByReferenceCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_HasNoErrors()
    {
        var command = new ReleaseReservationsByReferenceCommand(Guid.NewGuid());

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyReferenceId_HasError()
    {
        var command = new ReleaseReservationsByReferenceCommand(Guid.Empty);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(ReleaseReservationsByReferenceCommand.ReferenceId));
    }
}
