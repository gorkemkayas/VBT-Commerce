using FluentValidation;

namespace Catalog.Application.Commands.Products.SetPrimaryProductImage;

public class SetPrimaryProductImageCommandValidator : AbstractValidator<SetPrimaryProductImageCommand>
{
    public SetPrimaryProductImageCommandValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.ImageId).NotEmpty();
    }
}
