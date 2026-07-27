using FluentAssertions;
using Review.Application.Queries.GetProductReviewsList;
using Review.Domain.Enums;
using Xunit;

namespace Review.Application.Tests.Queries.GetProductReviewsList;

public class GetProductReviewsListQueryValidatorTests
{
    private readonly GetProductReviewsListQueryValidator _validator = new();

    [Fact]
    public void Validate_WithValidQuery_HasNoErrors()
    {
        var query = new GetProductReviewsListQuery(Guid.NewGuid(), ReviewItemType.Product, 1, 20);

        var result = _validator.Validate(query);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptySellableItemId_HasError()
    {
        var query = new GetProductReviewsListQuery(Guid.Empty, ReviewItemType.Product, 1, 20);

        var result = _validator.Validate(query);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(GetProductReviewsListQuery.SellableItemId));
    }

    [Fact]
    public void Validate_WithInvalidSellableItemType_HasError()
    {
        var query = new GetProductReviewsListQuery(Guid.NewGuid(), (ReviewItemType)99, 1, 20);

        var result = _validator.Validate(query);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(GetProductReviewsListQuery.SellableItemType));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_WithNonPositivePageNumber_HasError(int pageNumber)
    {
        var query = new GetProductReviewsListQuery(Guid.NewGuid(), ReviewItemType.Product, pageNumber, 20);

        var result = _validator.Validate(query);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(GetProductReviewsListQuery.PageNumber));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(101)]
    public void Validate_WithOutOfRangePageSize_HasError(int pageSize)
    {
        var query = new GetProductReviewsListQuery(Guid.NewGuid(), ReviewItemType.Product, 1, pageSize);

        var result = _validator.Validate(query);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(GetProductReviewsListQuery.PageSize));
    }
}
