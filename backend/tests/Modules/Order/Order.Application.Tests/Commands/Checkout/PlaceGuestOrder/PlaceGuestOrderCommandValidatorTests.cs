using FluentAssertions;
using Order.Application.Commands.Checkout.PlaceGuestOrder;
using Xunit;

namespace Order.Application.Tests.Commands.Checkout.PlaceGuestOrder;

public class PlaceGuestOrderCommandValidatorTests
{
    private readonly PlaceGuestOrderCommandValidator _validator = new();

    private static PlaceGuestOrderCommand CreateValidCommand() =>
        new(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            [],
            "Jane Doe",
            "5551234567",
            "Turkey",
            "Istanbul",
            "Kadikoy",
            "34000",
            "Bagdat Cd. No:1",
            null,
            null, null, null, null, null, null, null, null,
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
    public void Validate_WithEmptyGuestCustomerId_HasError()
    {
        var command = CreateValidCommand() with { GuestCustomerId = Guid.Empty };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(PlaceGuestOrderCommand.GuestCustomerId));
    }

    [Fact]
    public void Validate_WithEmptyAnonymousId_HasError()
    {
        var command = CreateValidCommand() with { AnonymousId = Guid.Empty };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(PlaceGuestOrderCommand.AnonymousId));
    }

    [Fact]
    public void Validate_WithEmptyShippingCompanyId_HasError()
    {
        var command = CreateValidCommand() with { ShippingCompanyId = Guid.Empty };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(PlaceGuestOrderCommand.ShippingCompanyId));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_WithEmptyRecipientName_HasError(string recipientName)
    {
        var command = CreateValidCommand() with { RecipientName = recipientName };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(PlaceGuestOrderCommand.RecipientName));
    }

    [Theory]
    [InlineData("13")]
    [InlineData("00")]
    [InlineData("1")]
    public void Validate_WithInvalidCardExpireMonth_HasError(string month)
    {
        var command = CreateValidCommand() with { CardExpireMonth = month };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(PlaceGuestOrderCommand.CardExpireMonth));
    }

    [Theory]
    [InlineData("30")]
    [InlineData("abcd")]
    public void Validate_WithInvalidCardExpireYear_HasError(string year)
    {
        var command = CreateValidCommand() with { CardExpireYear = year };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(PlaceGuestOrderCommand.CardExpireYear));
    }

    [Theory]
    [InlineData("12")]
    [InlineData("abc")]
    public void Validate_WithInvalidCardCvc_HasError(string cvc)
    {
        var command = CreateValidCommand() with { CardCvc = cvc };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(PlaceGuestOrderCommand.CardCvc));
    }

    [Theory]
    [InlineData("123")]
    [InlineData("123456789012")]
    public void Validate_WithInvalidBuyerIdentityNumber_HasError(string identityNumber)
    {
        var command = CreateValidCommand() with { BuyerIdentityNumber = identityNumber };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(PlaceGuestOrderCommand.BuyerIdentityNumber));
    }
}
