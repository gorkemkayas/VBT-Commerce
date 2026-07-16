using Catalog.Application.Abstractions;
using Catalog.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Commands.Products.RemoveProductAttribute;

public class RemoveProductAttributeCommandHandler(ICatalogDbContext dbContext) : IRequestHandler<RemoveProductAttributeCommand, Unit>
{
    public async Task<Unit> Handle(RemoveProductAttributeCommand request, CancellationToken cancellationToken)
    {
        var product = await dbContext.Products
            .Include(p => p.Attributes)
            .FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken)
            ?? throw new ProductNotFoundException(request.ProductId);

        var attribute = product.Attributes.FirstOrDefault(a => a.Id == request.AttributeId)
            ?? throw new ProductAttributeNotFoundException(request.AttributeId);

        product.RemoveAttribute(request.AttributeId);
        dbContext.ProductAttributes.Remove(attribute);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
