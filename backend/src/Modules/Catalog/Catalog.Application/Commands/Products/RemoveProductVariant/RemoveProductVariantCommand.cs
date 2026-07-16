using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Application.Security;
using MediatR;

namespace Catalog.Application.Commands.Products.RemoveProductVariant;

public record RemoveProductVariantCommand(Guid ProductId, Guid VariantId) : ICommand<Unit>, IRequireRole
{
    public string[] AllowedRoles => ["Admin"];
}
