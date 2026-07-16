using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Application.Security;
using MediatR;

namespace Catalog.Application.Commands.Products.SetPrimaryProductImage;

public record SetPrimaryProductImageCommand(Guid ProductId, Guid ImageId) : ICommand<Unit>, IRequireRole
{
    public string[] AllowedRoles => ["Admin"];
}
