using FluentAssertions;
using Pricing.Application.Queries.Coupons.GetCouponByCode;
using Xunit;

namespace Pricing.Application.Tests.Queries.Coupons.GetCouponByCode;

public class GetCouponByCodeQueryValidatorTests
{
    private readonly GetCouponByCodeQueryValidator _validator = new();

    [Fact]
    public void Validate_WithValidCode_HasNoErrors()
    {
        var result = _validator.Validate(new GetCouponByCodeQuery("SAVE10"));

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_WithEmptyCode_HasError(string code)
    {
        var result = _validator.Validate(new GetCouponByCodeQuery(code));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(GetCouponByCodeQuery.Code));
    }
}
