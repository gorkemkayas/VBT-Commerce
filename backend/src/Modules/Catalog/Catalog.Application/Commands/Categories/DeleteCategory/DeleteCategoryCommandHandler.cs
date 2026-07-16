using Catalog.Application.Abstractions;
using Catalog.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Commands.Categories.DeleteCategory;

public class DeleteCategoryCommandHandler(ICatalogDbContext dbContext) : IRequestHandler<DeleteCategoryCommand, Unit>
{
    public async Task<Unit> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await dbContext.Categories
            .FirstOrDefaultAsync(c => c.Id == request.CategoryId, cancellationToken)
            ?? throw new CategoryNotFoundException(request.CategoryId);

        var hasActiveChildren = await dbContext.Categories
            .AnyAsync(c => c.ParentCategoryId == request.CategoryId && c.IsActive, cancellationToken);

        if (hasActiveChildren)
            throw new CategoryHasActiveChildrenException(request.CategoryId);

        var hasActiveProducts = await dbContext.Products
            .AnyAsync(p => p.CategoryId == request.CategoryId && p.IsActive, cancellationToken);

        if (hasActiveProducts)
            throw new CategoryHasActiveProductsException(request.CategoryId);

        category.Deactivate();
        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
