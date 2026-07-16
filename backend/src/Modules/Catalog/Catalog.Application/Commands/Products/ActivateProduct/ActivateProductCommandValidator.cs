using FluentValidation;

namespace Catalog.Application.Commands.Products.ActivateProduct;

public class ActivateProductCommandValidator : AbstractValidator<ActivateProductCommand>
{
    public ActivateProductCommandValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
    }
}
