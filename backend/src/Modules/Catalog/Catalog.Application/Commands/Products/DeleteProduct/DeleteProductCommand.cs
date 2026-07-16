using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Application.Security;
using MediatR;

namespace Catalog.Application.Commands.Products.DeleteProduct;

public record DeleteProductCommand(Guid ProductId) : ICommand<Unit>, IRequireRole
{
    public string[] AllowedRoles => ["Admin"];
}
