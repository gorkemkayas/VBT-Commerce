using Catalog.Application.Abstractions;
using Catalog.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Queries.Products.GetProductsList;

public class GetProductsListQueryHandler(ICatalogDbContext dbContext)
    : IRequestHandler<GetProductsListQuery, PagedResult<ProductListItemDto>>
{
    public async Task<PagedResult<ProductListItemDto>> Handle(GetProductsListQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.Products.AsNoTracking().AsQueryable();

        if (request.CategoryId is not null)
            query = query.Where(p => p.CategoryId == request.CategoryId);

        if (request.IsActive is not null)
            query = query.Where(p => p.IsActive == request.IsActive);

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.Trim();
            query = query.Where(p => EF.Functions.Like(p.Name, $"%{term}%"));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(p => p.Name)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => new ProductListItemDto(
                p.Id,
                p.Name,
                p.Slug,
                p.CategoryId,
                p.ProductType,
                p.IsActive,
                p.Images.Where(i => i.IsPrimary).Select(i => i.Url).FirstOrDefault()))
            .ToListAsync(cancellationToken);

        return new PagedResult<ProductListItemDto>(items, request.PageNumber, request.PageSize, totalCount);
    }
}
