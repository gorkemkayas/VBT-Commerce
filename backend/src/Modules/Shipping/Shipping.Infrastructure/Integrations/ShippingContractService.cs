using MediatR;
using Microsoft.EntityFrameworkCore;
using Shipping.Application.Commands.Shipments.CreateShipment;
using Shipping.Contracts;
using Shipping.Infrastructure.Persistence;

namespace Shipping.Infrastructure.Integrations;

/// <summary>
/// Implementation of the Shipping module's outbound contract (see architecture.md §3 —
/// contracts/integrations). Other modules consume this instead of touching
/// <see cref="ShippingDbContext"/> or Shipping.Application directly.
/// </summary>
public class ShippingContractService(ShippingDbContext dbContext, ISender sender) : IShippingCatalogService
{
    public async Task<ShippingCompanySummaryDto?> GetShippingCompanyAsync(Guid shippingCompanyId, CancellationToken cancellationToken)
    {
        return await dbContext.ShippingCompanies
            .AsNoTracking()
            .Where(c => c.Id == shippingCompanyId)
            .Select(c => new ShippingCompanySummaryDto(c.Id, c.Name, c.Fee))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ShippingCompanySummaryDto>> GetActiveShippingCompaniesAsync(CancellationToken cancellationToken)
    {
        return await dbContext.ShippingCompanies
            .AsNoTracking()
            .Where(c => c.IsActive)
            .Select(c => new ShippingCompanySummaryDto(c.Id, c.Name, c.Fee))
            .ToListAsync(cancellationToken);
    }

    public Task<Guid> CreateShipmentAsync(Guid orderId, Guid shippingCompanyId, CancellationToken cancellationToken)
        => sender.Send(new CreateShipmentCommand(orderId, shippingCompanyId), cancellationToken);

    public async Task<ShipmentTrackingDto?> GetShipmentByOrderIdAsync(Guid orderId, CancellationToken cancellationToken)
    {
        var shipment = await dbContext.Shipments
            .AsNoTracking()
            .Include(s => s.StatusHistory)
            .FirstOrDefaultAsync(s => s.OrderId == orderId, cancellationToken);

        if (shipment is null)
            return null;

        return new ShipmentTrackingDto(
            shipment.Id,
            shipment.Status,
            shipment.TrackingNumber,
            shipment.CreatedAt,
            shipment.UpdatedAt,
            shipment.StatusHistory
                .OrderBy(h => h.CreatedAt)
                .Select(h => new ShipmentStatusHistoryDto(h.Status, h.TrackingNumber, h.CreatedAt))
                .ToList());
    }
}
