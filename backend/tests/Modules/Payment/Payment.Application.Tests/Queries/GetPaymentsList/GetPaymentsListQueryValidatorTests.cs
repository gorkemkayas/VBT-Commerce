using FluentAssertions;
using Payment.Application.Queries.GetPaymentsList;
using Payment.Domain.Enums;
using Xunit;

namespace Payment.Application.Tests.Queries.GetPaymentsList;

public class GetPaymentsListQueryValidatorTests
{
    private readonly GetPaymentsListQueryValidator _validator = new();

    [Fact]
    public void Validate_WithDefaultQuery_HasNoErrors()
    {
        var result = _validator.Validate(new GetPaymentsListQuery());

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithValidStatus_HasNoErrors()
    {
        var result = _validator.Validate(new GetPaymentsListQuery(PaymentStatus.Refunded));

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithInvalidStatus_HasError()
    {
        var query = new GetPaymentsListQuery((PaymentStatus)999);

        var result = _validator.Validate(query);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(GetPaymentsListQuery.Status));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_WithNonPositivePageNumber_HasError(int pageNumber)
    {
        var query = new GetPaymentsListQuery(PageNumber: pageNumber);

        var result = _validator.Validate(query);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(GetPaymentsListQuery.PageNumber));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(101)]
    public void Validate_WithOutOfRangePageSize_HasError(int pageSize)
    {
        var query = new GetPaymentsListQuery(PageSize: pageSize);

        var result = _validator.Validate(query);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(GetPaymentsListQuery.PageSize));
    }
}
