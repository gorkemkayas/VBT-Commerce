using Catalog.Application.Abstractions;
using Catalog.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Commands.Products.AddProductVariantAttribute;

public class AddProductVariantAttributeCommandHandler(ICatalogDbContext dbContext)
    : IRequestHandler<AddProductVariantAttributeCommand, Guid>
{
    public async Task<Guid> Handle(AddProductVariantAttributeCommand request, CancellationToken cancellationToken)
    {
        var product = await dbContext.Products
            .Include(p => p.VariantAttributes)
            .FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken)
            ?? throw new ProductNotFoundException(request.ProductId);

        var variantAttribute = product.AddVariantAttribute(request.Name, request.DisplayOrder);
        dbContext.ProductVariantAttributes.Add(variantAttribute);
        await dbContext.SaveChangesAsync(cancellationToken);

        return variantAttribute.Id;
    }
}
