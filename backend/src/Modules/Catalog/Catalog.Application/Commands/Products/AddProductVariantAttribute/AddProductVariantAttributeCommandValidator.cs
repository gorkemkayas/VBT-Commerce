using FluentValidation;

namespace Catalog.Application.Commands.Products.AddProductVariantAttribute;

public class AddProductVariantAttributeCommandValidator : AbstractValidator<AddProductVariantAttributeCommand>
{
    public AddProductVariantAttributeCommandValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.DisplayOrder).GreaterThanOrEqualTo(0);
    }
}
