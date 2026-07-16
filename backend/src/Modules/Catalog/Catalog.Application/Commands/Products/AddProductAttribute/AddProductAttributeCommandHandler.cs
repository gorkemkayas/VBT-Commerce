using Catalog.Application.Abstractions;
using Catalog.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Commands.Products.AddProductAttribute;

public class AddProductAttributeCommandHandler(ICatalogDbContext dbContext) : IRequestHandler<AddProductAttributeCommand, Guid>
{
    public async Task<Guid> Handle(AddProductAttributeCommand request, CancellationToken cancellationToken)
    {
        var product = await dbContext.Products
            .Include(p => p.Attributes)
            .FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken)
            ?? throw new ProductNotFoundException(request.ProductId);

        var attribute = product.AddAttribute(request.Name, request.Value, request.DisplayOrder);
        dbContext.ProductAttributes.Add(attribute);
        await dbContext.SaveChangesAsync(cancellationToken);

        return attribute.Id;
    }
}
