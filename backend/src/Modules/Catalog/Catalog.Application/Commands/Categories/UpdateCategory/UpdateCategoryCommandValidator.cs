using FluentValidation;

namespace Catalog.Application.Commands.Categories.UpdateCategory;

public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryCommandValidator()
    {
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Slug).NotEmpty().MaximumLength(150).Matches("^[a-z0-9]+(?:-[a-z0-9]+)*$")
            .WithMessage("Slug must contain only lowercase letters, numbers and hyphens.");
        RuleFor(x => x.Description).MaximumLength(2000);
        RuleFor(x => x.DisplayOrder).GreaterThanOrEqualTo(0);
    }
}
