using Catalog.Application.Abstractions;
using Catalog.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Commands.Products.RemoveProductImage;

public class RemoveProductImageCommandHandler(ICatalogDbContext dbContext) : IRequestHandler<RemoveProductImageCommand, Unit>
{
    public async Task<Unit> Handle(RemoveProductImageCommand request, CancellationToken cancellationToken)
    {
        var product = await dbContext.Products
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken)
            ?? throw new ProductNotFoundException(request.ProductId);

        var image = product.Images.FirstOrDefault(i => i.Id == request.ImageId)
            ?? throw new ProductImageNotFoundException(request.ImageId);

        product.RemoveImage(request.ImageId);
        dbContext.ProductImages.Remove(image);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
