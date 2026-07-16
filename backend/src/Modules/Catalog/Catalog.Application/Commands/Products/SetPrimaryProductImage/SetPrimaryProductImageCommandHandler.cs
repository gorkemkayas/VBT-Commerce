using Catalog.Application.Abstractions;
using Catalog.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Commands.Products.SetPrimaryProductImage;

public class SetPrimaryProductImageCommandHandler(ICatalogDbContext dbContext) : IRequestHandler<SetPrimaryProductImageCommand, Unit>
{
    public async Task<Unit> Handle(SetPrimaryProductImageCommand request, CancellationToken cancellationToken)
    {
        var product = await dbContext.Products
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken)
            ?? throw new ProductNotFoundException(request.ProductId);

        product.SetPrimaryImage(request.ImageId);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
