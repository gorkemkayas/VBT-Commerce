using Catalog.Application.Abstractions;
using Catalog.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Commands.Products.RemoveProductVariant;

public class RemoveProductVariantCommandHandler(ICatalogDbContext dbContext) : IRequestHandler<RemoveProductVariantCommand, Unit>
{
    public async Task<Unit> Handle(RemoveProductVariantCommand request, CancellationToken cancellationToken)
    {
        var product = await dbContext.Products
            .Include(p => p.Variants).ThenInclude(v => v.OptionValues)
            .FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken)
            ?? throw new ProductNotFoundException(request.ProductId);

        var variant = product.Variants.FirstOrDefault(v => v.Id == request.VariantId)
            ?? throw new ProductVariantNotFoundException(request.VariantId);

        product.RemoveVariant(request.VariantId);
        dbContext.ProductVariants.Remove(variant);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
