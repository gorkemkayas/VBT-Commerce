using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Application.Security;

namespace Catalog.Application.Commands.Products.AddProductVariant;

public record AddProductVariantCommand(
    Guid ProductId,
    string Sku,
    IReadOnlyDictionary<Guid, string> OptionValues) : ICommand<Guid>, IRequireRole
{
    public string[] AllowedRoles => ["Admin"];
}
