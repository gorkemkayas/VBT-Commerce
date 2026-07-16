using Catalog.Application.Abstractions;
using Catalog.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Commands.Categories.UpdateCategory;

public class UpdateCategoryCommandHandler(ICatalogDbContext dbContext) : IRequestHandler<UpdateCategoryCommand, Unit>
{
    public async Task<Unit> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await dbContext.Categories
            .FirstOrDefaultAsync(c => c.Id == request.CategoryId, cancellationToken)
            ?? throw new CategoryNotFoundException(request.CategoryId);

        var normalizedSlug = request.Slug.Trim().ToLowerInvariant();

        var slugAlreadyExists = await dbContext.Categories
            .AnyAsync(c => c.Id != request.CategoryId && c.Slug == normalizedSlug, cancellationToken);

        if (slugAlreadyExists)
            throw new DuplicateCategorySlugException(request.Slug);

        if (request.ParentCategoryId != category.ParentCategoryId)
            await ValidateNewParentAsync(request.CategoryId, request.ParentCategoryId, cancellationToken);

        category.Update(request.Name, request.Slug, request.Description, request.DisplayOrder);
        category.ChangeParent(request.ParentCategoryId);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }

    private async Task ValidateNewParentAsync(Guid categoryId, Guid? parentCategoryId, CancellationToken cancellationToken)
    {
        if (parentCategoryId is null)
            return;

        if (parentCategoryId == categoryId)
            throw new CategoryCannotBeOwnParentException(categoryId);

        var ancestorId = parentCategoryId;
        while (ancestorId is not null)
        {
            var ancestor = await dbContext.Categories
                .FirstOrDefaultAsync(c => c.Id == ancestorId, cancellationToken)
                ?? throw new CategoryNotFoundException(ancestorId.Value);

            if (ancestor.Id == categoryId)
                throw new CircularCategoryReferenceException(categoryId, parentCategoryId.Value);

            ancestorId = ancestor.ParentCategoryId;
        }
    }
}
