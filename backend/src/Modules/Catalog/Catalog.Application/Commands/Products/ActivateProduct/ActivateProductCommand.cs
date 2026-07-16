using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Application.Security;
using MediatR;

namespace Catalog.Application.Commands.Products.ActivateProduct;

public record ActivateProductCommand(Guid ProductId) : ICommand<Unit>, IRequireRole
{
    public string[] AllowedRoles => ["Admin"];
}
