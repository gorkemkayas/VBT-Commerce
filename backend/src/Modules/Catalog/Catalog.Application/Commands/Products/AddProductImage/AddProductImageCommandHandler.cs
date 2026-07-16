using Catalog.Application.Abstractions;
using Catalog.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Commands.Products.AddProductImage;

public class AddProductImageCommandHandler(ICatalogDbContext dbContext) : IRequestHandler<AddProductImageCommand, Guid>
{
    public async Task<Guid> Handle(AddProductImageCommand request, CancellationToken cancellationToken)
    {
        var product = await dbContext.Products
            .Include(p => p.Images)
            .Include(p => p.Variants)
            .FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken)
            ?? throw new ProductNotFoundException(request.ProductId);

        var image = product.AddImage(request.Url, request.DisplayOrder, request.IsPrimary, request.ProductVariantId);
        dbContext.ProductImages.Add(image);
        await dbContext.SaveChangesAsync(cancellationToken);

        return image.Id;
    }
}
