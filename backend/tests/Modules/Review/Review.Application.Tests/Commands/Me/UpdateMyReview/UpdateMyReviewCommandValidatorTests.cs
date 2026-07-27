using FluentAssertions;
using Review.Application.Commands.Me.UpdateMyReview;
using Xunit;

namespace Review.Application.Tests.Commands.Me.UpdateMyReview;

public class UpdateMyReviewCommandValidatorTests
{
    private readonly UpdateMyReviewCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_HasNoErrors()
    {
        var command = new UpdateMyReviewCommand(Guid.NewGuid(), 4, "Pretty good");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyReviewId_HasError()
    {
        var command = new UpdateMyReviewCommand(Guid.Empty, 4, "Pretty good");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateMyReviewCommand.ReviewId));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    [InlineData(-1)]
    public void Validate_WithOutOfRangeRating_HasError(int rating)
    {
        var command = new UpdateMyReviewCommand(Guid.NewGuid(), rating, "Pretty good");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateMyReviewCommand.Rating));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_WithEmptyComment_HasError(string comment)
    {
        var command = new UpdateMyReviewCommand(Guid.NewGuid(), 4, comment);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateMyReviewCommand.Comment));
    }

    [Fact]
    public void Validate_WithCommentTooLong_HasError()
    {
        var command = new UpdateMyReviewCommand(Guid.NewGuid(), 4, new string('a', 2001));

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateMyReviewCommand.Comment));
    }
}
