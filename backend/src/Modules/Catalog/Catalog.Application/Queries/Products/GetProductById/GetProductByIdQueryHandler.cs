using Catalog.Application.Abstractions;
using Catalog.Application.Common;
using Catalog.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Queries.Products.GetProductById;

public class GetProductByIdQueryHandler(ICatalogDbContext dbContext) : IRequestHandler<GetProductByIdQuery, ProductDto>
{
    public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await dbContext.Products
            .AsNoTracking()
            .Include(p => p.Attributes)
            .Include(p => p.VariantAttributes)
            .Include(p => p.Variants).ThenInclude(v => v.OptionValues)
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken)
            ?? throw new ProductNotFoundException(request.ProductId);

        return ProductMapper.ToDto(product);
    }
}
