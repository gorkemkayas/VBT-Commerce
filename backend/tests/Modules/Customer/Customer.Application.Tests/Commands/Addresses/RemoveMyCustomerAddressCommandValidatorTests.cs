using Customer.Application.Commands.Addresses.RemoveMyCustomerAddress;
using FluentAssertions;
using Xunit;

namespace Customer.Application.Tests.Commands.Addresses;

public class RemoveMyCustomerAddressCommandValidatorTests
{
    private readonly RemoveMyCustomerAddressCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_HasNoErrors()
    {
        var result = _validator.Validate(new RemoveMyCustomerAddressCommand(Guid.NewGuid()));

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyAddressId_HasError()
    {
        var result = _validator.Validate(new RemoveMyCustomerAddressCommand(Guid.Empty));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(RemoveMyCustomerAddressCommand.AddressId));
    }
}
