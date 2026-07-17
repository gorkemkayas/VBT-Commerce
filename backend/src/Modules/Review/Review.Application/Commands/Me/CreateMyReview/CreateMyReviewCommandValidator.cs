using FluentValidation;

namespace Review.Application.Commands.Me.CreateMyReview;

public class CreateMyReviewCommandValidator : AbstractValidator<CreateMyReviewCommand>
{
    public CreateMyReviewCommandValidator()
    {
        RuleFor(x => x.SellableItemId).NotEmpty();
        RuleFor(x => x.SellableItemType).IsInEnum();
        RuleFor(x => x.Rating).InclusiveBetween(1, 5);
        RuleFor(x => x.Comment).NotEmpty().MaximumLength(2000);
    }
}
