using Catalog.Application.Abstractions;
using Catalog.Application.Common;
using Catalog.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Queries.Categories.GetCategoryById;

public class GetCategoryByIdQueryHandler(ICatalogDbContext dbContext) : IRequestHandler<GetCategoryByIdQuery, CategoryDto>
{
    public async Task<CategoryDto> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await dbContext.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.CategoryId, cancellationToken)
            ?? throw new CategoryNotFoundException(request.CategoryId);

        return new CategoryDto(
            category.Id,
            category.Name,
            category.Slug,
            category.Description,
            category.ParentCategoryId,
            category.DisplayOrder,
            category.IsActive);
    }
}
