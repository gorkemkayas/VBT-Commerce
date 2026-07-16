using FluentValidation;

namespace Catalog.Application.Commands.Products.UpdateProduct;

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Slug).NotEmpty().MaximumLength(200).Matches("^[a-z0-9]+(?:-[a-z0-9]+)*$")
            .WithMessage("Slug must contain only lowercase letters, numbers and hyphens.");
        RuleFor(x => x.Description).MaximumLength(4000);
        RuleFor(x => x.CategoryId).NotEmpty();
    }
}
