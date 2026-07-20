namespace Shipping.Contracts;

public interface IShippingCatalogService
{
    Task<ShippingCompanySummaryDto?> GetShippingCompanyAsync(Guid shippingCompanyId, CancellationToken cancellationToken);

    Task<IReadOnlyList<ShippingCompanySummaryDto>> GetActiveShippingCompaniesAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Creates a shipment for an order with the given shipping company (architecture.md §3). Meant
    /// to be called in-process by the Order module's checkout saga once the order id is known.
    /// </summary>
    Task<Guid> CreateShipmentAsync(Guid orderId, Guid shippingCompanyId, CancellationToken cancellationToken);
}
