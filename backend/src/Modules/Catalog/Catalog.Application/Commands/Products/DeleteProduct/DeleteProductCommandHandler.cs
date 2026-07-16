using Catalog.Application.Abstractions;
using Catalog.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Commands.Products.DeleteProduct;

public class DeleteProductCommandHandler(ICatalogDbContext dbContext) : IRequestHandler<DeleteProductCommand, Unit>
{
    public async Task<Unit> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await dbContext.Products
            .FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken)
            ?? throw new ProductNotFoundException(request.ProductId);

        product.Deactivate();
        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
