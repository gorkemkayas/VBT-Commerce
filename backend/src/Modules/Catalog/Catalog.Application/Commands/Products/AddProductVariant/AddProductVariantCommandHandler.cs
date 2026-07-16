using Catalog.Application.Abstractions;
using Catalog.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Commands.Products.AddProductVariant;

public class AddProductVariantCommandHandler(ICatalogDbContext dbContext) : IRequestHandler<AddProductVariantCommand, Guid>
{
    public async Task<Guid> Handle(AddProductVariantCommand request, CancellationToken cancellationToken)
    {
        var product = await dbContext.Products
            .Include(p => p.VariantAttributes)
            .Include(p => p.Variants).ThenInclude(v => v.OptionValues)
            .FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken)
            ?? throw new ProductNotFoundException(request.ProductId);

        var variant = product.AddVariant(request.Sku, request.OptionValues);
        dbContext.ProductVariants.Add(variant);
        dbContext.ProductVariantOptionValues.AddRange(variant.OptionValues);
        await dbContext.SaveChangesAsync(cancellationToken);

        return variant.Id;
    }
}
