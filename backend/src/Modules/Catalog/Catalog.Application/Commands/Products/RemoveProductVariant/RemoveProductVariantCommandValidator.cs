using FluentValidation;

namespace Catalog.Application.Commands.Products.RemoveProductVariant;

public class RemoveProductVariantCommandValidator : AbstractValidator<RemoveProductVariantCommand>
{
    public RemoveProductVariantCommandValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.VariantId).NotEmpty();
    }
}
