using FluentAssertions;
using Order.Application.Commands.Checkout.PlaceMyOrder;
using Xunit;

namespace Order.Application.Tests.Commands.Checkout.PlaceMyOrder;

public class PlaceMyOrderCommandValidatorTests
{
    private readonly PlaceMyOrderCommandValidator _validator = new();

    private static PlaceMyOrderCommand CreateValidCommand() =>
        new(
            Guid.NewGuid(),
            null,
            Guid.NewGuid(),
            [],
            "Jane Doe",
            "4111111111111111",
            "12",
            "2030",
            "123",
            "12345678901");

    [Fact]
    public void Validate_WithValidCommand_HasNoErrors()
    {
        var result = _validator.Validate(CreateValidCommand());

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyAddressId_HasError()
    {
        var command = CreateValidCommand() with { AddressId = Guid.Empty };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(PlaceMyOrderCommand.AddressId));
    }

    [Fact]
    public void Validate_WithEmptyShippingCompanyId_HasError()
    {
        var command = CreateValidCommand() with { ShippingCompanyId = Guid.Empty };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(PlaceMyOrderCommand.ShippingCompanyId));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_WithEmptyCardHolderName_HasError(string cardHolderName)
    {
        var command = CreateValidCommand() with { CardHolderName = cardHolderName };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(PlaceMyOrderCommand.CardHolderName));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_WithEmptyCardNumber_HasError(string cardNumber)
    {
        var command = CreateValidCommand() with { CardNumber = cardNumber };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(PlaceMyOrderCommand.CardNumber));
    }

    [Theory]
    [InlineData("13")]
    [InlineData("00")]
    public void Validate_WithInvalidCardExpireMonth_HasError(string month)
    {
        var command = CreateValidCommand() with { CardExpireMonth = month };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(PlaceMyOrderCommand.CardExpireMonth));
    }

    [Theory]
    [InlineData("30")]
    [InlineData("abcd")]
    public void Validate_WithInvalidCardExpireYear_HasError(string year)
    {
        var command = CreateValidCommand() with { CardExpireYear = year };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(PlaceMyOrderCommand.CardExpireYear));
    }

    [Theory]
    [InlineData("12")]
    [InlineData("abc")]
    public void Validate_WithInvalidCardCvc_HasError(string cvc)
    {
        var command = CreateValidCommand() with { CardCvc = cvc };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(PlaceMyOrderCommand.CardCvc));
    }

    [Theory]
    [InlineData("123")]
    [InlineData("123456789012")]
    public void Validate_WithInvalidBuyerIdentityNumber_HasError(string identityNumber)
    {
        var command = CreateValidCommand() with { BuyerIdentityNumber = identityNumber };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(PlaceMyOrderCommand.BuyerIdentityNumber));
    }
}
