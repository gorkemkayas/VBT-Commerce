using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Application.Security;

namespace Catalog.Application.Commands.Products.AddProductAttribute;

public record AddProductAttributeCommand(
    Guid ProductId,
    string Name,
    string Value,
    int DisplayOrder) : ICommand<Guid>, IRequireRole
{
    public string[] AllowedRoles => ["Admin"];
}
