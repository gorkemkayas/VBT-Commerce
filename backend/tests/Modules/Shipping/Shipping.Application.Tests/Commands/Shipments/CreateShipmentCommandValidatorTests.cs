using FluentAssertions;
using Shipping.Application.Commands.Shipments.CreateShipment;
using Xunit;

namespace Shipping.Application.Tests.Commands.Shipments;

public class CreateShipmentCommandValidatorTests
{
    private readonly CreateShipmentCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_HasNoErrors()
    {
        var command = new CreateShipmentCommand(Guid.NewGuid(), Guid.NewGuid());

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyOrderId_HasError()
    {
        var command = new CreateShipmentCommand(Guid.Empty, Guid.NewGuid());

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateShipmentCommand.OrderId));
    }

    [Fact]
    public void Validate_WithEmptyShippingCompanyId_HasError()
    {
        var command = new CreateShipmentCommand(Guid.NewGuid(), Guid.Empty);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateShipmentCommand.ShippingCompanyId));
    }
}
