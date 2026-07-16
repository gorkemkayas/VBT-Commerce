using FluentValidation;

namespace Catalog.Application.Queries.Products.GetProductById;

public class GetProductByIdQueryValidator : AbstractValidator<GetProductByIdQuery>
{
    public GetProductByIdQueryValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
    }
}
