using FluentAssertions;
using Payment.Application.Queries.GetPaymentById;
using Xunit;

namespace Payment.Application.Tests.Queries.GetPaymentById;

public class GetPaymentByIdQueryValidatorTests
{
    private readonly GetPaymentByIdQueryValidator _validator = new();

    [Fact]
    public void Validate_WithValidQuery_HasNoErrors()
    {
        var query = new GetPaymentByIdQuery(Guid.NewGuid());

        var result = _validator.Validate(query);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyPaymentId_HasError()
    {
        var query = new GetPaymentByIdQuery(Guid.Empty);

        var result = _validator.Validate(query);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(GetPaymentByIdQuery.PaymentId));
    }
}
