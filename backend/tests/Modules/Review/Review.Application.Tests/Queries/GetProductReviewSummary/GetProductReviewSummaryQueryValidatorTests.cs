using FluentAssertions;
using Review.Application.Queries.GetProductReviewSummary;
using Review.Domain.Enums;
using Xunit;

namespace Review.Application.Tests.Queries.GetProductReviewSummary;

public class GetProductReviewSummaryQueryValidatorTests
{
    private readonly GetProductReviewSummaryQueryValidator _validator = new();

    [Fact]
    public void Validate_WithValidQuery_HasNoErrors()
    {
        var query = new GetProductReviewSummaryQuery(Guid.NewGuid(), ReviewItemType.Product);

        var result = _validator.Validate(query);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptySellableItemId_HasError()
    {
        var query = new GetProductReviewSummaryQuery(Guid.Empty, ReviewItemType.Product);

        var result = _validator.Validate(query);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(GetProductReviewSummaryQuery.SellableItemId));
    }

    [Fact]
    public void Validate_WithInvalidSellableItemType_HasError()
    {
        var query = new GetProductReviewSummaryQuery(Guid.NewGuid(), (ReviewItemType)99);

        var result = _validator.Validate(query);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(GetProductReviewSummaryQuery.SellableItemType));
    }
}
