using Customer.Application.Commands.Addresses.SetDefaultMyCustomerAddress;
using FluentAssertions;
using Xunit;

namespace Customer.Application.Tests.Commands.Addresses;

public class SetDefaultMyCustomerAddressCommandValidatorTests
{
    private readonly SetDefaultMyCustomerAddressCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_HasNoErrors()
    {
        var result = _validator.Validate(new SetDefaultMyCustomerAddressCommand(Guid.NewGuid()));

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyAddressId_HasError()
    {
        var result = _validator.Validate(new SetDefaultMyCustomerAddressCommand(Guid.Empty));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(SetDefaultMyCustomerAddressCommand.AddressId));
    }
}
