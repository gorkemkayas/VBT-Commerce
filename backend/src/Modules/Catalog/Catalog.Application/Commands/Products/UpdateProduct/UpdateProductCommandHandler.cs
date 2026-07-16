using Catalog.Application.Abstractions;
using Catalog.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Commands.Products.UpdateProduct;

public class UpdateProductCommandHandler(ICatalogDbContext dbContext) : IRequestHandler<UpdateProductCommand, Unit>
{
    public async Task<Unit> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await dbContext.Products
            .FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken)
            ?? throw new ProductNotFoundException(request.ProductId);

        var categoryExists = await dbContext.Categories
            .AnyAsync(c => c.Id == request.CategoryId, cancellationToken);

        if (!categoryExists)
            throw new CategoryNotFoundException(request.CategoryId);

        var normalizedSlug = request.Slug.Trim().ToLowerInvariant();

        var slugAlreadyExists = await dbContext.Products
            .AnyAsync(p => p.Id != request.ProductId && p.Slug == normalizedSlug, cancellationToken);

        if (slugAlreadyExists)
            throw new DuplicateProductSlugException(request.Slug);

        product.Update(request.Name, request.Slug, request.Description, request.CategoryId);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
