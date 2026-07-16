using Catalog.Application.Abstractions;
using Catalog.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Commands.Products.UpdateProductAttribute;

public class UpdateProductAttributeCommandHandler(ICatalogDbContext dbContext) : IRequestHandler<UpdateProductAttributeCommand, Unit>
{
    public async Task<Unit> Handle(UpdateProductAttributeCommand request, CancellationToken cancellationToken)
    {
        var product = await dbContext.Products
            .Include(p => p.Attributes)
            .FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken)
            ?? throw new ProductNotFoundException(request.ProductId);

        product.UpdateAttribute(request.AttributeId, request.Name, request.Value, request.DisplayOrder);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
