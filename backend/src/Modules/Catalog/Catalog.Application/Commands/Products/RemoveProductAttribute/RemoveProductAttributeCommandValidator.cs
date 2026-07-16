using FluentValidation;

namespace Catalog.Application.Commands.Products.RemoveProductAttribute;

public class RemoveProductAttributeCommandValidator : AbstractValidator<RemoveProductAttributeCommand>
{
    public RemoveProductAttributeCommandValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.AttributeId).NotEmpty();
    }
}
