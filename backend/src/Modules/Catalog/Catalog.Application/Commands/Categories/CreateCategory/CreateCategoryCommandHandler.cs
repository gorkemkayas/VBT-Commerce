using Catalog.Application.Abstractions;
using Catalog.Domain.Entities;
using Catalog.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Commands.Categories.CreateCategory;

public class CreateCategoryCommandHandler(ICatalogDbContext dbContext) : IRequestHandler<CreateCategoryCommand, Guid>
{
    public async Task<Guid> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var normalizedSlug = request.Slug.Trim().ToLowerInvariant();

        var slugAlreadyExists = await dbContext.Categories
            .AnyAsync(c => c.Slug == normalizedSlug, cancellationToken);

        if (slugAlreadyExists)
            throw new DuplicateCategorySlugException(request.Slug);

        if (request.ParentCategoryId is not null)
        {
            var parentExists = await dbContext.Categories
                .AnyAsync(c => c.Id == request.ParentCategoryId, cancellationToken);

            if (!parentExists)
                throw new CategoryNotFoundException(request.ParentCategoryId.Value);
        }

        var category = Category.Create(
            request.Name,
            request.Slug,
            request.Description,
            request.ParentCategoryId,
            request.DisplayOrder);

        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync(cancellationToken);

        return category.Id;
    }
}
