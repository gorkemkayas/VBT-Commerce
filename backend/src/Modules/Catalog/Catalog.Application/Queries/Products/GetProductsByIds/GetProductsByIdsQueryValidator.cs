using FluentValidation;

namespace Catalog.Application.Queries.Products.GetProductsByIds;

public class GetProductsByIdsQueryValidator : AbstractValidator<GetProductsByIdsQuery>
{
    public GetProductsByIdsQueryValidator()
    {
        RuleFor(x => x.ProductIds).NotEmpty();
    }
}
