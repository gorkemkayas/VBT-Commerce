using FluentValidation;

namespace Catalog.Application.Queries.Categories.GetCategoryById;

public class GetCategoryByIdQueryValidator : AbstractValidator<GetCategoryByIdQuery>
{
    public GetCategoryByIdQueryValidator()
    {
        RuleFor(x => x.CategoryId).NotEmpty();
    }
}
