using FluentValidation;

namespace Catalog.Application.Commands.Products.DeleteProduct;

public class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
{
    public DeleteProductCommandValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
    }
}
