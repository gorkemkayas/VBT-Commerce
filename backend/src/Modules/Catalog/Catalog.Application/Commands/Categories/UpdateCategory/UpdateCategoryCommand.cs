using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Application.Security;
using MediatR;

namespace Catalog.Application.Commands.Categories.UpdateCategory;

public record UpdateCategoryCommand(
    Guid CategoryId,
    string Name,
    string Slug,
    string? Description,
    int DisplayOrder,
    Guid? ParentCategoryId) : ICommand<Unit>, IRequireRole
{
    public string[] AllowedRoles => ["Admin"];
}
