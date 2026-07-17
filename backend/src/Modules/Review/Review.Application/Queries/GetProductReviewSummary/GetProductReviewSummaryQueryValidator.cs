using FluentValidation;

namespace Review.Application.Queries.GetProductReviewSummary;

public class GetProductReviewSummaryQueryValidator : AbstractValidator<GetProductReviewSummaryQuery>
{
    public GetProductReviewSummaryQueryValidator()
    {
        RuleFor(x => x.SellableItemId).NotEmpty();
        RuleFor(x => x.SellableItemType).IsInEnum();
    }
}
