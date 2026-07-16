using FluentValidation;

namespace Catalog.Application.Commands.Products.AddProductVariant;

public class AddProductVariantCommandValidator : AbstractValidator<AddProductVariantCommand>
{
    public AddProductVariantCommandValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.Sku).NotEmpty().MaximumLength(100);
        RuleFor(x => x.OptionValues).NotNull();
        RuleForEach(x => x.OptionValues).ChildRules(value =>
        {
            value.RuleFor(kv => kv.Key).NotEmpty();
            value.RuleFor(kv => kv.Value).NotEmpty().MaximumLength(200);
        });
    }
}
