using FluentValidation;

namespace Catalog.Application.Commands.Categories.DeleteCategory;

public class DeleteCategoryCommandValidator : AbstractValidator<DeleteCategoryCommand>
{
    public DeleteCategoryCommandValidator()
    {
        RuleFor(x => x.CategoryId).NotEmpty();
    }
}
