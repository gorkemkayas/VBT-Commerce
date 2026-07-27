using FluentAssertions;
using Shipping.Application.Commands.Shipments.UpdateShipmentStatus;
using Shipping.Domain.Enums;
using Xunit;

namespace Shipping.Application.Tests.Commands.Shipments;

public class UpdateShipmentStatusCommandValidatorTests
{
    private readonly UpdateShipmentStatusCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_HasNoErrors()
    {
        var command = new UpdateShipmentStatusCommand(Guid.NewGuid(), ShipmentStatus.Shipped, "TRK-1");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyShipmentId_HasError()
    {
        var command = new UpdateShipmentStatusCommand(Guid.Empty, ShipmentStatus.Shipped, null);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateShipmentStatusCommand.ShipmentId));
    }

    [Fact]
    public void Validate_WithInvalidStatus_HasError()
    {
        var command = new UpdateShipmentStatusCommand(Guid.NewGuid(), (ShipmentStatus)999, null);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateShipmentStatusCommand.Status));
    }

    [Fact]
    public void Validate_WithTrackingNumberTooLong_HasError()
    {
        var command = new UpdateShipmentStatusCommand(Guid.NewGuid(), ShipmentStatus.Shipped, new string('a', 101));

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateShipmentStatusCommand.TrackingNumber));
    }
}
