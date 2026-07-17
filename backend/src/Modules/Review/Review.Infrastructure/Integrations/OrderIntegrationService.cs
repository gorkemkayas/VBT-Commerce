using Order.Contracts;
using Review.Application.Integrations;
using Review.Domain.Enums;

namespace Review.Infrastructure.Integrations;

/// <summary>
/// Adapts Review's own <see cref="IOrderIntegrationService"/> to Order's actual contract
/// (<see cref="IOrderPurchaseVerifier"/>), so Review.Application never references Order directly.
/// Review's own <see cref="ReviewItemType"/> maps 1:1 onto Order's <see cref="OrderItemType"/> since
/// both describe the same Product-or-Variant distinction.
/// </summary>
public class OrderIntegrationService(IOrderPurchaseVerifier orderPurchaseVerifier) : IOrderIntegrationService
{
    public Task<bool> HasPurchasedItemAsync(Guid userId, Guid sellableItemId, ReviewItemType sellableItemType, CancellationToken cancellationToken)
    {
        var orderItemType = sellableItemType switch
        {
            ReviewItemType.Product => Order.Domain.Enums.OrderItemType.Product,
            ReviewItemType.Variant => Order.Domain.Enums.OrderItemType.Variant,
            _ => throw new ArgumentOutOfRangeException(nameof(sellableItemType))
        };

        return orderPurchaseVerifier.HasCustomerPurchasedItemAsync(userId, sellableItemId, orderItemType, cancellationToken);
    }
}
