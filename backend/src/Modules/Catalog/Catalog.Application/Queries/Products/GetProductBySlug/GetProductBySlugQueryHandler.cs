using Catalog.Application.Abstractions;
using Catalog.Application.Common;
using Catalog.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Queries.Products.GetProductBySlug;

public class GetProductBySlugQueryHandler(ICatalogDbContext dbContext) : IRequestHandler<GetProductBySlugQuery, ProductDto>
{
    public async Task<ProductDto> Handle(GetProductBySlugQuery request, CancellationToken cancellationToken)
    {
        var normalizedSlug = request.Slug.Trim().ToLowerInvariant();

        var product = await dbContext.Products
            .AsNoTracking()
            .Include(p => p.Attributes)
            .Include(p => p.VariantAttributes)
            .Include(p => p.Variants).ThenInclude(v => v.OptionValues)
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Slug == normalizedSlug, cancellationToken)
            ?? throw new ProductNotFoundException(request.Slug);

        return ProductMapper.ToDto(product);
    }
}
