using FluentAssertions;
using Payment.Application.Commands.Charges.ChargeOrderPayment;
using Payment.Application.Gateway;
using Xunit;

namespace Payment.Application.Tests.Commands.Charges;

public class ChargeOrderPaymentCommandValidatorTests
{
    private readonly ChargeOrderPaymentCommandValidator _validator = new();

    private static ChargeOrderPaymentCommand BuildValidCommand() => new(
        Guid.NewGuid(),
        100m,
        100m,
        new IyzicoCardInfo("John Doe", "5528790000000008", "12", "2030", "123"),
        new IyzicoBuyerInfo("John", "Doe", "john@example.com", "12345678901", "5551234567", "127.0.0.1"),
        new IyzicoAddressInfo("Main St", "Istanbul", "Turkey", "34000"),
        new IyzicoAddressInfo("Main St", "Istanbul", "Turkey", "34000"),
        [new IyzicoBasketItem("Item", "Category", 100m)]);

    [Fact]
    public void Validate_WithValidCommand_HasNoErrors()
    {
        var result = _validator.Validate(BuildValidCommand());

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyOrderId_HasError()
    {
        var command = BuildValidCommand() with { OrderId = Guid.Empty };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(ChargeOrderPaymentCommand.OrderId));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void Validate_WithNonPositiveBasketTotal_HasError(decimal basketTotal)
    {
        var command = BuildValidCommand() with { BasketTotal = basketTotal };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(ChargeOrderPaymentCommand.BasketTotal));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void Validate_WithNonPositivePaidTotal_HasError(decimal paidTotal)
    {
        var command = BuildValidCommand() with { PaidTotal = paidTotal };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(ChargeOrderPaymentCommand.PaidTotal));
    }

    [Fact]
    public void Validate_WithEmptyBasketItems_HasError()
    {
        var command = BuildValidCommand() with { BasketItems = [] };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(ChargeOrderPaymentCommand.BasketItems));
    }

    [Fact]
    public void Validate_WithEmptyCardHolderName_HasError()
    {
        var command = BuildValidCommand();
        command = command with { Card = command.Card with { HolderName = "" } };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Card.HolderName");
    }

    [Fact]
    public void Validate_WithEmptyCardNumber_HasError()
    {
        var command = BuildValidCommand();
        command = command with { Card = command.Card with { CardNumber = "" } };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Card.CardNumber");
    }

    [Theory]
    [InlineData("00")]
    [InlineData("13")]
    [InlineData("1")]
    public void Validate_WithInvalidExpireMonth_HasError(string expireMonth)
    {
        var command = BuildValidCommand();
        command = command with { Card = command.Card with { ExpireMonth = expireMonth } };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Card.ExpireMonth");
    }

    [Theory]
    [InlineData("30")]
    [InlineData("abcd")]
    public void Validate_WithInvalidExpireYear_HasError(string expireYear)
    {
        var command = BuildValidCommand();
        command = command with { Card = command.Card with { ExpireYear = expireYear } };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Card.ExpireYear");
    }

    [Theory]
    [InlineData("12")]
    [InlineData("abcde")]
    public void Validate_WithInvalidCvc_HasError(string cvc)
    {
        var command = BuildValidCommand();
        command = command with { Card = command.Card with { Cvc = cvc } };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Card.Cvc");
    }

    [Fact]
    public void Validate_WithEmptyBuyerName_HasError()
    {
        var command = BuildValidCommand();
        command = command with { Buyer = command.Buyer with { Name = "" } };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Buyer.Name");
    }

    [Fact]
    public void Validate_WithEmptyBuyerSurname_HasError()
    {
        var command = BuildValidCommand();
        command = command with { Buyer = command.Buyer with { Surname = "" } };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Buyer.Surname");
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-an-email")]
    public void Validate_WithInvalidBuyerEmail_HasError(string email)
    {
        var command = BuildValidCommand();
        command = command with { Buyer = command.Buyer with { Email = email } };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Buyer.Email");
    }

    [Theory]
    [InlineData("123")]
    [InlineData("123456789012")]
    public void Validate_WithInvalidBuyerIdentityNumber_HasError(string identityNumber)
    {
        var command = BuildValidCommand();
        command = command with { Buyer = command.Buyer with { IdentityNumber = identityNumber } };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Buyer.IdentityNumber");
    }

    [Fact]
    public void Validate_WithEmptyBuyerPhoneNumber_HasError()
    {
        var command = BuildValidCommand();
        command = command with { Buyer = command.Buyer with { PhoneNumber = "" } };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Buyer.PhoneNumber");
    }

    [Fact]
    public void Validate_WithEmptyBuyerIp_HasError()
    {
        var command = BuildValidCommand();
        command = command with { Buyer = command.Buyer with { Ip = "" } };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Buyer.Ip");
    }

    [Fact]
    public void Validate_WithEmptyAddressDescription_HasError()
    {
        var command = BuildValidCommand();
        command = command with { Address = command.Address with { Description = "" } };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Address.Description");
    }

    [Fact]
    public void Validate_WithEmptyAddressCity_HasError()
    {
        var command = BuildValidCommand();
        command = command with { Address = command.Address with { City = "" } };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Address.City");
    }

    [Fact]
    public void Validate_WithEmptyAddressCountry_HasError()
    {
        var command = BuildValidCommand();
        command = command with { Address = command.Address with { Country = "" } };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Address.Country");
    }

    [Fact]
    public void Validate_WithEmptyAddressZipCode_HasError()
    {
        var command = BuildValidCommand();
        command = command with { Address = command.Address with { ZipCode = "" } };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Address.ZipCode");
    }
}
