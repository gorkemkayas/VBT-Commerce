using Catalog.Application.Abstractions;
using Catalog.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Queries.Products.GetProductsByIds;

public class GetProductsByIdsQueryHandler(ICatalogDbContext dbContext)
    : IRequestHandler<GetProductsByIdsQuery, IReadOnlyCollection<ProductDto>>
{
    public async Task<IReadOnlyCollection<ProductDto>> Handle(GetProductsByIdsQuery request, CancellationToken cancellationToken)
    {
        var products = await dbContext.Products
            .AsNoTracking()
            .Where(p => request.ProductIds.Contains(p.Id))
            .Include(p => p.Attributes)
            .Include(p => p.VariantAttributes)
            .Include(p => p.Variants).ThenInclude(v => v.OptionValues)
            .Include(p => p.Images)
            .ToListAsync(cancellationToken);

        return products.Select(ProductMapper.ToDto).ToList();
    }
}
