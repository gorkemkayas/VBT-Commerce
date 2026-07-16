using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Application.Security;
using MediatR;

namespace Catalog.Application.Commands.Products.RemoveProductAttribute;

public record RemoveProductAttributeCommand(Guid ProductId, Guid AttributeId) : ICommand<Unit>, IRequireRole
{
    public string[] AllowedRoles => ["Admin"];
}
