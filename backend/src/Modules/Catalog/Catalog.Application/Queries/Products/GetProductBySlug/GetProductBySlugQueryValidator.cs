using FluentValidation;

namespace Catalog.Application.Queries.Products.GetProductBySlug;

public class GetProductBySlugQueryValidator : AbstractValidator<GetProductBySlugQuery>
{
    public GetProductBySlugQueryValidator()
    {
        RuleFor(x => x.Slug).NotEmpty();
    }
}
