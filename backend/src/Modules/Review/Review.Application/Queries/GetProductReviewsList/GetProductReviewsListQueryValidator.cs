using FluentValidation;

namespace Review.Application.Queries.GetProductReviewsList;

public class GetProductReviewsListQueryValidator : AbstractValidator<GetProductReviewsListQuery>
{
    public GetProductReviewsListQueryValidator()
    {
        RuleFor(x => x.SellableItemId).NotEmpty();
        RuleFor(x => x.SellableItemType).IsInEnum();
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
