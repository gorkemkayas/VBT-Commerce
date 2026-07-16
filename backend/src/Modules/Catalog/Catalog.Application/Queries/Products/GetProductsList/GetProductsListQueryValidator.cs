using FluentValidation;

namespace Catalog.Application.Queries.Products.GetProductsList;

public class GetProductsListQueryValidator : AbstractValidator<GetProductsListQuery>
{
    public GetProductsListQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
