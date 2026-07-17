namespace Shipping.Contracts;

public interface IShippingCatalogService
{
    Task<ShippingCompanySummaryDto?> GetShippingCompanyAsync(Guid shippingCompanyId, CancellationToken cancellationToken);

    Task<IReadOnlyList<ShippingCompanySummaryDto>> GetActiveShippingCompaniesAsync(CancellationToken cancellationToken);
}
