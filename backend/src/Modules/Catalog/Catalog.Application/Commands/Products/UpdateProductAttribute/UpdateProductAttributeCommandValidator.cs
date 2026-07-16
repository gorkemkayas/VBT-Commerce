using FluentValidation;

namespace Catalog.Application.Commands.Products.UpdateProductAttribute;

public class UpdateProductAttributeCommandValidator : AbstractValidator<UpdateProductAttributeCommand>
{
    public UpdateProductAttributeCommandValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.AttributeId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Value).NotEmpty().MaximumLength(500);
        RuleFor(x => x.DisplayOrder).GreaterThanOrEqualTo(0);
    }
}
