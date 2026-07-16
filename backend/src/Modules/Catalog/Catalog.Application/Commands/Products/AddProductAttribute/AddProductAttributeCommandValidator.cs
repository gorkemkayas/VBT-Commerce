using FluentValidation;

namespace Catalog.Application.Commands.Products.AddProductAttribute;

public class AddProductAttributeCommandValidator : AbstractValidator<AddProductAttributeCommand>
{
    public AddProductAttributeCommandValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Value).NotEmpty().MaximumLength(500);
        RuleFor(x => x.DisplayOrder).GreaterThanOrEqualTo(0);
    }
}
