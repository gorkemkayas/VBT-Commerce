using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Application.Security;
using MediatR;

namespace Catalog.Application.Commands.Products.UpdateProductAttribute;

public record UpdateProductAttributeCommand(
    Guid ProductId,
    Guid AttributeId,
    string Name,
    string Value,
    int DisplayOrder) : ICommand<Unit>, IRequireRole
{
    public string[] AllowedRoles => ["Admin"];
}
