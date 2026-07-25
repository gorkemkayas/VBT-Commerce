using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Application.Security;

namespace Catalog.Application.Commands.Categories.CreateCategory;

public record CreateCategoryCommand(
    string Name,
    string Slug,
    string? Description,
    string? ImageUrl,
    Guid? ParentCategoryId,
    int DisplayOrder) : ICommand<Guid>, IRequireRole
{
    public string[] AllowedRoles => ["Admin"];
}
