using FluentValidation;

namespace Review.Application.Commands.Me.UpdateMyReview;

public class UpdateMyReviewCommandValidator : AbstractValidator<UpdateMyReviewCommand>
{
    public UpdateMyReviewCommandValidator()
    {
        RuleFor(x => x.ReviewId).NotEmpty();
        RuleFor(x => x.Rating).InclusiveBetween(1, 5);
        RuleFor(x => x.Comment).NotEmpty().MaximumLength(2000);
    }
}
