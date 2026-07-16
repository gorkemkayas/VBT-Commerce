using Catalog.Application.Abstractions;
using Catalog.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Commands.Products.UpdateProductVariant;

public class UpdateProductVariantCommandHandler(ICatalogDbContext dbContext) : IRequestHandler<UpdateProductVariantCommand, Unit>
{
    public async Task<Unit> Handle(UpdateProductVariantCommand request, CancellationToken cancellationToken)
    {
        var product = await dbContext.Products
            .Include(p => p.VariantAttributes)
            .Include(p => p.Variants).ThenInclude(v => v.OptionValues)
            .FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken)
            ?? throw new ProductNotFoundException(request.ProductId);

        var variant = product.Variants.FirstOrDefault(v => v.Id == request.VariantId)
            ?? throw new ProductVariantNotFoundException(request.VariantId);
        var previousOptionValues = variant.OptionValues.ToList();

        product.UpdateVariant(request.VariantId, request.Sku, request.OptionValues, request.IsActive);

        dbContext.ProductVariantOptionValues.RemoveRange(previousOptionValues);
        dbContext.ProductVariantOptionValues.AddRange(variant.OptionValues);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
