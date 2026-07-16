using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Application.Security;
using MediatR;

namespace Catalog.Application.Commands.Products.UpdateProductVariant;

public record UpdateProductVariantCommand(
    Guid ProductId,
    Guid VariantId,
    string Sku,
    IReadOnlyDictionary<Guid, string> OptionValues,
    bool IsActive) : ICommand<Unit>, IRequireRole
{
    public string[] AllowedRoles => ["Admin"];
}
