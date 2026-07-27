using FluentAssertions;
using Pricing.Application.Queries.Coupons.GetCouponsList;
using Xunit;

namespace Pricing.Application.Tests.Queries.Coupons.GetCouponsList;

public class GetCouponsListQueryValidatorTests
{
    private readonly GetCouponsListQueryValidator _validator = new();

    [Fact]
    public void Validate_WithDefaultValues_HasNoErrors()
    {
        var result = _validator.Validate(new GetCouponsListQuery());

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithZeroPageNumber_HasError()
    {
        var result = _validator.Validate(new GetCouponsListQuery(0, 20));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(GetCouponsListQuery.PageNumber));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(101)]
    public void Validate_WithPageSizeOutOfRange_HasError(int pageSize)
    {
        var result = _validator.Validate(new GetCouponsListQuery(1, pageSize));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(GetCouponsListQuery.PageSize));
    }
}
