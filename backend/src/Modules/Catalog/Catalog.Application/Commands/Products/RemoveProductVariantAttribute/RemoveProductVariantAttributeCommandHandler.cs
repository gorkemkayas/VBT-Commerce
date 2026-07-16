using Catalog.Application.Abstractions;
using Catalog.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Commands.Products.RemoveProductVariantAttribute;

public class RemoveProductVariantAttributeCommandHandler(ICatalogDbContext dbContext)
    : IRequestHandler<RemoveProductVariantAttributeCommand, Unit>
{
    public async Task<Unit> Handle(RemoveProductVariantAttributeCommand request, CancellationToken cancellationToken)
    {
        var product = await dbContext.Products
            .Include(p => p.VariantAttributes)
            .Include(p => p.Variants).ThenInclude(v => v.OptionValues)
            .FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken)
            ?? throw new ProductNotFoundException(request.ProductId);

        var variantAttribute = product.VariantAttributes.FirstOrDefault(a => a.Id == request.VariantAttributeId)
            ?? throw new ProductVariantAttributeNotFoundException(request.VariantAttributeId);

        product.RemoveVariantAttribute(request.VariantAttributeId);
        dbContext.ProductVariantAttributes.Remove(variantAttribute);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
