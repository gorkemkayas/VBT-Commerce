using Shipping.Contracts;

namespace Order.Application.Integrations;

public interface IShippingIntegrationService
{
    /// <summary>
    /// Returns the shipping company only if it is currently active — <see cref="ShippingCompanySummaryDto"/>
    /// itself carries no IsActive flag, so this checks membership in the active list.
    /// </summary>
    Task<ShippingCompanySummaryDto?> GetActiveShippingCompanyAsync(Guid shippingCompanyId, CancellationToken cancellationToken);
}
