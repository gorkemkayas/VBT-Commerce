using Catalog.Application.Abstractions;
using Catalog.Domain.Entities;
using Catalog.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Commands.Products.CreateProduct;

public class CreateProductCommandHandler(ICatalogDbContext dbContext) : IRequestHandler<CreateProductCommand, Guid>
{
    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var categoryExists = await dbContext.Categories
            .AnyAsync(c => c.Id == request.CategoryId, cancellationToken);

        if (!categoryExists)
            throw new CategoryNotFoundException(request.CategoryId);

        var normalizedSlug = request.Slug.Trim().ToLowerInvariant();

        var slugAlreadyExists = await dbContext.Products
            .AnyAsync(p => p.Slug == normalizedSlug, cancellationToken);

        if (slugAlreadyExists)
            throw new DuplicateProductSlugException(request.Slug);

        var product = Product.Create(request.Name, request.Slug, request.Description, request.CategoryId, request.ProductType);

        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(cancellationToken);

        return product.Id;
    }
}
