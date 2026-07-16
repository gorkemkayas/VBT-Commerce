using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Application.Security;
using MediatR;

namespace Catalog.Application.Commands.Products.RemoveProductVariantAttribute;

public record RemoveProductVariantAttributeCommand(Guid ProductId, Guid VariantAttributeId) : ICommand<Unit>, IRequireRole
{
    public string[] AllowedRoles => ["Admin"];
}
