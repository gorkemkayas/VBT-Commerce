using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Application.Security;

namespace Catalog.Application.Commands.Products.AddProductImage;

public record AddProductImageCommand(
    Guid ProductId,
    string Url,
    int DisplayOrder,
    bool IsPrimary,
    Guid? ProductVariantId) : ICommand<Guid>, IRequireRole
{
    public string[] AllowedRoles => ["Admin"];
}
