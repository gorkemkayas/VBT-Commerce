using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Application.Security;
using Catalog.Domain.Enums;

namespace Catalog.Application.Commands.Products.CreateProduct;

public record CreateProductCommand(
    string Name,
    string Slug,
    string? Description,
    Guid CategoryId,
    ProductType ProductType) : ICommand<Guid>, IRequireRole
{
    public string[] AllowedRoles => ["Admin"];
}
