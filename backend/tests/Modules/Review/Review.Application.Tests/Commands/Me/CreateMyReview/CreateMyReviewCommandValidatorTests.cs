using FluentAssertions;
using Review.Application.Commands.Me.CreateMyReview;
using Review.Domain.Enums;
using Xunit;

namespace Review.Application.Tests.Commands.Me.CreateMyReview;

public class CreateMyReviewCommandValidatorTests
{
    private readonly CreateMyReviewCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_HasNoErrors()
    {
        var command = new CreateMyReviewCommand(Guid.NewGuid(), ReviewItemType.Product, 5, "Great product");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptySellableItemId_HasError()
    {
        var command = new CreateMyReviewCommand(Guid.Empty, ReviewItemType.Product, 5, "Great product");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateMyReviewCommand.SellableItemId));
    }

    [Fact]
    public void Validate_WithInvalidSellableItemType_HasError()
    {
        var command = new CreateMyReviewCommand(Guid.NewGuid(), (ReviewItemType)99, 5, "Great product");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateMyReviewCommand.SellableItemType));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    [InlineData(-1)]
    public void Validate_WithOutOfRangeRating_HasError(int rating)
    {
        var command = new CreateMyReviewCommand(Guid.NewGuid(), ReviewItemType.Product, rating, "Great product");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateMyReviewCommand.Rating));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_WithEmptyComment_HasError(string comment)
    {
        var command = new CreateMyReviewCommand(Guid.NewGuid(), ReviewItemType.Product, 5, comment);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateMyReviewCommand.Comment));
    }

    [Fact]
    public void Validate_WithCommentTooLong_HasError()
    {
        var command = new CreateMyReviewCommand(Guid.NewGuid(), ReviewItemType.Product, 5, new string('a', 2001));

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateMyReviewCommand.Comment));
    }
}
