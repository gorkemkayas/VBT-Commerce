using FluentAssertions;
using Payment.Application.Commands.Refunds.RefundOrderPayment;
using Xunit;

namespace Payment.Application.Tests.Commands.Refunds;

public class RefundOrderPaymentCommandValidatorTests
{
    private readonly RefundOrderPaymentCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_HasNoErrors()
    {
        var command = new RefundOrderPaymentCommand(Guid.NewGuid(), "127.0.0.1");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyOrderId_HasError()
    {
        var command = new RefundOrderPaymentCommand(Guid.Empty, "127.0.0.1");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(RefundOrderPaymentCommand.OrderId));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_WithEmptyIp_HasError(string ip)
    {
        var command = new RefundOrderPaymentCommand(Guid.NewGuid(), ip);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(RefundOrderPaymentCommand.Ip));
    }
}
