using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Application.Security;

namespace Catalog.Application.Commands.Products.AddProductVariantAttribute;

public record AddProductVariantAttributeCommand(
    Guid ProductId,
    string Name,
    int DisplayOrder) : ICommand<Guid>, IRequireRole
{
    public string[] AllowedRoles => ["Admin"];
}
