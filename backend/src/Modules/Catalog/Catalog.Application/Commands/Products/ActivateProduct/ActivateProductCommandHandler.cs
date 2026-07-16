using Catalog.Application.Abstractions;
using Catalog.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Commands.Products.ActivateProduct;

public class ActivateProductCommandHandler(ICatalogDbContext dbContext) : IRequestHandler<ActivateProductCommand, Unit>
{
    public async Task<Unit> Handle(ActivateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await dbContext.Products
            .Include(p => p.Variants)
            .FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken)
            ?? throw new ProductNotFoundException(request.ProductId);

        product.Activate();
        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
