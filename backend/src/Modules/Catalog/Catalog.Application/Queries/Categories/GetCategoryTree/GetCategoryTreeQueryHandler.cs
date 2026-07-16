using Catalog.Application.Abstractions;
using Catalog.Application.Common;
using Catalog.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Queries.Categories.GetCategoryTree;

public class GetCategoryTreeQueryHandler(ICatalogDbContext dbContext)
    : IRequestHandler<GetCategoryTreeQuery, IReadOnlyCollection<CategoryTreeDto>>
{
    public async Task<IReadOnlyCollection<CategoryTreeDto>> Handle(GetCategoryTreeQuery request, CancellationToken cancellationToken)
    {
        var categories = await dbContext.Categories
            .AsNoTracking()
            .Where(c => request.IncludeInactive || c.IsActive)
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync(cancellationToken);

        var childrenByParentId = categories
            .Where(c => c.ParentCategoryId is not null)
            .GroupBy(c => c.ParentCategoryId!.Value)
            .ToDictionary(g => g.Key, g => g.ToList());

        List<CategoryTreeDto> BuildNodes(Guid? parentId)
        {
            var siblings = parentId is null
                ? categories.Where(c => c.ParentCategoryId is null)
                : childrenByParentId.GetValueOrDefault(parentId.Value, []);

            return siblings.Select(c => ToTreeDto(c, BuildNodes(c.Id))).ToList();
        }

        return BuildNodes(null);
    }

    private static CategoryTreeDto ToTreeDto(Category category, IReadOnlyCollection<CategoryTreeDto> children) => new(
        category.Id,
        category.Name,
        category.Slug,
        category.Description,
        category.DisplayOrder,
        category.IsActive,
        children);
}
