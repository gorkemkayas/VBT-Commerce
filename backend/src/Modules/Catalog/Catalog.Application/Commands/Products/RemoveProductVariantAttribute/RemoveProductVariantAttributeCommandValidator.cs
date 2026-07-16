using FluentValidation;

namespace Catalog.Application.Commands.Products.RemoveProductVariantAttribute;

public class RemoveProductVariantAttributeCommandValidator : AbstractValidator<RemoveProductVariantAttributeCommand>
{
    public RemoveProductVariantAttributeCommandValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.VariantAttributeId).NotEmpty();
    }
}
