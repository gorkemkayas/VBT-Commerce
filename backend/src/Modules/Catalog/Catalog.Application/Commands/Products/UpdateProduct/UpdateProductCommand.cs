using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Application.Security;
using MediatR;

namespace Catalog.Application.Commands.Products.UpdateProduct;

public record UpdateProductCommand(
    Guid ProductId,
    string Name,
    string Slug,
    string? Description,
    Guid CategoryId) : ICommand<Unit>, IRequireRole
{
    public string[] AllowedRoles => ["Admin"];
}
