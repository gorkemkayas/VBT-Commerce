using Order.Application.Integrations;
using Shipping.Contracts;

namespace Order.Infrastructure.Integrations;

public class ShippingIntegrationService(IShippingCatalogService shippingCatalogService) : IShippingIntegrationService
{
    public async Task<ShippingCompanySummaryDto?> GetActiveShippingCompanyAsync(Guid shippingCompanyId, CancellationToken cancellationToken)
    {
        var activeCompanies = await shippingCatalogService.GetActiveShippingCompaniesAsync(cancellationToken);
        return activeCompanies.FirstOrDefault(c => c.Id == shippingCompanyId);
    }
}
